#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Res.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Globalization;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    #region Usings

    using Resources = Properties.Resources;

    #endregion

    /// <summary>
    /// Contains the x64 package specific resources. Unlike in the x86 version, we get the general string resources from
    /// Imaging Tools using the <see cref="DebuggerHelper"/> class. Only the package specific known resources are taken from this assembly.
    /// </summary>
    internal static class Res
    {
        #region Constants

        private const string unavailableResource = "Resource ID not found: {0}";

        #endregion

        #region Properties

        /// <summary>KGy SOFT Image DebuggerVisualizers</summary>
        internal static string TitleMessageDialog => Get(Ids.ResourceTitle);

        /// <summary>Change log</summary>
        internal static string InfoMessageChangeLog => DebuggerHelper.GetStringResource("InfoMessage_ChangeLog");

        /// <summary>Open Imaging Tools</summary>
        internal static string InfoMessageOpenImagingTools => DebuggerHelper.GetStringResource("InfoMessage_OpenImagingTools");

        /// <summary>Shell service could not be obtained. Installation status of the classic debugger visualizers cannot be checked.</summary>
        internal static string ErrorMessageShellServiceUnavailable => DebuggerHelper.GetStringResource("ErrorMessage_ShellServiceUnavailable");

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>KGy SOFT Imaging Tools v{0} and the debugger visualizers have been installed.</summary>
        internal static string InfoMessagePackageInstalled(Version version) => DebuggerHelper.GetStringResource("InfoMessage_PackageInstalledFormat", version);

        /// <summary>KGy SOFT Imaging Tools v{0} and the debugger visualizers have been upgraded to version v{1}.</summary>
        internal static string InfoMessagePackageUpgraded(Version lastVersion, Version currentVersion) => DebuggerHelper.GetStringResource("InfoMessage_PackageUpgradedFormat", lastVersion, currentVersion);

        /// <summary>Failed to uninstall the classic visualizers from {0}: {1}
        ///
        /// Make sure every running debugger is closed. Removal will be tried again on restarting Visual Studio.</summary>
        internal static string ErrorMessageFailedToUninstallClassic(string targetPath, string message) => DebuggerHelper.GetStringResource("ErrorMessage_FailedToUninstallClassicFormat", targetPath, message);

        /// <summary>Unexpected error occurred: {0}</summary>
        internal static string ErrorMessageUnexpectedError(string message) => DebuggerHelper.GetStringResource("ErrorMessage_UnexpectedErrorFormat", message);

        #endregion

        #region Private Methods

        private static string Get(string id) => Resources.ResourceManager.GetString(id) ?? String.Format(CultureInfo.InvariantCulture, unavailableResource, id);

        #endregion

        #endregion
    }
}
