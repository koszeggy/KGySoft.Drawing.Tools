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
using System.Threading;
using System.Windows.Forms;

using KGySoft.Collections;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Represents the colors used by a theme. If no theme is used, then the default colors are used.
    /// </summary>
    public static class ThemeColors
    {
        #region Fields

        private static ThreadSafeDictionary<string, Color>? currentTheme;

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
        /// Gets whether theming is enabled. Theming is enabled if at least one color is defined in the current theme.
        /// </summary>
        public static bool IsThemingEnabled => !SystemInformation.HighContrast && currentTheme is { IsEmpty: false };

        public static Color Control { get => Get(SystemColors.Control); set => Set(value); }
        public static Color ControlText { get => Get(SystemColors.ControlText); set => Set(value); }

        #endregion

        #region Private Properties

        private static ThreadSafeDictionary<string, Color> CurrentTheme
        {
            get
            {
                ThreadSafeDictionary<string, Color>? result = currentTheme;
                if (result != null)
                    return result;

                // Note: currentTheme can be set by ResetTheme (even to null), so it can happen that between the previous null check and this one
                // the currentTheme is already set. In such cases, we return the already set value.
                result = new ThreadSafeDictionary<string, Color>();
                return Interlocked.CompareExchange(ref currentTheme, result, null) ?? result;
            }
        }


        #endregion

        #endregion

        #region Methods

        #region Public Methods

        public static void ResetTheme(IDictionary<string, Color>? theme = null)
        {
            if (theme is null || theme.Count == 0)
            {
                if (Interlocked.Exchange(ref currentTheme, null) != null)
                    OnThemeChanged(EventArgs.Empty);
                return;
            }

            Interlocked.Exchange(ref currentTheme, new ThreadSafeDictionary<string, Color>(theme));
            OnThemeChanged(EventArgs.Empty);
        }

        public static bool IsSet(string key) => currentTheme?.ContainsKey(key) == true;

        #endregion

        #region Private Methods

        private static Color Get(Color defaultColor, [CallerMemberName]string key = null!)
            => currentTheme is { } theme ? theme.GetValueOrDefault(key, defaultColor) : defaultColor;

        private static void Set(Color color, [CallerMemberName]string key = null!)
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

        #endregion
    }
}
