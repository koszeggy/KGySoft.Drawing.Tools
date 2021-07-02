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
#if !NET35
using System.Linq;
#endif
using System.Security;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
#if NET45
using KGySoft.Drawing.ImagingTools.WinApi;
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents a class that can manage debugger visualizer installations.
    /// </summary>
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

        private static InstallationInfo? availableVersion;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the available debugger visualizer version that can be installed with the <see cref="InstallationManager"/> class.
        /// If the debugger visualizer assembly is not deployed with this application, then the <see cref="InstallationInfo.Installed"/> property of the returned instance will be <see langword="false"/>.
        /// </summary>
        public static InstallationInfo AvailableVersion => availableVersion ??= new InstallationInfo(Files.GetExecutingPath());

        /// <summary>
        /// Gets version of the currently used <c>ImagingTools</c> application.
        /// </summary>
        public static Version ImagingToolsVersion { get; } = typeof(InstallationManager).Assembly.GetName().Version!;

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Gets the installation information of the debugger visualizer for the specified <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The directory for which the installation status is about to be retrieved.</param>
        /// <returns>An <see cref="InstallationInfo"/> instance that provides information about the debugger visualizer installation for the specified <paramref name="directory"/>.</returns>
        public static InstallationInfo GetInstallationInfo(string directory) => new InstallationInfo(directory);

        /// <summary>
        /// Installs the debugger visualizers into the specified <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The directory where the debugger visualizers have to be installed.</param>
        /// <param name="error">If the installation fails, then this parameter returns the error message; otherwise, this parameter returns <see langword="null"/>.</param>
        /// <param name="warning">If the installation succeeds with warnings, then this parameter returns the warning message; otherwise, this parameter returns <see langword="null"/>.</param>
        [SecuritySafeCritical]
        public static void Install(string directory, out string? error, out string? warning)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory), PublicResources.ArgumentNull);
            if (directory.Length == 0)
                throw new ArgumentException(PublicResources.ArgumentEmpty, nameof(directory));
            try
            {
                directory = Path.GetFullPath(directory);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                throw new ArgumentException(PublicResources.ArgumentInvalidString, nameof(directory), e);
            }

            error = null;
            warning = null;
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                error = Res.ErrorMessageCouldNotCreateDirectory(directory, e.Message);
                return;
            }

            string selfPath = Files.GetExecutingPath();
            if (selfPath == directory)
            {
                error = Res.ErrorMessageInstallationCannotBeOverwritten;
                return;
            }

            Uninstall(directory, out error);
            if (error != null)
            {
                // VS issue workaround, see the comment in Uninstall
                if (error == Res.ErrorMessageInstallationCannotBeRemoved)
                    error = Res.ErrorMessageInstallationCannotBeOverwritten;
                return;
            }

            foreach (string file in files)
            {
                try
                {
                    File.Copy(Path.Combine(selfPath, file), Path.Combine(directory, file), true);
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    error = Res.ErrorMessageCouldNotCopyFile(file, e.Message);
                    return;
                }
            }

#if NET45
            // .NET Core support: the visualizer must be in a netstandard2.0 subdirectory.
            // And actually it can contain framework assemblies so we just create a symbolic link to it
            // NOTE: It must be the .NET 4.5 build, others do not work (even a Core build itself, even in netcoreapp folder)
            string netCorePath = Path.Combine(directory, netCoreSubdirectory);
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
                char drive = Char.ToUpperInvariant(Path.GetFullPath(directory)[0]);
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
                    string source = Path.Combine(directory, file);
                    string target = Path.Combine(netCorePath, file);
                    if (isNtfs && OSUtils.IsWindows)
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
#endif
        }

        /// <summary>
        /// Removes the debugger visualizers from the specified <paramref name="directory"/>.
        /// </summary>
        /// <param name="directory">The directory where the debugger visualizers have to be removed from.</param>
        /// <param name="error">If the removal fails, then this parameter returns the error message; otherwise, this parameter returns <see langword="null"/>.</param>
        public static void Uninstall(string directory, out string? error)
        {
            error = null;
            if (!Directory.Exists(directory))
                return;

            string executingPath = Files.GetExecutingPath();
            if (directory == executingPath)
            {
                error = Res.ErrorMessageInstallationCannotBeRemoved;
                return;
            }

            string netCorePath = Path.Combine(directory, netCoreSubdirectory);
            bool netCoreDirExists = Directory.Exists(netCorePath);
            foreach (string file in files)
            {
                try
                {
                    File.Delete(Path.Combine(directory, file));
                    if (netCoreDirExists)
                        File.Delete(Path.Combine(netCorePath, file));
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    // VS issue workaround: if the package is installed and we try to remove the executed version during debugging, then the executing path will be
                    // the package installation path in uppercase, instead of the target directory (the path is the directory if the package is not installed though).
                    // However, the same path is in lowercase if we start the removing from the VS Tools menu instead of debugging...
                    if (executingPath == executingPath.ToUpperInvariant() && e is UnauthorizedAccessException)
                        error = Res.ErrorMessageInstallationCannotBeRemoved;
                    else
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
