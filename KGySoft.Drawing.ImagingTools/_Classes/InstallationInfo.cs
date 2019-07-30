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

            public void InitializeInfo(InstallationInfo info, string path)
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
                RuntimeVersion = debuggerVisualizerAssembly.ImageRuntimeVersion;
                return;
            }

            // not loaded: trying to determine the version by loading it into a sandbox domain
            try
            {
                Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
                AppDomain sandboxDomain = AppDomain.CreateDomain(nameof(InitializerSandbox), evidence, AppDomain.CurrentDomain.BaseDirectory, null, false);
                var initializer = (InitializerSandbox)sandboxDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(InitializerSandbox).FullName);
                try
                {
                    initializer.InitializeInfo(this, path);
                }
                finally
                {
                    AppDomain.Unload(sandboxDomain);
                }
            }
            catch (Exception)
            {
                Version = null;
                RuntimeVersion = null;
            }
        }

        #endregion
    }
}
