#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridView.cs
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using KGySoft.ComponentModel;
#if NETFRAMEWORK
using KGySoft.CoreLibraries;
#endif
using KGySoft.Drawing.ImagingTools.View.Components;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - Columns items are never null
#pragma warning disable CS8602 // Dereference of a possibly null reference. - Columns items are never null
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just a DataGridView that
    /// - provides some default styles/colors with a few fixed issues
    /// - scales the columns automatically
    /// - provides scaling error/warning/info icons, which appear also on Linux/Mono
    /// - supports dark mode
    /// </summary>
    internal class AdvancedDataGridView : DataGridView
    {
        #region Fields

        #region Static Fields

        private static FieldAccessor? toolTipControlField;
        private static FieldAccessor? toolTipControlToolTipField;

        private static bool toolAccessorsInitialized;

        #endregion

        #region Instance Fields

        private readonly DataGridViewCellStyle defaultDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultHeadersDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultAlternatingRowsDefaultCellStyle;

        private bool isCustomDefaultCellStyle;
        private bool isCustomColumnHeadersDefaultCellStyle;
        private bool isCustomRowHeadersDefaultCellStyle;
        private bool isCustomAlternatingRowsDefaultCellStyle;
        private bool isCustomGridColor;
        private bool isCustomBackgroundColor;
        private BorderStyle borderStyle = BorderStyle.FixedSingle;

        private bool isRightToLeft;
        private Bitmap? errorIcon;
        private Bitmap? warningIcon;
        private Bitmap? infoIcon;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        // these are reintroduced just for the ShouldSerialize... methods
        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle DefaultCellStyle
        {
            get => base.DefaultCellStyle;
            set
            {
                isCustomDefaultCellStyle = value != null;
                base.DefaultCellStyle = isCustomDefaultCellStyle ? value : defaultDefaultCellStyle;
            }
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle ColumnHeadersDefaultCellStyle
        {
            get => base.ColumnHeadersDefaultCellStyle;
            set
            {
                isCustomColumnHeadersDefaultCellStyle = value != null;
                base.ColumnHeadersDefaultCellStyle = isCustomColumnHeadersDefaultCellStyle ? value : defaultHeadersDefaultCellStyle;
            }
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle RowHeadersDefaultCellStyle
        {
            get => base.RowHeadersDefaultCellStyle;
            set
            {
                isCustomRowHeadersDefaultCellStyle = value != null;
                base.RowHeadersDefaultCellStyle = isCustomRowHeadersDefaultCellStyle ? value : defaultHeadersDefaultCellStyle;
            }
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle AlternatingRowsDefaultCellStyle
        {
            get => base.AlternatingRowsDefaultCellStyle;
            set
            {
                isCustomAlternatingRowsDefaultCellStyle = value != null;
                base.AlternatingRowsDefaultCellStyle = isCustomAlternatingRowsDefaultCellStyle ? value
                    : SystemInformation.HighContrast ? null
                    : defaultAlternatingRowsDefaultCellStyle;
            }
        }

        public new Color GridColor
        {
            get => base.GridColor;
            set
            {
                isCustomGridColor = !value.IsEmpty;
                base.GridColor = isCustomGridColor ? value : ThemeColors.GridLine;
            }
        }

        public new Color BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                isCustomBackgroundColor = !value.IsEmpty;
                base.BackgroundColor = isCustomBackgroundColor ? value : ThemeColors.Workspace;
            }
        }

        [DefaultValue(DataGridViewHeaderBorderStyle.Single)]
        public new DataGridViewHeaderBorderStyle ColumnHeadersBorderStyle
        {
            get => base.ColumnHeadersBorderStyle;
            set => base.ColumnHeadersBorderStyle = value;
        }

        [DefaultValue(DataGridViewHeaderBorderStyle.Single)]
        public new DataGridViewHeaderBorderStyle RowHeadersBorderStyle
        {
            get => base.RowHeadersBorderStyle;
            set => base.RowHeadersBorderStyle = value;
        }

        [DefaultValue(BorderStyle.FixedSingle)]
        public new BorderStyle BorderStyle
        {
            get => borderStyle;
            set
            {
                // In dark mode just overriding the default border painting is not enough, because it clashes with the default black border painting.
                // So setting the base border style to None, and doing our own NC painting completely separately.
                borderStyle = value;
                base.BorderStyle = ThemeColors.IsDarkBaseTheme && value == BorderStyle.FixedSingle ? BorderStyle.None : value;
                InvalidateNC();
            }
        }

        #endregion

        #region Private Properties

        private Bitmap ErrorIcon => errorIcon ??= Icons.SystemError.ToScaledBitmap(this.GetScale());
        private Bitmap WarningIcon => warningIcon ??= Icons.SystemWarning.ToScaledBitmap(this.GetScale());
        private Bitmap InfoIcon => infoIcon ??= Icons.SystemInformation.ToScaledBitmap(this.GetScale());

        private AdvancedToolTip? ToolTip
        {
            get
            {
                // Mono has a ToolTip tooltip_window field
                if (OSUtils.IsMono)
                {
                    if (!toolAccessorsInitialized)
                    {
                        try
                        {
                            FieldInfo? field = typeof(DataGridView).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("tooltip_window", StringComparison.Ordinal));
                            if (field is not null)
                            {
                                toolTipControlField = FieldAccessor.GetAccessor(field);
                                toolTipControlField.Set(this, CreateAdvancedToolTip());
                            }
                        }
                        catch (Exception e) when (!e.IsCritical())
                        {
                            toolTipControlField = null;
                        }
                        finally
                        {
                            toolAccessorsInitialized = true;
                        }
                    }

                    return toolTipControlField?.GetInstanceValue<DataGridView, ToolTip>(this) as AdvancedToolTip;
                }

                // Assuming Windows implementation here: DataGridView.*toolTipControl*.*toolTip*
                if (!toolAccessorsInitialized)
                {
                    try
                    {
                        FieldInfo? field = typeof(DataGridView).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("toolTipControl", StringComparison.Ordinal));
                        if (field is not null)
                        {
                            toolTipControlField = FieldAccessor.GetAccessor(field);
                            var toolTipControl = toolTipControlField.Get(this);
                            if (toolTipControl != null)
                            {
                                FieldInfo? toolTipField = toolTipControl.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("toolTip", StringComparison.OrdinalIgnoreCase));
                                if (toolTipField is not null)
                                {
                                    toolTipControlToolTipField = FieldAccessor.GetAccessor(toolTipField);
                                    toolTipControlToolTipField.Set(toolTipControl, CreateAdvancedToolTip());
                                }
                                else
                                    toolTipControlField = null;
                            }
                        }
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        toolTipControlField = null;
                        toolTipControlToolTipField = null;
                    }
                    finally
                    {
                        toolAccessorsInitialized = true;
                    }
                }

                return toolTipControlToolTipField?.Get(toolTipControlField?.Get(this)) as AdvancedToolTip;
            }
        }

        #endregion

        #endregion

        #region Constructors

        public AdvancedDataGridView()
        {
            base.DefaultCellStyle = defaultDefaultCellStyle = new DataGridViewCellStyle(DefaultCellStyle)
            {
                // Base default uses Window back color with ControlText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                BackColor = ThemeColors.Window,
                ForeColor = ThemeColors.WindowText,
                SelectionBackColor = ThemeColors.Highlight,
                SelectionForeColor = ThemeColors.HighlightText,
            };

            base.ColumnHeadersDefaultCellStyle = base.RowHeadersDefaultCellStyle = defaultHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                // Base default uses Control back color with WindowText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                // Effective only with disabled visual styles.
                BackColor = ThemeColors.Control,
                ForeColor = ThemeColors.ControlText,
            };

            base.AlternatingRowsDefaultCellStyle = defaultAlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = ThemeColors.WindowAlternate,
                ForeColor = ThemeColors.WindowTextAlternate,
            };

            base.GridColor = ThemeColors.GridLine;
            RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            base.BackgroundColor = ThemeColors.Workspace;

            ToolTip?.ResetAppearance();
        }

        #endregion

        #region Methods

        #region Static Methods

        private static AdvancedToolTip CreateAdvancedToolTip() => new()
        {
            ShowAlways = true,
            InitialDelay = 0,
            UseFading = false,
            UseAnimation = false,
            AutoPopDelay = 0,
        };

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void ApplyTheme()
        {
            if (Parent == null || DesignMode)
                return;

            if (!isCustomDefaultCellStyle)
            {
                defaultDefaultCellStyle.BackColor = ThemeColors.Window;
                defaultDefaultCellStyle.ForeColor = ThemeColors.WindowText;
            }

            bool isHighContrast = SystemInformation.HighContrast;
            if (!isCustomAlternatingRowsDefaultCellStyle)
            {
                if (isHighContrast)
                    base.AlternatingRowsDefaultCellStyle = null;
                else
                {
                    defaultAlternatingRowsDefaultCellStyle.BackColor = ThemeColors.WindowAlternate;
                    defaultAlternatingRowsDefaultCellStyle.ForeColor = ThemeColors.WindowTextAlternate;
                    base.AlternatingRowsDefaultCellStyle = defaultAlternatingRowsDefaultCellStyle;
                }
            }

            if (!isCustomColumnHeadersDefaultCellStyle || !isCustomRowHeadersDefaultCellStyle)
            {
                EnableHeadersVisualStyles = !ThemeColors.IsThemingEnabled && !SystemInformation.HighContrast;
                defaultHeadersDefaultCellStyle.BackColor = ThemeColors.Control;
                defaultHeadersDefaultCellStyle.ForeColor = ThemeColors.ControlText;
            }

            if (!isCustomGridColor)
                base.GridColor = ThemeColors.GridLine;
            if (!isCustomBackgroundColor)
                base.BackgroundColor = ThemeColors.Workspace;
            if (borderStyle == BorderStyle.FixedSingle)
                base.BorderStyle = ThemeColors.IsDarkBaseTheme ? BorderStyle.None : BorderStyle.FixedSingle;

            HorizontalScrollBar.ApplyTheme();
            VerticalScrollBar.ApplyTheme();
            ToolTip?.ResetAppearance();
        }

        #endregion

        #region Protected Methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Trying to avoid double invocation of ApplyTheme
            if (ThemeColors.IsThemeEverChanged && !SystemInformation.HighContrast)
                return;

            ApplyTheme();
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);

            // Unfortunately OnSystemColorsChanged is not called when the dark/light theme changes so we have an ApplyTheme call also in ThemeColors.
            // Which also means that we can ignore the base invocation if theming will be applied anyway.
            if (ThemeColors.IsThemeEverChanged && !SystemInformation.HighContrast)
                return;

            ApplyTheme();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            if (factor.Width.Equals(1f))
                return;
            foreach (DataGridViewColumn column in Columns)
                column.Width = (int)(column.Width * factor.Width);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            isRightToLeft = RightToLeft == RightToLeft.Yes;
            ToolTip?.ResetAppearance();
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ErrorIcon);
            if ((e.PaintParts & DataGridViewPaintParts.ErrorIcon) != DataGridViewPaintParts.None)
                DrawValidationIcon(e);

            e.Handled = true;
        }

        protected override void OnRowErrorTextNeeded(DataGridViewRowErrorTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || Rows[e.RowIndex].DataBoundItem is not IValidatingObject validatingObject)
                return;

            e.ErrorText = validatingObject.ValidationResults.Message;
        }

        protected override void OnCellErrorTextNeeded(DataGridViewCellErrorTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || Rows[e.RowIndex].DataBoundItem is not IValidatingObject validatingObject)
                return;

            ValidationResultsCollection validationResults = validatingObject.ValidationResults;
            e.ErrorText = validationResults.TryGetFirstWithHighestSeverity(Columns[e.ColumnIndex].DataPropertyName)?.Message!;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InvalidateNC();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_NCCALCSIZE:
                    if (!ThemeColors.IsDarkBaseTheme || BorderStyle != BorderStyle.FixedSingle)
                        goto default;

                    unsafe
                    {
                        // actually if WParam is 1, the LParam points to an NCCALCSIZE_PARAMS structure, but we only use the first field anyway
                        var rect = (RECT*)m.LParam;
                        rect->Left += 1;
                        rect->Right -= 1;
                        rect->Top += 1;
                        rect->Bottom -= 1;
                    }

                    break;

                case Constants.WM_NCPAINT:
                    if (!ThemeColors.IsDarkBaseTheme || BorderStyle != BorderStyle.FixedSingle)
                        goto default;

                    PaintDarkNCArea(m.WParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!ThemeColors.IsSet(ThemeColor.Workspace))
                return;

            if (!HorizontalScrollBar.Visible || !VerticalScrollBar.Visible)
                return;

            var rect = new Rectangle(isRightToLeft ? 0 : Width - VerticalScrollBar.Width - 2,
                Height - HorizontalScrollBar.Height - 2,
                VerticalScrollBar.Width,
                HorizontalScrollBar.Height);

            using Brush brush = new SolidBrush(ThemeColors.Workspace);
            e.Graphics.FillRectangle(brush, rect);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                errorIcon?.Dispose();
                warningIcon?.Dispose();
                infoIcon?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void DrawValidationIcon(DataGridViewCellPaintingEventArgs e)
        {
            Rectangle bounds = e.CellBounds;
            bounds.Height -= 1;
            bounds.Width -= 1;

            Bitmap? icon = GetCellIcon(e);
            if (icon == null)
                return;

            Size size = icon.Size;
            Rectangle iconRect = new Rectangle(bounds.Left + (isRightToLeft ? 4 : bounds.Width - size.Width - 4),
                bounds.Top + ((bounds.Height >> 1) - (size.Height >> 1)),
                size.Width, size.Height);

            Rectangle iconBounds = Rectangle.Intersect(bounds, iconRect);
            if (iconBounds.IsEmpty)
                return;

            bool clip = iconRect != iconBounds;
            if (clip)
                e.Graphics!.IntersectClip(iconBounds);
            e.Graphics!.DrawImage(icon, iconRect);
            if (clip)
                e.Graphics.ResetClip();
        }

        private Bitmap? GetCellIcon(DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return null;

            // falling back to default error logic
            if (Rows[e.RowIndex].DataBoundItem is not IValidatingObject validatingObject)
                return String.IsNullOrEmpty(e.ErrorText) ? null : ErrorIcon;

            if (!OSUtils.IsWindows)
                EnsureValidationText(e, validatingObject);

            ValidationResultsCollection validationResults = validatingObject.ValidationResults;
            if (validationResults.Count == 0)
                return null;

            // row header
            if (e.ColumnIndex < 0)
            {
                return validationResults.HasErrors ? ErrorIcon
                    : validationResults.HasWarnings ? WarningIcon
                    : validationResults.HasInfos ? InfoIcon
                    : null;
            }

            // cell
            ValidationResultsCollection propertyValidation = validationResults[Columns[e.ColumnIndex].DataPropertyName];
            return propertyValidation.HasErrors ? ErrorIcon
                : propertyValidation.HasWarnings ? WarningIcon
                : propertyValidation.HasInfos ? InfoIcon
                : null;
        }

        private void EnsureValidationText(DataGridViewCellPaintingEventArgs e, IValidatingObject validatingObject)
        {
            DataGridViewRow row = Rows[e.RowIndex];
            DataGridViewCell cell = e.ColumnIndex < 0 ? row.HeaderCell : row.Cells[e.ColumnIndex];
            ValidationResultsCollection validationResults = validatingObject.ValidationResults;

            cell.ErrorText = e.ColumnIndex < 0
                ? validationResults.Message
                : validationResults.TryGetFirstWithHighestSeverity(Columns[e.ColumnIndex].DataPropertyName)?.Message;
        }

        private void InvalidateNC()
        {
            if (ThemeColors.IsDarkBaseTheme && BorderStyle == BorderStyle.FixedSingle && IsHandleCreated)
                User32.InvalidateNC(Handle);
        }

        private void PaintDarkNCArea(IntPtr hRgn)
        {
            var hWnd = Handle;
            IntPtr hDC = User32.GetNonClientDC(hWnd, hRgn);
            try
            {
                using var g = Graphics.FromHdc(hDC);
                using var pen = new Pen(ThemeColors.FixedSingleBorder);
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.DrawRectangle(pen, rect);
            }
            finally
            {
                if (hRgn == new IntPtr(1))
                    User32.ReleaseDC(hWnd, hDC);
            }
        }

        private bool ShouldSerializeDefaultCellStyle() => isCustomDefaultCellStyle;
        private bool ShouldSerializeAlternatingRowsDefaultCellStyle() => isCustomColumnHeadersDefaultCellStyle;
        private bool ShouldSerializeColumnHeadersDefaultCellStyle() => isCustomColumnHeadersDefaultCellStyle;
        private bool ShouldSerializeRowHeadersDefaultCellStyle() => isCustomRowHeadersDefaultCellStyle;
        private bool ShouldSerializeGridColor() => isCustomGridColor;
        private bool ShouldSerializeBackgroundColor() => isCustomBackgroundColor;

        #endregion

        #endregion

        #endregion
    }
}
