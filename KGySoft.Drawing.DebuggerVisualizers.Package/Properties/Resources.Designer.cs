﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KGySoft.Drawing.DebuggerVisualizers.Package.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KGySoft.Drawing.DebuggerVisualizers.Package.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to KGy SOFT Drawing DebuggerVisualizers.
        /// </summary>
        internal static string _110 {
            get {
                return ResourceManager.GetString("110", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Debugger Visualizers for System.Drawing types such as Image, Bitmap, Metafile, Icon, Graphics, BitmapData, ColorPalette and Color..
        /// </summary>
        internal static string _112 {
            get {
                return ResourceManager.GetString("112", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon _400 {
            get {
                object obj = ResourceManager.GetObject("400", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to install the visualizers to {0}: {1}
        ///
        ///Make sure every running debugger is closed. Installing will be tried again on restarting Visual Studio..
        /// </summary>
        internal static string ErrorMessage_FailedToInstallFormat {
            get {
                return ResourceManager.GetString("ErrorMessage_FailedToInstallFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Shell service could not be obtained. Installation of the debugger visualizers cannot be checked..
        /// </summary>
        internal static string ErrorMessage_ShellServiceUnavailable {
            get {
                return ResourceManager.GetString("ErrorMessage_ShellServiceUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected error occurred: {0}.
        /// </summary>
        internal static string ErrorMessage_UnexpectedErrorFormat {
            get {
                return ResourceManager.GetString("ErrorMessage_UnexpectedErrorFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to KGy SOFT Drawing DebuggerVisualizers {0} have been installed to {1}.
        /// </summary>
        internal static string InfoMessage_InstallationFinishedFormat {
            get {
                return ResourceManager.GetString("InfoMessage_InstallationFinishedFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The installation of KGy SOFT Drawing DebuggerVisualizers to {0} finished with a warning: {1}.
        /// </summary>
        internal static string WarningMessage_InstallationFinishedWithWarningFormat {
            get {
                return ResourceManager.GetString("WarningMessage_InstallationFinishedWithWarningFormat", resourceCulture);
            }
        }
    }
}
