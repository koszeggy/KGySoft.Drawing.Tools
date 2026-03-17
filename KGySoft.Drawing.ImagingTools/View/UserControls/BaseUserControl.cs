#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BaseUserControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using KGySoft.Drawing.ImagingTools.View.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal class BaseUserControl : KGySoft.WinForms.Controls.BaseUserControl
    {
        #region Fields

        private bool themeApplied;

        #endregion

        #region Constructors

        protected BaseUserControl()
        {
            ThemeColors.ThemeChanged += ThemeColors_ThemeChanged;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (IsDesignMode)
                return;

            if (ThemeColors.IsThemeEverChanged)
                ApplyTheme();
        }

        protected virtual void ApplyTheme()
        {
            if (themeApplied)
                return;

            // Applying the theme to the child controls only if the parent is not a BaseForm, because BaseForm would apply it automatically
            if (ParentForm is not BaseForm)
            {
                // not in a form at all: assuming an WPF WinFormsHost, and setting the Fore/BackColors explicitly
                // (this will overwrite the possibly Transparent BackColor, which is alright because there will be visible no resize grip anyway)
                if (ParentForm is null && Visible && Created)
                {
                    BackColor = ThemeColors.Control;
                    ForeColor = ThemeColors.ControlText;
                }

                ControlExtensions.ApplyTheme(this);
            }

            themeApplied = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ThemeColors.ThemeChanged -= ThemeColors_ThemeChanged;
        }

        #endregion

        #region Event Handlers

        private void ThemeColors_ThemeChanged(object? sender, EventArgs e)
        {
            themeApplied = false;
            if (!IsHandleCreated)
                return;

            InvokeOnUIThread(ApplyTheme);
        }

        #endregion

        #endregion
    }
}