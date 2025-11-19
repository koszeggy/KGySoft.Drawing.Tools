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
#if NET35
using System.ComponentModel; 
#endif
using System.Configuration;
using System.Globalization;
#if NETFRAMEWORK
using System.Net; 
#endif
using System.Reflection; 
using System.Runtime.CompilerServices;

using KGySoft.CoreLibraries; 
using KGySoft.Drawing.ImagingTools.Properties;

using Microsoft.Win32;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// NOTE: Originally read-write configurations were stored in user-scoped application settings, but when using Imaging Tools as a VS extension, this a problem,
    /// because in such case application settings location keeps changing with every new Visual Studio version.
    /// Because of this, now the Registry is used for read-write configurations. Fortunately this works even on Linux with Mono, as Mono implements the registry using files.
    /// The application settings are kept only for backward compatibility, when upgrading from an older version,
    /// or when the registry is not accessible from the possibly restricted AppDomain of a Visual Studio extension.
    /// Application-scoped configuration (app.config) is kept for read-only settings that are alright to be used from Imaging Tools as a standalone application.
    /// Make sure to call <see cref="Release"/> after accessing registry-based settings even when not saving any changes to release the registry key.
    /// </summary>
    internal static class Configuration
    {
        #region Nested classes

#if NET35
        /// <summary>
        /// This class is needed for .NET 3.5, which emits display name of a language instead of the <see cref="CultureInfo.Name"/> property.
        /// </summary>
        private sealed class CultureInfoConverterFixed : CultureInfoConverter
        {
            #region Methods

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
            {
                if (value == null)
                    return String.Empty;
                if (value is not CultureInfo ci)
                    throw new ArgumentException(PublicResources.NotAnInstanceOfType(typeof(CultureInfo)));

                return destinationType == typeof(string) ? ci.Name : base.ConvertTo(context, culture, value, destinationType);
            }

            #endregion
        }
#endif

        #endregion

        #region Constants

        private const string defaultResourceRepositoryLocation = "https://koszeggy.github.io/KGySoft.Drawing.Tools/res/"; // same as "https://raw.githubusercontent.com/koszeggy/KGySoft.Drawing.Tools/pages/res/"
        private const string fallbackResourceRepositoryLocation = "http://kgysoft.net/res/"; // "http://koszeggy.github.io/KGySoft.Drawing.Tools/res/" does not work on Win7/.NET 3.5
        private const string registryPath = @"Software\KGy SOFT\Imaging Tools";

        #endregion

        #region Fields

        private static readonly bool allowHttps;

        private static Uri? baseUri;
        private static RegistryKey? registryKey;
        private static bool forceAppSettings;

        #endregion

        #region Properties

        #region Internal Properties

        internal static bool UseOSLanguage { get => Get<bool>(); set => Set(value); }
        internal static CultureInfo DisplayLanguage { get => Get<CultureInfo>() ?? Res.DefaultLanguage; set => Set(value); }
        internal static string? ResXResourcesCustomPath { get => Get<string?>(); set => Set(value); }
        internal static Uri BaseUri => baseUri ??= new Uri(ResourceRepositoryLocation);

        #endregion

        #region Private Properties
        
        private static string ResourceRepositoryLocation => GetFromAppConfig()
            ?? (allowHttps ? defaultResourceRepositoryLocation : fallbackResourceRepositoryLocation);

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

        #region Constructors

        static Configuration()
        {
            allowHttps = !(OSUtils.IsMono && OSUtils.IsWindows);
#if NET35
            allowHttps &= OSUtils.IsWindows8OrLater;
#endif

            // To be able to resolve UserSettingsGroup with other framework version
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
#if NET35
            // To prevent serializing CultureInfo by DisplayName instead of Name
            typeof(CultureInfo).RegisterTypeConverter<CultureInfoConverterFixed>();
#endif

#if NETFRAMEWORK
            try
            {
                // To be able to use HTTP requests with TLS 1.2 security protocol (may not work on Windows XP)
                ServicePointManager.SecurityProtocol |=
#if NET35 || NET40
                    (SecurityProtocolType)3072;
#else
                    SecurityProtocolType.Tls12;
#endif
            }
            catch (NotSupportedException)
            {
                allowHttps = false;
            }
#endif
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal static void SaveSettings()
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

        private static T? Get<T>([CallerMemberName]string propertyName = null!)
        {
            T? result = default;
            if (!forceAppSettings && TryGetFromRegistry(propertyName, out result))
                return result;

            if (Equals(result, default(T)))
            {
                result = GetFromSettings<T>(propertyName);

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

        private static bool TryGetFromRegistry<T>(string propertyName, out T? value)
        {
            try
            {
                object? result = RegistryKey?.GetValue(propertyName);
                if (result is null)
                {
                    value = default;
                    return false;
                }

                if (result is T t || result.TryConvert(out t!))
                {
                    value = t;
                    return true;
                }

                value = default;
                return false;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                value = default;
                return false;
            }
        }

        private static void SetInRegistry(object? value, string propertyName)
        {
            value = value switch
            {
                bool boolValue => boolValue ? 1 : 0,
                null => String.Empty,
                _ => value
            };

            try
            {
                RegistryKey? key = RegistryKey;
                key?.SetValue(propertyName, value);
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        }

        private static string? GetFromAppConfig([CallerMemberName]string propertyName = null!)
        {
            try
            {
                return ConfigurationManager.AppSettings[propertyName];
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return null;
            }
        }

        #endregion

        #region Event handlers

        private static Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
#if NETFRAMEWORK
            if (args.Name.StartsWith("System, Version=", StringComparison.Ordinal))
                return typeof(UserSettingsGroup).Assembly;
#elif NETCOREAPP
            if (args.Name.StartsWith("System.Configuration.ConfigurationManager, Version=", StringComparison.Ordinal))
                return typeof(UserSettingsGroup).Assembly;
#endif
            return null;
        } 

        #endregion

        #endregion
    }
}
