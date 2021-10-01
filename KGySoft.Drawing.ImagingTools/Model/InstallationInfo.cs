#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: InstallationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
#if NETFRAMEWORK
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Reflection;
#if !NET35
using System.Runtime.Versioning; 
#endif
#if NETCOREAPP
using System.Runtime.Loader;
#endif
#if NETFRAMEWORK
using System.Security.Policy; 
#endif

#if NETFRAMEWORK
using KGySoft.CoreLibraries; 
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Provides information about the installation status of the debugger visualizers in a directory.
    /// </summary>
    public sealed class InstallationInfo : MarshalByRefObject
    {
        #region InitializerSandbox class

#if NETFRAMEWORK
        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "False alarm, instantiated by the InstallationInfo constructor in a separated AppDmain")]
        private sealed class InitializerSandbox : MarshalByRefObject
        {
            #region Constructors

            public InitializerSandbox(InstallationInfo info, string path)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(InstallationManager.GetDebuggerVisualizerFilePath(path));
                    info.Version = asm.GetName().Version;
                    info.RuntimeVersion = asm.ImageRuntimeVersion;
#if !NET35
                    info.TargetFramework = (Attribute.GetCustomAttribute(asm, typeof(TargetFrameworkAttribute)) as TargetFrameworkAttribute)?.FrameworkName; 
#endif
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    info.Version = null;
                    info.RuntimeVersion = null;
                }
            }

            #endregion
        }
#endif

        #endregion

        #region SandboxContext class

#if NETCOREAPP
        private sealed class SandboxContext : AssemblyLoadContext
        {

            #region Field

            private readonly AssemblyDependencyResolver resolver;

            #endregion

            #region Constructors

            internal SandboxContext(string path) : base(nameof(SandboxContext), isCollectible: true)
            {
                resolver = new AssemblyDependencyResolver(path);
            }

            #endregion

            #region Methods

            protected override Assembly? Load(AssemblyName name)
            {
                // ensuring that dependencies of the main assembly are also loaded into this context
                string? assemblyPath = resolver.ResolveAssemblyToPath(name);
                if (assemblyPath == null)
                    return null;

                using FileStream fs = File.OpenRead(assemblyPath);
                return LoadFromStream(fs);
            }

            #endregion
        }
#endif

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path of the installation.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets whether a debugger visualizer assembly exists in the directory specified by the <see cref="Path"/> property.
        /// </summary>
        public bool Installed { get; }

        /// <summary>
        /// Gets the version of an identified debugger visualizer installation.
        /// Can return <see langword="null"/>&#160;even if <see cref="Installed"/> is <see langword="true"/>, if the installed version could not be determined.
        /// </summary>
        public Version? Version { get; private set; }

        /// <summary>
        /// Gets the runtime version of an identified debugger visualizer installation.
        /// Can return <see langword="null"/>&#160;even if <see cref="Installed"/> is <see langword="true"/>, if the runtime version could not be determined.
        /// </summary>
        public string? RuntimeVersion { get; private set; }

        /// <summary>
        /// Gets the target framework of an identified debugger visualizer installation.
        /// Can return <see langword="null"/>&#160;even if <see cref="Installed"/> is <see langword="true"/>, if the assembly does no contain target framework information
        /// (typically .NET 3.5 version).
        /// </summary>
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string? TargetFramework
        {
            get;
#if !NET35
            private set; 
#endif
        }

        #endregion

        #region Constructors

        internal InstallationInfo(string path)
        {
            Path = path;
            Installed = File.Exists(InstallationManager.GetDebuggerVisualizerFilePath(path));

            if (!Installed)
                return;

            // Trying to determine the version by loading it into a sandbox domain. 
            try
            {
#if NETFRAMEWORK
                Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
                AppDomain sandboxDomain = AppDomain.CreateDomain(nameof(InitializerSandbox), evidence, Files.GetExecutingPath(), null, false);
                try
                {
                    // initializing by the constructor rather than unwrapping and calling a public method because unwrap may fail if executed from a Visual Studio package
#if NET35
                    Activator.CreateInstance(sandboxDomain, Assembly.GetExecutingAssembly().FullName, typeof(InitializerSandbox).FullName, false, BindingFlags.Public | BindingFlags.Instance, null, new object[] { this, path }, null, null, evidence);
#else
                    Activator.CreateInstance(sandboxDomain, Assembly.GetExecutingAssembly().FullName, typeof(InitializerSandbox).FullName, false, BindingFlags.Public | BindingFlags.Instance, null, new object[] { this, path }, null, null);
#endif
                }
                finally
                {
                    AppDomain.Unload(sandboxDomain);
                }
#else
                var context = new SandboxContext(path);
                try
                {
                    // note: we use LoadFromStream instead of LoadFromAssemblyPath because that keeps the file locked even after unloading the context
                    using var fs = File.OpenRead(InstallationManager.GetDebuggerVisualizerFilePath(path));
                    Assembly asm = context.LoadFromStream(fs);
                    Version = asm.GetName().Version;
                    RuntimeVersion = asm.ImageRuntimeVersion;
                    TargetFramework = (Attribute.GetCustomAttribute(asm, typeof(TargetFrameworkAttribute)) as TargetFrameworkAttribute)?.FrameworkName;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    Version = null;
                    RuntimeVersion = null;
                }
                finally
                {
                    context.Unload();
                }
#endif
            }
            catch (Exception e) when (!e.IsCritical())
            {
                RuntimeVersion = null;
                InitializeInfoByFileVersion(path);
            }
        }

        private void InitializeInfoByFileVersion(string path)
        {
            try
            {
                string? fileVersion = FileVersionInfo.GetVersionInfo(InstallationManager.GetDebuggerVisualizerFilePath(path)).FileVersion;
                Version = fileVersion == null ? null : new Version(fileVersion);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Version = null;
            }
        }

        #endregion
    }
}
