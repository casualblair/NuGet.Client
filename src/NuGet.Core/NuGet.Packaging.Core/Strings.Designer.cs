﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NuGet.Packaging.Core {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Strings() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NuGet.Packaging.Core.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Nuspec file does not contain the &apos;{0}&apos; node..
        /// </summary>
        public static string MissingMetadataNode {
            get {
                return ResourceManager.GetString("MissingMetadataNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Nuspec file does not exist in package..
        /// </summary>
        public static string MissingNuspec {
            get {
                return ResourceManager.GetString("MissingNuspec", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Nuspec file contains a package type that is missing the name attribute..
        /// </summary>
        public static string MissingPackageTypeName {
            get {
                return ResourceManager.GetString("MissingPackageTypeName", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Package contains multiple nuspec files..
        /// </summary>
        public static string MultipleNuspecFiles {
            get {
                return ResourceManager.GetString("MultipleNuspecFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Nuspec file contains multiple package types. Zero or one package type nodes are allowed..
        /// </summary>
        public static string MultiplePackageTypes {
            get {
                return ResourceManager.GetString("MultiplePackageTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to String argument &apos;{0}&apos; cannot be null or empty.
        /// </summary>
        public static string StringCannotBeNullOrEmpty {
            get {
                return ResourceManager.GetString("StringCannotBeNullOrEmpty", resourceCulture);
            }
        }
    }
}
