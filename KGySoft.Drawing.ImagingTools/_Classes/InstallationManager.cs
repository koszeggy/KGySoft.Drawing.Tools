#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: InstallationManager.cs
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
using System.IO;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class InstallationManager
    {
        #region Constants

        private const string debuggerVisualizerFileName = "KGySoft.Drawing.DebuggerVisualizers.dll";

        #endregion

        #region Fields

        private static readonly string[] files =
        {
            "KGySoft.Drawing.ImagingTools.exe",
            "KGySoft.Drawing.dll",
            "KGySoft.CoreLibraries.dll",
            debuggerVisualizerFileName
        };

        #endregion

        #region Methods

        internal static InstallationInfo GetInstallationInfo(string path) => new InstallationInfo(path);

        internal static bool IsInstalled(string path) => Directory.Exists(path) && File.Exists(GetDebuggerVisualizerFilePath(path));

        internal static string GetDebuggerVisualizerFilePath(string path) => Path.Combine(path, debuggerVisualizerFileName);

        internal static void Install(string path, out string error)
        {
            error = null;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                error = $"Could not create directory {path}: {e.Message}";
                return;
            }

            string selfPath = Files.GetExecutingPath();
            if (selfPath == path)
            {
                error = "The current installation is being executed, which cannot be overwritten";
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    File.Copy(Path.Combine(selfPath, file), Path.Combine(path, file), true);
                }
                catch (Exception e)
                {
                    error = $"Could not copy file {file}: {e.Message}";
                    return;
                }
            }
        }

        internal static void Uninstall(string path, out string error)
        {
            error = null;
            if (!Directory.Exists(path))
                return;

            if (path == Files.GetExecutingPath())
            {
                error = "The current installation is being executed, which cannot be removed";
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    File.Delete(Path.Combine(path, file));
                }
                catch (Exception e)
                {
                    error = $"Could not delete file {file}: {e.Message}";
                    return;
                }
            }
        }

        #endregion
    }
}
