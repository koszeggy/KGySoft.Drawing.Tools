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

    internal static class Res
    {
        #region Constants

        private const string unavailableResource = "Resource ID not found: {0}";
        private const string invalidResource = "Resource text is not valid for {0} arguments: {1}";

        #endregion

        #region Properties

        /// <summary>KGy SOFT Image DebuggerVisualizers</summary>
        internal static string TitleMessageDialog => Get(Ids.ResourceTitle);

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>Unexpected error occurred: {0}</summary>
        internal static string ErrorMessageUnexpectedError(string message) => Get(Resources.ErrorMessage_UnexpectedErrorFormat, message);

        /// <summary>Failed to install the visualizers to {0}: {1}
        ///
        /// Make sure every running debugger is closed. Installing will be tried again on restarting Visual Studio.</summary>
        internal static string ErrorMessageFailedToInstall(string targetPath, string message) => Get(Resources.ErrorMessage_FailedToInstallFormat, targetPath, message);

        /// <summary>The installation KGy SOFT Imaging Tools v{0} and the debugger visualizers finished with a warning: {1}.</summary>
        internal static string WarningMessageInstallationFinishedWithWarning(string targetPath, string message) => Get(Resources.WarningMessage_InstallationFinishedWithWarningFormat, targetPath, message);

        /// <summary>KGy SOFT Imaging Tools v{0} and the debugger visualizers have been installed to {1}.</summary>
        internal static string InfoMessageInstallationFinished(Version version, string targetPath) => Get(Resources.InfoMessage_InstallationFinishedFormat, version, targetPath);

        /// <summary>KGy SOFT Imaging Tools v{0} and the debugger visualizers have been upgraded to version {1} at {2}.</summary>
        internal static string InfoMessageUpgradeFinished(Version oldVersion, Version newVersion, string targetPath) => Get(Resources.InfoMessage_InstallationUpgradedFormat, oldVersion, newVersion, targetPath);

        #endregion

        #region Private Methods

        private static string Get(string id) => Resources.ResourceManager.GetString(id) ?? String.Format(CultureInfo.InvariantCulture, unavailableResource, id);

        private static string Get(string format, params object?[]? args) => args == null ? format : SafeFormat(format, args);

        private static string SafeFormat(string format, object?[] args)
        {
            try
            {
                int i = Array.IndexOf(args, null);
                if (i >= 0)
                {
                    string nullRef = PublicResources.Null;
                    for (; i < args.Length; i++)
                        args[i] ??= nullRef;
                }

                return String.Format(LanguageSettings.FormattingLanguage, format, args);
            }
            catch (FormatException)
            {
                return String.Format(CultureInfo.InvariantCulture, invalidResource, args.Length, format);
            }
        }

        #endregion

        #endregion
    }
}
