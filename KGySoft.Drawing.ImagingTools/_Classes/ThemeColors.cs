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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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

        private static readonly Color[] defaultThemeColors =
        [
            SystemColors.Control,
            SystemColors.ControlText,
            SystemColors.ControlDarkDark, // ControlTextDisabled
            SystemColors.Window,
            SystemColors.WindowText,
            SystemColors.GrayText, // WindowTextDisabled
            SystemColors.ControlLight, // WindowAlternate
            SystemColors.ControlText, // WindowTextAlternate
            SystemColors.ControlText, // GroupBoxText - NOTE: with visual styles enabled it's returned by VisualStyleRenderer
            SystemColors.WindowFrame, // GridLine
            SystemColors.AppWorkspace, // Workspace
        ];

        private static readonly Color[] darkThemeColors =
        [
            Color.FromArgb((unchecked((int)0xFF373737))), // Control
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // ControlText
            Color.FromArgb((unchecked((int)0xFFCCCCCC))), // ControlTextDisabled // e.g. disabled CheckBox with FlatStyle.System
            Color.FromArgb((unchecked((int)0xFF323232))), // Window
            Color.FromArgb((unchecked((int)0xFFF0F0F0))), // WindowText
            Color.FromArgb((unchecked((int)0xFF6D6D6D))), // WindowTextDisabled // e.g. disabled TextBox
            Color.FromArgb((unchecked((int)0xFF464646))), // WindowAlternate // e.g. in DataGridView
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // WindowTextAlternate // e.g. in DataGridView
            Color.FromArgb((unchecked((int)0xFFFFFFFF))), // GroupBoxText
            Color.FromArgb((unchecked((int)0xFF646464))), // GridLine - .NET 9: FF282828 (WindowFrame)
            Color.FromArgb((unchecked((int)0xFF3C3C3C))), // Workspace - .NET 9: FF464646 (ControlDark)
        ];

        private static /*volatile*/ bool isDarkBaseTheme;
        private static /*volatile*/ bool isBaseThemeEverChanged;
        private static /*volatile*/ bool isCustomThemeEverChanged;
        private static bool? isDarkSystemTheme;
        private static DefaultTheme currentBaseTheme;
        private static ThreadSafeDictionary<ThemeColor, Color>? customColors;
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
        public static bool IsThemingEnabled => (isDarkBaseTheme || customColors is { IsEmpty: false }) && !SystemInformation.HighContrast;

        public static Color Control => Get(ThemeColor.Control);
        public static Color ControlText => Get(ThemeColor.ControlText);
        public static Color ControlTextDisabled => Get(ThemeColor.ControlTextDisabled);
        public static Color Window => Get(ThemeColor.Window);
        public static Color WindowText => Get(ThemeColor.WindowText);
        public static Color WindowTextDisabled => Get(ThemeColor.WindowTextDisabled);
        public static Color WindowAlternate => Get(ThemeColor.WindowAlternate);
        public static Color WindowTextAlternate => Get(ThemeColor.WindowTextAlternate);
        public static Color GroupBoxText => Get(ThemeColor.GroupBoxText); // Special handling!
        public static Color GridLine => Get(ThemeColor.GridLine);
        public static Color Workspace => Get(ThemeColor.Workspace);

        #endregion

        #region Internal Properties

        // These are not public theme colors because they match the fix theme of a TextBox/ComboBox that cannot be changed. Used only in dark mode.
        internal static Color FixedSingleBorder => Color.FromArgb(unchecked((int)(0xFFC8C8C8)));
        internal static Color FixedSingleBorderInactive => Color.FromArgb(unchecked((int)(0xFF9B9B9B)));

        internal static bool IsBaseThemeEverChanged => isBaseThemeEverChanged;
        internal static bool IsThemeEverChanged => isBaseThemeEverChanged || isCustomThemeEverChanged;
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

        private static ThreadSafeDictionary<ThemeColor, Color> CurrentTheme
        {
            get
            {
                ThreadSafeDictionary<ThemeColor, Color>? result = customColors;
                if (result != null)
                    return result;

                // Note: currentTheme can be set by ResetTheme (even to null), so it can happen that between the previous null check and this one
                // the currentTheme is already set. In such cases, we return the already set value.
                result = new ThreadSafeDictionary<ThemeColor, Color>();
                return Interlocked.CompareExchange(ref customColors, result, null) ?? result;
            }
        }

        #endregion

        #endregion

        #region Constructors

#if !SYSTEM_THEMING
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
#endif

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

            bool isNewThemeDark = (theme == DefaultTheme.Dark || theme == DefaultTheme.System && IsDarkSystemTheme) && Application.RenderWithVisualStyles;
            bool defaultColorsChanged = isNewThemeDark != isDarkBaseTheme;
            currentBaseTheme = theme;

            // TODO: remove .NET 9
            // NOTE: Not using the built-in dark mode support even if available, because it is not quite the same as ours.
            // E.g.: TextBox borders, ToolStrip, etc
#if NET9_0_OR_GREATER && SYSTEM_THEMING
            if (defaultColorsChanged)
                Application.SetColorMode((SystemColorMode)theme);
#else
            isDarkBaseTheme = isNewThemeDark;
            isBaseThemeEverChanged |= defaultColorsChanged;
            if (defaultColorsChanged)
                InitializeBaseTheme(theme);
            if (resetCustomColors)
                DoResetCustomColors(null, defaultColorsChanged);
            else if (defaultColorsChanged)
                OnThemeChanged(EventArgs.Empty);
#endif
        }

        #endregion

        #region Instance Methods

        #region Public Methods

        public static void ResetCustomColors(IDictionary<ThemeColor, Color>? theme = null) => DoResetCustomColors(theme, false);

        public static bool IsSet(ThemeColor key) => customColors?.ContainsKey(key) == true;

        #endregion

        #region Private Methods

        private static void InitializeBaseTheme(DefaultTheme theme)
        {
            // Context menus of the current process
            try
            {
                UxTheme.SetPreferredAppMode(theme);
                UxTheme.FlushMenuThemes();
            }
            catch (Exception e) when (!e.IsCritical())
            {
            }
        } 

        private static void DoResetCustomColors(IDictionary<ThemeColor, Color>? theme, bool defaultColorsChanged)
        {
            if (theme is null || theme.Count == 0)
            {
                if (Interlocked.Exchange(ref customColors, null) != null || defaultColorsChanged)
                {
                    isCustomThemeEverChanged = true;
                    OnThemeChanged(EventArgs.Empty);
                }

                return;
            }

            isCustomThemeEverChanged = true;
            Interlocked.Exchange(ref customColors, new ThreadSafeDictionary<ThemeColor, Color>(theme));
            OnThemeChanged(EventArgs.Empty);
        }

        private static Color Get(ThemeColor key)
        {
            if (customColors?.TryGetValue(key, out Color result) == true)
                return result;

            Debug.Assert(key.IsDefined() && (int)key < defaultThemeColors.Length && (int)key < darkThemeColors.Length);

            // Special handling for GroupBoxText: it may be different when visual styles are enabled (e.g. Windows XP)
            if (key == ThemeColor.GroupBoxText && !isDarkBaseTheme && Application.RenderWithVisualStyles)
                return new VisualStyleRenderer(VisualStyleElement.Button.GroupBox.Normal).GetColor(ColorProperty.TextColor);

            return isDarkBaseTheme ? darkThemeColors[(int)key] : defaultThemeColors[(int)key];
        }

        // TODO: remove. WHen done, customColors can be a simple Dictionary because always replaced at once
        //private static void Set(ThemeColor key, Color color)
        //{
        //    bool isChanged = true;
        //    CurrentTheme.AddOrUpdate(key, color,
        //        (_, c) =>
        //        {
        //            isChanged = c != color;
        //            return color;
        //        });

        //    if (isChanged)
        //    {
        //        isCustomThemeEverChanged = true;
        //        OnThemeChanged(EventArgs.Empty);
        //    }
        //}

        private static void OnThemeChanged(EventArgs e) => themeChangedHandler?.Invoke(null, e);

        #endregion

        #region Event Handlers

#if !SYSTEM_THEMING
        private static void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            switch (e.Category)
            {
                //// TODO: is this needed?
                //case UserPreferenceCategory.Color or UserPreferenceCategory.VisualStyle:
                //    OnThemeChanged(EventArgs.Empty);
                //    break;

                case UserPreferenceCategory.General: // Light/dark change
                    isDarkSystemTheme = null;
                    if (currentBaseTheme == DefaultTheme.System)
                        SetBaseTheme(DefaultTheme.System, false);
                    break;
            }
        }
#endif

        #endregion

        #endregion

        #endregion
    }
}
