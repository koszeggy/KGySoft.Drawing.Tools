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
using System.Diagnostics;
using System.Runtime.CompilerServices;

using KGySoft.Drawing.DebuggerVisualizers.Package.Properties;

using Microsoft.Win32;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    /// <summary>
    /// NOTE: Originally configurations were stored in user-scoped application settings, but as its location keeps changing with every new
    /// Visual Studio version, now registry is used. The application settings are kept only for backward compatibility, when upgrading
    /// from an older version, or when the registry is not accessible from the possibly restricted AppDomain of a Visual Studio extension.
    /// </summary>
    internal static class Configuration
    {
        #region Constants

        private const string registryPath = @"Software\KGy SOFT\Imaging Tools Debugger Visualizers";

        #endregion

        #region Fields

        private static RegistryKey? registryKey;
        private static bool forceAppSettings;

        #endregion

        #region Properties
        
        #region Internal Properties

        internal static string? LastVersion { get => Get(); set => Set(value); }

        #endregion

        #region Private Properties

        private static RegistryKey? RegistryKey
        {
            get
            {
                if (registryKey is null)
                {
                    try
                    {
                        registryKey = Registry.CurrentUser.CreateSubKey(registryPath);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        forceAppSettings = true;
                    }
                }
                return registryKey;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal static void SaveConfig()
        {
            if (forceAppSettings)
                Settings.Default.Save();
            registryKey?.Flush();
            Release();
        }

        internal static void Release()
        {
            registryKey?.Close();
            registryKey = null;
            forceAppSettings = false;
        }

        #endregion

        #region Private Methods

        private static string? Get([CallerMemberName]string propertyName = null!)
        {
            string? result = null;
            if (!forceAppSettings)
                result = GetFromRegistry<string>(propertyName);
            if (result == null)
            {
                result = GetFromSettings<string>(propertyName);

                // If found in settings and registry is accessible, migrating to registry
                if (!forceAppSettings)
                    SetInRegistry(result, propertyName);
            }

            return result;
        }

        private static void Set(object? value, [CallerMemberName]string propertyName = null!)
        {
            if (!forceAppSettings)
                SetInRegistry(value, propertyName);
            if (forceAppSettings)
                SetInSettings(value, propertyName);
        }

        private static T? GetFromSettings<T>(string propertyName)
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

        private static void SetInSettings(object? value, string propertyName)
        {
            try
            {
                Settings.Default[propertyName] = value;
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        private static T? GetFromRegistry<T>(string propertyName)
            where T : class // if it can be a struct, change to TryGetFromRegistry like in ImagingTools
        {
            try
            {
                RegistryKey? key = RegistryKey;
                if (key is null)
                    return default;
                return (T?)key.GetValue(propertyName);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return default;
            }
        }

        private static void SetInRegistry(object? value, string propertyName)
        {
            Debug.Assert(value is string);

            try
            {
                RegistryKey? key = RegistryKey;
                key?.SetValue(propertyName, value ?? String.Empty, RegistryValueKind.String);
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        #endregion

        #endregion
    }
}