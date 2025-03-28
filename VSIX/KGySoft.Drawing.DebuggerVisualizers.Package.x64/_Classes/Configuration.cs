#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Configuration.cs
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
using System.Runtime.CompilerServices;

using KGySoft.Drawing.DebuggerVisualizers.Package.Properties;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class Configuration
    {
        #region Properties

        internal static string? LastVersion { get => GetFromSettings<string>(); set => SetInSettings(value); }

        #endregion

        #region Methods

        #region Internal Methods

        internal static void SaveConfig() => Settings.Default.Save();

        #endregion

        #region Private Methods

        private static T? GetFromSettings<T>([CallerMemberName]string propertyName = null!)
        {
            try
            {
                return (T)Settings.Default[propertyName];
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return default;
            }
        }

        private static void SetInSettings(object? value, [CallerMemberName]string propertyName = null!)
        {
            try
            {
                Settings.Default[propertyName] = value;
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        #endregion

        #endregion
    }
}