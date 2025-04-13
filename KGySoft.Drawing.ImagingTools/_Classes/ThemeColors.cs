#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ThemeColors.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.WinApi;

using Microsoft.Win32;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents the colors used by a theme. If no theme is used, then the default colors are used.
    /// </summary>
    public static class ThemeColors
    {
        #region Fields

        private static /*volatile*/ bool isDarkBaseTheme;
        private static /*volatile*/ bool isBaseThemeEverChanged;
        private static bool? isDarkSystemTheme;
        private static DefaultTheme currentBaseTheme;
        private static ThreadSafeDictionary<string, Color>? customColors;
        private static EventHandler? themeChangedHandler;

        #endregion

        #region Events

        public static event EventHandler? ThemeChanged
        {
            add => value.AddSafe(ref themeChangedHandler);
            remove => value.RemoveSafe(ref themeChangedHandler);
        }

        #endregion

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets whether theming is enabled. Theming is enabled if high contrast mode is disabled,
        /// and either at least one color is defined in the current theme or the current base theme is dark.
        /// </summary>
        public static bool IsThemingEnabled => !SystemInformation.HighContrast && (isDarkBaseTheme || customColors is { IsEmpty: false });

        public static Color Control { get => Get(SystemColors.Control); set => Set(value); }
        public static Color ControlText { get => Get(SystemColors.ControlText); set => Set(value); }

        #endregion

        #region Internal Properties

        internal static bool IsThemingEverChanged => isBaseThemeEverChanged;
        internal static bool IsDarkBaseTheme => isDarkBaseTheme;

        #endregion

        #region Private Properties

        internal static bool IsDarkSystemTheme
        {
            get
            {
                if (isDarkSystemTheme.HasValue)
                    return isDarkSystemTheme.Value;

                if (!OSUtils.IsWindows10OrLater)
                    return (isDarkSystemTheme = false).Value;

                const string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string keyAppsUseLightTheme = "AppsUseLightTheme";
                try
                {
                    using RegistryKey? reg = Registry.CurrentUser.OpenSubKey(path);
                    isDarkSystemTheme = reg?.GetValue(keyAppsUseLightTheme) is int value && Math.Abs(value) == 0;
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    isDarkSystemTheme = false;
                }

                return isDarkSystemTheme.Value;
            }
        }

        private static ThreadSafeDictionary<string, Color> CurrentTheme
        {
            get
            {
                ThreadSafeDictionary<string, Color>? result = customColors;
                if (result != null)
                    return result;

                // Note: currentTheme can be set by ResetTheme (even to null), so it can happen that between the previous null check and this one
                // the currentTheme is already set. In such cases, we return the already set value.
                result = new ThreadSafeDictionary<string, Color>();
                return Interlocked.CompareExchange(ref customColors, result, null) ?? result;
            }
        }

        #endregion

        #endregion

        #region Constructors

        static ThemeColors()
        {
            try
            {
                if (OSUtils.IsWindows)
                    SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            }
            catch (Exception e) when (e is InvalidOperationException or ExternalException)
            {
            }
        }

        #endregion

        #region Methods

        #region Static Methods

        public static void SetBaseTheme(DefaultTheme theme = DefaultTheme.Classic, bool resetCustomColors = true)
        {
            if (!theme.IsDefined())
                throw new ArgumentOutOfRangeException(nameof(theme), theme, PublicResources.EnumOutOfRange(theme));

            if (theme == DefaultTheme.Classic && !isBaseThemeEverChanged && (customColors is null || !resetCustomColors))
                return;

            if (!OSUtils.IsWindows10OrLater)
                return;

            bool isNewThemeDark = theme == DefaultTheme.Dark || theme == DefaultTheme.System && IsDarkSystemTheme;
            bool defaultColorsChanged = isNewThemeDark != isDarkBaseTheme;
            currentBaseTheme = theme;
            isBaseThemeEverChanged = defaultColorsChanged;
            isDarkBaseTheme = isNewThemeDark;

            // TODO: remove .NET 9
            //// NOTE: Not using the built-in dark mode support even if available, because it is not quite the same as ours.
            //// E.g.: TextBox borders, ToolStrip, etc
#if NET9_0_OR_GREATER
                if (defaultColorsChanged)
                    Application.SetColorMode((SystemColorMode)theme);
#else
            if (defaultColorsChanged)
                InitializeBaseTheme(theme);
#endif
            if (resetCustomColors)
                DoResetCustomColors(null, defaultColorsChanged);
            else if (defaultColorsChanged)
                OnThemeChanged(EventArgs.Empty);
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        public static void ResetCustomColors(IDictionary<string, Color>? theme = null) => DoResetCustomColors(theme, false);

        public static bool IsSet(string key) => customColors?.ContainsKey(key) == true;

        #endregion

        #region Private Methods

#if !NET9_0_OR_GREATER
        private static void InitializeBaseTheme(DefaultTheme theme)
        {
            // Context menus of the current process
            try
            {
                UxTheme.SetPreferredAppMode(theme);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return;
            }

            //// Title bars of all already existing windows - TODO
            //bool isDarkTheme = isDarkBaseTheme;
            //foreach (Form form in Application.OpenForms)
            //{
            //    try
            //    {
            //        //User32.SetControlText();
            //    }
            //    catch (Exception e) when (!e.IsCritical())
            //    {
            //        continue;
            //    }
            //}
        } 
#endif

        private static void DoResetCustomColors(IDictionary<string, Color>? theme, bool defaultColorsChanged)
        {
            if (theme is null || theme.Count == 0)
            {
                if (Interlocked.Exchange(ref customColors, null) != null || defaultColorsChanged)
                    OnThemeChanged(EventArgs.Empty);
                return;
            }

            Interlocked.Exchange(ref customColors, new ThreadSafeDictionary<string, Color>(theme));
            OnThemeChanged(EventArgs.Empty);
        }

        private static Color Get(Color defaultColor, [CallerMemberName] string key = null!)
            => customColors is { } theme ? theme.GetValueOrDefault(key, defaultColor) : defaultColor;

        private static void Set(Color color, [CallerMemberName] string key = null!)
        {
            bool isChanged = true;
            CurrentTheme.AddOrUpdate(key, color,
                (_, c) =>
                {
                    isChanged = c != color;
                    return color;
                });

            if (isChanged)
                OnThemeChanged(EventArgs.Empty);
        }

        private static void OnThemeChanged(EventArgs e) => themeChangedHandler?.Invoke(null, e);

        #endregion

        #region Event Handlers

        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            switch (e.Category)
            {
                case UserPreferenceCategory.Color or UserPreferenceCategory.VisualStyle:
                    OnThemeChanged(EventArgs.Empty);
                    break;

                case UserPreferenceCategory.General:
                    isDarkSystemTheme = null;
                    if (currentBaseTheme == DefaultTheme.System)
                        SetBaseTheme(DefaultTheme.System, false);
                    break;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
