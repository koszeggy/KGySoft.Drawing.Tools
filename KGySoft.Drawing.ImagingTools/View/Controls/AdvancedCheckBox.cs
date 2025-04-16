#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedCheckBox.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A CheckBox that
    /// - is scaled properly even with System FlatStyle (the same way as with Standard FlatStyle) - NOTE: would not be required with KGySoft.WinForms' AdvancedCheckBox, whose Standard FlatStyle works properly
    /// - fixes rendering in dark mode (see also ControlExtensions). Issue: with System FlatStyle enabled text is too dark, whereas with Standard FlatStyle the disabled text is too dark.
    /// </summary>
    internal class AdvancedCheckBox : CheckBox
    {
        #region Fields

        private FlatStyle lastFlatStyle = FlatStyle.Standard;
        private bool suppressBaseFlatStyleChange;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the flat style appearance of the button control.
        /// </summary>
        public new FlatStyle FlatStyle // it is also detected when base.FlatStyle changes but reacting onto that in OnPaint has a performance cost
        {
            get => base.FlatStyle;
            set
            {
                if (base.FlatStyle == value && lastFlatStyle == value && !suppressBaseFlatStyleChange)
                    return;

                suppressBaseFlatStyleChange = false; // when changed explicitly, we allow it in whatever state
                base.FlatStyle = value;
                lastFlatStyle = value;
                OnFlatStyleChanged();
            }
        }

        #endregion

        #region Methods

        #region Public Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            var flatStyle = base.FlatStyle;
            if (flatStyle != FlatStyle.System || !IsHandleCreated)
                return base.GetPreferredSize(proposedSize);

            // System flat style calculates the preferred size incorrectly.
            // This temporal change would not be needed with KGySoft.WinForms. This is also bad because this recreates the handle, so the theme should be applied again.
            SuspendLayout(); // preventing auto resize while changing style
            base.FlatStyle = FlatStyle.Standard;

            // The gap between the CheckBox and the text is 3px smaller with System at every DPI
            Size result = base.GetPreferredSize(proposedSize);
#if NET35
            // The scaling is different in .NET 3.5 so instead if subtracting a constant padding difference
            // we need to add some based on scaling, but only when visual styles are not applied
            if (!Application.RenderWithVisualStyles)
                result.Width += this.ScaleWidth(2);
#else
            result.Width -= 3;
#endif

            base.FlatStyle = FlatStyle.System;
            ResumeLayout();
            if (ThemeColors.IsDarkBaseTheme)
                this.ApplyTheme();
            return result;
        }

        #endregion

        #region Protected Methods

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!suppressBaseFlatStyleChange)
                CheckBaseFlatStyle();
            if (AdjustFlatStyleForTheme())
                return;

            base.OnPaint(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (base.FlatStyle != FlatStyle.System)
            {
                base.WndProc(ref m);
                return;
            }

            switch (m.Msg)
            {
                case Constants.WM_PAINT:
                    if (!suppressBaseFlatStyleChange)
                        CheckBaseFlatStyle();
                    if (AdjustFlatStyleForTheme())
                        return;

                    base.WndProc(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        #endregion

        #region Private Methods

        private void CheckBaseFlatStyle()
        {
            // FlatStyle is not an overridable property so unless suppressed, detecting its change here.
            if (base.FlatStyle != lastFlatStyle)
            {
                lastFlatStyle = base.FlatStyle;
                OnFlatStyleChanged();
            }
        }

        private bool AdjustFlatStyleForTheme()
        {
            // Only when using System FlatStyle (this could be removed when using AdvancedCheckBox from KGySoft.WinForms)
            if (lastFlatStyle != FlatStyle.System)
                return false;

            Debug.Assert(suppressBaseFlatStyleChange || lastFlatStyle == base.FlatStyle); // could not be asserted in a general library because anyone could change the base.FlatStyle

            if (!ThemeColors.IsDarkBaseTheme)
            {
                // theme has changed back to light mode
                if (suppressBaseFlatStyleChange)
                {
                    suppressBaseFlatStyleChange = false;
                    base.FlatStyle = lastFlatStyle;
                    OnFlatStyleChanged();
                    if (Parent is CheckGroupBox)
                        ForeColor = ThemeColors.GroupBoxText;
                    return true;
                }

                return false;
            }

            // Enabled with FlatStyle.System: too dark in dark mode, so changing the style to Standard and disabling auto adjustment by base
            if (base.FlatStyle == FlatStyle.System && Enabled)
            {
                suppressBaseFlatStyleChange = true;
                base.FlatStyle = FlatStyle.Standard;
                OnFlatStyleChanged();
                if (Parent is CheckGroupBox)
                    ForeColor = ThemeColors.GroupBoxText;
                return true;
            }

            // Disabled with FlatStyle.Standard: too dark in dark mode, so changing the style back to System
            if (suppressBaseFlatStyleChange && base.FlatStyle == FlatStyle.Standard && !Enabled)
            {
                suppressBaseFlatStyleChange = false;
                base.FlatStyle = lastFlatStyle;
                OnFlatStyleChanged();
                return true;
            }

            // No change was necessary: the paint operation may continue
            return false;
        }

        private void OnFlatStyleChanged()
        {
            Invalidate();
            if (AutoSize)
                PerformLayout();
            if (ThemeColors.IsDarkBaseTheme)
                this.ApplyTheme();
        }

        #endregion

        #endregion
    }
}