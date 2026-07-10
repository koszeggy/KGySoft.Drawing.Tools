#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BaseForm.cs
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

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal class BaseForm : KGySoft.WinForms.Forms.BaseForm
    {
        #region Constructors

        protected BaseForm()
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

            this.ApplyTheme();
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
            if (!IsHandleCreated)
                return;

            InvokeOnUIThread(this.ApplyTheme);
        }

        #endregion

        #endregion
    }
}
