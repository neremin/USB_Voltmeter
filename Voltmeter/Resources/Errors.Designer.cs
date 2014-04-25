﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Voltmeter.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Voltmeter.Resources.Errors", typeof(Errors).Assembly);
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
        ///   Looks up a localized string similar to Could not send/receive data to/from device.
        /// </summary>
        internal static string Data_Request_Failed {
            get {
                return ResourceManager.GetString("Data_Request_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device is opened already.
        /// </summary>
        internal static string Device_Already_Opened {
            get {
                return ResourceManager.GetString("Device_Already_Opened", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not open device.
        /// </summary>
        internal static string Device_Opening_Failed {
            get {
                return ResourceManager.GetString("Device_Opening_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The device &apos;{0}&apos; cannot be accessed because it is being used by another process..
        /// </summary>
        internal static string Device_Sharing_Violation_0 {
            get {
                return ResourceManager.GetString("Device_Sharing_Violation_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Size of DeviceInterfaceDetailData.DevicePath is not enough. {0} bytes are required..
        /// </summary>
        internal static string Insufficient_Path_Buffer_0 {
            get {
                return ResourceManager.GetString("Insufficient_Path_Buffer_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid expression.
        /// </summary>
        internal static string Invalid_Expression {
            get {
                return ResourceManager.GetString("Invalid_Expression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to InfoSet.Size property was not set correctly.
        /// </summary>
        internal static string Invalid_InfoSet_Size {
            get {
                return ResourceManager.GetString("Invalid_InfoSet_Size", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Settings file &apos;{0}&apos; not found or could not be read. Default settings will be applied..
        /// </summary>
        internal static string Invalid_Settings_File_0 {
            get {
                return ResourceManager.GetString("Invalid_Settings_File_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid settings file format &apos;{0}&apos;. Default settings will be applied..
        /// </summary>
        internal static string Invalid_Settings_Format_0 {
            get {
                return ResourceManager.GetString("Invalid_Settings_Format_0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Device is being polled already.
        /// </summary>
        internal static string Polling_Device_Already {
            get {
                return ResourceManager.GetString("Polling_Device_Already", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error writting configuration file to disk.
        /// </summary>
        internal static string Save_Settings_Error {
            get {
                return ResourceManager.GetString("Save_Settings_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Task execution failed.
        /// </summary>
        internal static string Task_Execution_Failed {
            get {
                return ResourceManager.GetString("Task_Execution_Failed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not get reply from device {0} times after {1:c} waiting period.
        /// </summary>
        internal static string Too_Many_Failed_Attempts_1 {
            get {
                return ResourceManager.GetString("Too_Many_Failed_Attempts_1", resourceCulture);
            }
        }
    }
}
