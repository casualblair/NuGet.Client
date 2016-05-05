// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.Shared;

namespace NuGet.Packaging.Core
{
    public class PackageType : IEquatable<PackageType>
    {
        public static readonly PackageType Legacy = new PackageType("Legacy", version: new Version(0, 0));
        public static readonly PackageType DotnetCliTool = new PackageType("DotnetCliTool", version: new Version(0, 0));
        public static readonly PackageType Dependency = new PackageType("Dependency", version: new Version(0, 0));

        public PackageType(string name, Version version)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(Strings.StringCannotBeNullOrEmpty, nameof(name));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Name = name;
            Version = version;
        }
        
        public string Name { get; }

        public Version Version { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PackageType);
        }

        public bool Equals(PackageType other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name) && Version == other.Version;
        }

        public override int GetHashCode()
        {
            var combiner = new HashCodeCombiner();

            combiner.AddObject(Name);
            combiner.AddInt32(Version.Major);
            combiner.AddInt32(Version.Minor);
            combiner.AddInt32(Version.Build);
            combiner.AddInt32(Version.Revision);

            return combiner.CombinedHash;
        }
    }
}
