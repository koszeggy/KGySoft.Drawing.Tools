#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: InstallationInfo.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Policy;
using KGySoft.CoreLibraries;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal sealed class InstallationInfo : MarshalByRefObject
    {
        #region InitializerSandbox class

        private sealed class InitializerSandbox : MarshalByRefObject
        {
            #region Methods

            public InitializerSandbox(InstallationInfo info, string path)
            {
                try
                {
                    var asm = Assembly.ReflectionOnlyLoadFrom(InstallationManager.GetDebuggerVisualizerFilePath(path));
                    info.Version = asm.GetName().Version;
                    info.RuntimeVersion = asm.ImageRuntimeVersion;
                }
                catch (Exception)
                {
                    info.Version = null;
                    info.RuntimeVersion = null;
                }
            }

            #endregion
        }

        #endregion

        #region Properties

        internal string Path { get; }
        internal bool Installed { get; }
        internal Version Version { get; set; }
        internal string RuntimeVersion { get; set; }

        #endregion

        #region Constructors

        internal InstallationInfo(string path)
        {
            Path = path;
            Installed = InstallationManager.IsInstalled(path);

            if (!Installed)
                return;

            // DebuggerVisualizers is already loaded (executed from debugger)
            var debuggerVisualizerAssembly = Reflector.ResolveAssembly("KGySoft.Drawing.DebuggerVisualizers", false, true);
            if (debuggerVisualizerAssembly != null)
            {
                Version = debuggerVisualizerAssembly.GetName().Version;
                RuntimeVersion = null; // not relevant, would return the already loaded version
                return;
            }

            // not loaded: trying to determine the version by loading it into a sandbox domain
            try
            {
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
            }
            catch (Exception)
            {
                RuntimeVersion = null;
                InitializeInfoByFileVersion(path);
            }
        }

        private void InitializeInfoByFileVersion(string path)
        {
            try
            {
                Version = new Version(FileVersionInfo.GetVersionInfo(InstallationManager.GetDebuggerVisualizerFilePath(path)).FileVersion);
            }
            catch (Exception)
            {
                Version = null;
            }
        }

#endregion
    }
}
