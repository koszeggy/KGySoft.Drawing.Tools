#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Configuration.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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

#if NET35
using KGySoft.CoreLibraries; 
#endif
using KGySoft.Drawing.ImagingTools.Properties;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
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
        private const string fallbackResourceRepositoryLocation = "http://koszeggy.github.io/KGySoft.Drawing.Tools/res/";

        #endregion

        #region Fields

        private static readonly bool allowHttps;

        private static Uri? baseUri;

        #endregion

        #region Properties

        #region Internal Properties
        
        internal static bool AllowResXResources { get => GetFromSettings<bool>(); set => SetInSettings(value); }
        internal static bool UseOSLanguage { get => GetFromSettings<bool>(); set => SetInSettings(value); }
        internal static CultureInfo DisplayLanguage { get => GetFromSettings<CultureInfo>() ?? Res.DefaultLanguage; set => SetInSettings(value); }
        internal static Uri BaseUri => baseUri ??= new Uri(ResourceRepositoryLocation);

        #endregion

        #region Private Properties
        
        private static string ResourceRepositoryLocation => GetFromAppConfig()
            ?? (allowHttps ? defaultResourceRepositoryLocation : fallbackResourceRepositoryLocation);

        #endregion

        #endregion

        #region Constructors

        static Configuration()
        {
            allowHttps = !(OSUtils.IsMono && OSUtils.IsWindows);

            // To be able to resolve UserSettingsGroup of with other framework version
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
#if NET35
            // To prevent serializing CultureInfo by DisplayName instead of Name
            typeof(CultureInfo).RegisterTypeConverter<CultureInfoConverterFixed>();
#endif

#if NETFRAMEWORK
            if (!allowHttps)
                return;

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

        internal static void SaveSettings() => Settings.Default.Save();

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

        private static void SetInSettings(object value, [CallerMemberName]string propertyName = null!)
        {
            try
            {
                Settings.Default[propertyName] = value;
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
