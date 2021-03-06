﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Packaging.PackageCreation.Resources;
using NuGet.Packaging.Xml;

namespace NuGet.Packaging
{
    public class Manifest
    {
        private const string SchemaVersionAttributeName = "schemaVersion";

        public Manifest(ManifestMetadata metadata)
                    : this(metadata, null)
        {
        }

        public Manifest(ManifestMetadata metadata, ICollection<ManifestFile> files)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            Metadata = metadata;

            if (files != null)
            {
                Files = files;
            }
        }

        public ManifestMetadata Metadata { get; }

        public ICollection<ManifestFile> Files { get; } = new List<ManifestFile>();

        /// <summary>
        /// Saves the current manifest to the specified stream.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        public void Save(Stream stream)
        {
            Save(stream, validate: true, minimumManifestVersion: 1);
        }

        /// <summary>
        /// Saves the current manifest to the specified stream.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="minimumManifestVersion">The minimum manifest version that this class must use when saving.</param>
        public void Save(Stream stream, int minimumManifestVersion)
        {
            Save(stream, validate: true, minimumManifestVersion: minimumManifestVersion);
        }

        public void Save(Stream stream, bool validate)
        {
            Save(stream, validate, minimumManifestVersion: 1);
        }

        public void Save(Stream stream, bool validate, int minimumManifestVersion)
        {
            if (validate)
            {
                // Validate before saving
                Validate(this);
            }

            int version = Math.Max(minimumManifestVersion, ManifestVersionUtility.GetManifestVersion(Metadata));
            var schemaNamespace = (XNamespace)ManifestSchemaUtility.GetSchemaNamespace(version);

            new XDocument(
                new XElement(schemaNamespace + "package",
                    Metadata.ToXElement(schemaNamespace),
                    Files.Any() ?
                        new XElement(schemaNamespace + "files",
                            Files.Select(file => new XElement(schemaNamespace + "file",
                                new XAttribute("src", file.Source),
                                file.Target != null ? new XAttribute("target", file.Target) : null,
                                file.Exclude != null ? new XAttribute("exclude", file.Exclude) : null))) : null)).Save(stream);
        }

        public static Manifest ReadFrom(Stream stream, bool validateSchema)
        {
            return ReadFrom(stream, null, validateSchema);
        }

        public static Manifest ReadFrom(Stream stream, Func<string, string> propertyProvider, bool validateSchema)
        {
            XDocument document;
            if (propertyProvider == null)
            {
                document = XmlUtility.LoadSafe(stream, ignoreWhiteSpace: true);
            }
            else
            {
                string content = Preprocessor.Process(stream, propName => propertyProvider(propName));
                document = XDocument.Parse(content);
            }

            string schemaNamespace = GetSchemaNamespace(document);
            foreach (var e in document.Descendants())
            {
                // Assign the schema namespace derived to all nodes in the document.
                e.Name = XName.Get(e.Name.LocalName, schemaNamespace);
            }

            // Validate if the schema is a known one
            CheckSchemaVersion(document);

            if (validateSchema)
            {
                // Validate the schema
                ValidateManifestSchema(document, schemaNamespace);
            }

            // Deserialize it
            var manifest = ManifestReader.ReadManifest(document);

            // Validate before returning
            Validate(manifest);

            return manifest;
        }

        private static string GetSchemaNamespace(XDocument document)
        {
            string schemaNamespace = ManifestSchemaUtility.SchemaVersionV1;
            var rootNameSpace = document.Root.Name.Namespace;
            if (rootNameSpace != null && !String.IsNullOrEmpty(rootNameSpace.NamespaceName))
            {
                schemaNamespace = rootNameSpace.NamespaceName;
            }
            return schemaNamespace;
        }

        public static Manifest Create(IPackageMetadata metadata)
        {
            return new Manifest(new ManifestMetadata(metadata));
        }

        private static void ValidateManifestSchema(XDocument document, string schemaNamespace)
        {
#if !IS_CORECLR // CORECLR_TODO: XmlSchema
            var schemaSet = ManifestSchemaUtility.GetManifestSchemaSet(schemaNamespace);

            document.Validate(schemaSet, (sender, e) =>
            {
                if (e.Severity == XmlSeverityType.Error)
                {
                    // Throw an exception if there is a validation error
                    throw new InvalidOperationException(e.Message);
                }
            });
#endif
        }

        private static void CheckSchemaVersion(XDocument document)
        {
#if !IS_CORECLR // CORECLR_TODO: XmlSchema
            // Get the metadata node and look for the schemaVersion attribute
            XElement metadata = GetMetadataElement(document);

            if (metadata != null)
            {
                // Yank this attribute since we don't want to have to put it in our xsd
                XAttribute schemaVersionAttribute = metadata.Attribute(SchemaVersionAttributeName);

                if (schemaVersionAttribute != null)
                {
                    schemaVersionAttribute.Remove();
                }

                // Get the package id from the metadata node
                string packageId = GetPackageId(metadata);

                // If the schema of the document doesn't match any of our known schemas
                if (!ManifestSchemaUtility.IsKnownSchema(document.Root.Name.Namespace.NamespaceName))
                {
                    throw new InvalidOperationException(
                            String.Format(CultureInfo.CurrentCulture,
                                          NuGetResources.IncompatibleSchema,
                                          packageId,
                                          typeof(Manifest).Assembly.GetName().Version));
                }
            }
#endif
        }

        private static string GetPackageId(XElement metadataElement)
        {
            XName idName = XName.Get("id", metadataElement.Document.Root.Name.NamespaceName);
            XElement element = metadataElement.Element(idName);

            if (element != null)
            {
                return element.Value;
            }

            return null;
        }

        private static XElement GetMetadataElement(XDocument document)
        {
            // Get the metadata element this way so that we don't have to worry about the schema version
            XName metadataName = XName.Get("metadata", document.Root.Name.Namespace.NamespaceName);

            return document.Root.Element(metadataName);
        }

        public static void Validate(Manifest manifest)
        {
            var results = new List<string>();

            // Run all validations
            results.AddRange(manifest.Metadata.Validate());
            foreach(var manifestFile in manifest.Files)
            {
                results.AddRange(manifestFile.Validate());
            }

            if (manifest.Metadata.PackageAssemblyReferences != null)
            {
                foreach (var packageAssemblyReference in manifest.Metadata.PackageAssemblyReferences)
                {
                    results.AddRange(packageAssemblyReference.Validate());
                }
            }

            if (results.Any())
            {
                string message = String.Join(Environment.NewLine, results);
                throw new Exception(message);
            }

            // Validate additional dependency rules dependencies
            ValidateDependencyGroups(manifest.Metadata);
        }

        private static void ValidateDependencyGroups(IPackageMetadata metadata)
        {
            foreach (var dependencyGroup in metadata.DependencyGroups)
            {
                var dependencyHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var dependency in dependencyGroup.Packages)
                {
                    // Throw an error if this dependency has been defined more than once
                    if (!dependencyHash.Add(dependency.Id))
                    {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, NuGetResources.DuplicateDependenciesDefined, metadata.Id, dependency.Id));
                    }

                    // Validate the dependency version
                    ValidateDependencyVersion(dependency);
                }
            }
        }

        private static void ValidateDependencyVersion(PackageDependency dependency)
        {
            if (dependency.VersionRange != null)
            {
                if (dependency.VersionRange.MinVersion != null &&
                    dependency.VersionRange.MaxVersion != null)
                {

                    if ((!dependency.VersionRange.IsMaxInclusive ||
                         !dependency.VersionRange.IsMinInclusive) &&
                        dependency.VersionRange.MaxVersion == dependency.VersionRange.MinVersion)
                    {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, NuGetResources.DependencyHasInvalidVersion, dependency.Id));
                    }

                    if (dependency.VersionRange.MaxVersion < dependency.VersionRange.MinVersion)
                    {
                        throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, NuGetResources.DependencyHasInvalidVersion, dependency.Id));
                    }
                }
            }
        }
    }
}