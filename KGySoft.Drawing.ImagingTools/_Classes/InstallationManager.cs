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
using System.Linq;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    public static class InstallationManager
    {
        #region Constants

        private const string debuggerVisualizerFileName = "KGySoft.Drawing.DebuggerVisualizers.dll";
        private const string netCoreSubdirectory = "netstandard2.0";

        #endregion

        #region Fields

        private static readonly string[] files =
        {
            "KGySoft.Drawing.ImagingTools.exe",
            "KGySoft.Drawing.dll",
            "KGySoft.CoreLibraries.dll",
            debuggerVisualizerFileName
        };

        private static InstallationInfo availableVersion;

        #endregion

        #region Methods

        #region Public Methods

        public static InstallationInfo AvailableVersion => availableVersion ??= new InstallationInfo(Files.GetExecutingPath());

        public static InstallationInfo GetInstallationInfo(string path) => new InstallationInfo(path);

        public static void Install(string path, out string error, out string warning)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path), PublicResources.ArgumentNull);
            if (path.Length == 0)
                throw new ArgumentException(PublicResources.ArgumentEmpty, nameof(path));
            try
            {
                path = Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                throw new ArgumentException(PublicResources.ValueContainsIllegalPathCharacters(path), nameof(path), e);
            }

            error = null;
            warning = null;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                error = Res.ErrorMessageCouldNotCreateDirectory(path, e.Message);
                return;
            }

            string selfPath = Files.GetExecutingPath();
            if (selfPath == path)
            {
                error = Res.ErrorMessageInstallationCannotBeOverwritten;
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    File.Copy(Path.Combine(selfPath, file), Path.Combine(path, file), true);
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    error = Res.ErrorMessageCouldNotCopyFile(file, e.Message);
                    return;
                }
            }

            // .NET Core 3.0 support: the visualizer must be in a netstandard2.0 subdirectory.
            // And actually it can contain framework assemblies so we just create a symbolic link to it
            string netCorePath = Path.Combine(path, netCoreSubdirectory);
            try
            {
                if (!Directory.Exists(netCorePath))
                    Directory.CreateDirectory(netCorePath);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                warning = Res.WarningMessageCouldNotCreateNetCoreDirectory(netCorePath, e.Message);
                return;
            }

            bool isNtfs;
            try
            {
                char drive = Char.ToUpperInvariant(Path.GetFullPath(path)[0]);
                isNtfs = drive >= 'A' && drive <= 'Z' && new DriveInfo(drive.ToString(null)).DriveFormat == "NTFS";
            }
            catch (Exception e) when (!e.IsCritical())
            {
                isNtfs = false;
            }

            foreach (string file in files)
            {
                try
                {
                    string source = Path.Combine(path, file);
                    string target = Path.Combine(netCorePath, file);
                    if (isNtfs)
                    {
                        if (File.Exists(target))
                            File.Delete(target);
                        Kernel32.CreateHardLink(target, source);
                    }
                    else
                        File.Copy(source, target, true);
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    warning = isNtfs
                        ? Res.WarningMessageCouldNotCreateNetCoreLink(file, e.Message)
                        : Res.WarningMessageCouldNotCopyFileNetCore(file, e.Message);
                    return;
                }
            }
        }

        public static void Uninstall(string path, out string error)
        {
            error = null;
            if (!Directory.Exists(path))
                return;

            if (path == Files.GetExecutingPath())
            {
                error = Res.ErrorMessageInstallationCannotBeRemoved;
                return;
            }

            string netCorePath = Path.Combine(path, netCoreSubdirectory);
            bool netCoreDirExists = Directory.Exists(netCorePath);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(Path.Combine(path, file));
                    if (netCoreDirExists)
                        File.Delete(Path.Combine(netCorePath, file));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    error = Res.ErrorMessageCouldNotDeleteFile(file, e.Message);
                    return;
                }
            }

            try
            {
                if (netCoreDirExists &&
#if NET35
                    Directory.GetFileSystemEntries(netCorePath).Length == 0
#else
                    !Directory.EnumerateFileSystemEntries(netCorePath).Any()
#endif
                    )
                {
                    Directory.Delete(netCorePath);
                }
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        #endregion

        #region Internal Methods

        internal static string GetDebuggerVisualizerFilePath(string path) => Path.Combine(path, debuggerVisualizerFileName);

        #endregion

        #endregion
    }
}
