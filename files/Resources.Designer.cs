﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DALGenerator.files {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DALGenerator.files.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to using System.Reflection;
        ///using System.Runtime.InteropServices;
        ///
        ///[assembly: AssemblyTitle(&quot;%%ASSEMBLY_TITLE%%&quot;)]
        ///[assembly: AssemblyDescription(&quot;%%ASSEMBLY_TITLE%%&quot;)]
        ///[assembly: AssemblyConfiguration(&quot;&quot;)]
        ///[assembly: AssemblyCompany(&quot;JGhost&quot;)]
        ///[assembly: AssemblyProduct(&quot;%%ASSEMBLY_TITLE%%&quot;)]
        ///[assembly: AssemblyCopyright(&quot;© 2003-%%YEAR%%, BobS JGhost&quot;)]
        ///[assembly: AssemblyTrademark(&quot;&quot;)]
        ///[assembly: AssemblyCulture(&quot;&quot;)]
        ///[assembly: ComVisible(false)]
        ///[assembly: Guid(&quot;%%COM_GUID%%&quot;)]
        ///[assembly: Assem [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string AssemblyInfo_cs {
            get {
                return ResourceManager.GetString("AssemblyInfo_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;Project ToolsVersion=&quot;12.0&quot; DefaultTargets=&quot;Build&quot; xmlns=&quot;http://schemas.microsoft.com/developer/msbuild/2003&quot;&gt;
        ///  &lt;PropertyGroup&gt;
        ///    &lt;Configuration Condition=&quot; &apos;$(Configuration)&apos; == &apos;&apos; &quot;&gt;Debug&lt;/Configuration&gt;
        ///    &lt;Platform Condition=&quot; &apos;$(Platform)&apos; == &apos;&apos; &quot;&gt;AnyCPU&lt;/Platform&gt;
        ///    &lt;ProductVersion&gt;8.0.30703&lt;/ProductVersion&gt;
        ///    &lt;SchemaVersion&gt;2.0&lt;/SchemaVersion&gt;
        ///    &lt;ProjectGuid&gt;{%%PROJECT_GUID%%}&lt;/ProjectGuid&gt;
        ///    &lt;OutputType&gt;Library&lt;/OutputType&gt;
        ///    &lt;RootNames [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DAL_csproj {
            get {
                return ResourceManager.GetString("DAL_csproj", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Microsoft Visual Studio Solution File, Format Version 11.00
        ///Project(&quot;{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}&quot;) = &quot;%%SOLUTION_NAME%%&quot;, &quot;%%PROJECT_NAME%%.csproj&quot;, &quot;{%%PROJECT_GUID%%}&quot;
        ///EndProject
        ///Global
        ///	GlobalSection(SolutionConfigurationPlatforms) = preSolution
        ///		Debug|Any CPU = Debug|Any CPU
        ///		Release|Any CPU = Release|Any CPU
        ///	EndGlobalSection
        ///	GlobalSection(ProjectConfigurationPlatforms) = postSolution
        ///		{%%PROJECT_GUID%%}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        ///		{%%PROJECT_GUID%%}.Debug|Any CPU [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DAL_sln {
            get {
                return ResourceManager.GetString("DAL_sln", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Data;
        ///using System.Data.SqlClient;
        ///
        ///namespace %%NAMESPACE%% {
        ///
        ///    public class DALBase: IDisposable {
        ///        protected SqlConnection dbConnection;
        ///
        ///        #region public DALBase
        ///        /// &lt;summary&gt;
        ///        /// Constructor
        ///        /// &lt;/summary&gt;
        ///        /// &lt;param name=&quot;DbConnection&quot;&gt;&lt;/param&gt;
        ///        public DALBase(SqlConnection DbConnection) {
        ///            this.dbConnection = DbConnection;
        ///        }
        ///        #endregion
        ///
        ///        #region protected SqlCommand Pr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DALBase_cs {
            get {
                return ResourceManager.GetString("DALBase_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///
        ///namespace %%NAMESPACE%% {
        ///
        ///    public class DALException: Exception {
        ///        public DALException() : base() { }
        ///        public DALException(string message) : base(message) { }
        ///        public DALException(string message, Exception innerException) : base(message, innerException) { }
        ///        public DALException(string message, Exception innerException, Dictionary&lt;string, string&gt; Data)
        ///            : base(message, innerException) {
        ///            foreach  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DALException_cs {
            get {
                return ResourceManager.GetString("DALException_cs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Collections.Generic;
        ///
        ///namespace %%NAMESPACE%% {
        ///
        ///    internal static class Extensions {
        ///        public static Int32 ToInt32(this object obj, Int32 Default = 0) {
        ///            try {
        ///                return Convert.ToInt32(obj);
        ///            } catch {
        ///                return Default;
        ///            }
        ///        }
        ///    }
        ///}.
        /// </summary>
        internal static string Extensions_cs {
            get {
                return ResourceManager.GetString("Extensions_cs", resourceCulture);
            }
        }
    }
}