﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RIPharmStatutesAggregator.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>http://webserver.rilin.state.ri.us/Statutes/TITLE5/5-19.1/INDEX.HTM</string>
  <string>http://webserver.rilin.state.ri.us/Statutes/TITLE5/5-19.2/INDEX.HTM</string>
  <string>http://webserver.rilin.state.ri.us/Statutes/TITLE21/21-28/INDEX.HTM</string>
  <string>http://webserver.rilin.state.ri.us/Statutes/TITLE21/21-31/INDEX.HTM</string>
</ArrayOfString>")]
        public global::System.Collections.Specialized.StringCollection Addresses {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["Addresses"]));
            }
            set {
                this["Addresses"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SaveLocation {
            get {
                return ((string)(this["SaveLocation"]));
            }
            set {
                this["SaveLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime LastDownloaded {
            get {
                return ((global::System.DateTime)(this["LastDownloaded"]));
            }
            set {
                this["LastDownloaded"] = value;
            }
        }
    }
}
