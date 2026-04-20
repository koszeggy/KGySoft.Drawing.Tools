#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridView.cs
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
using KGySoft.WinForms;

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - Columns items are never null
#pragma warning disable CS8602 // Dereference of a possibly null reference. - Columns items are never null
#endif

#if NETFRAMEWORK
// ReSharper disable AssignNullToNotNullAttribute - Nullability annotation is not the same on old frameworks
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just a DataGridView that
    /// - provides some default styles/colors with a few fixed issues
    /// - scales the columns and rows automatically
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
        private Font? clonedFont;

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
                    : ThemeColors.HighContrast ? null
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

        [DefaultValue(DataGridViewColumnHeadersHeightSizeMode.AutoSize)]
        public new DataGridViewColumnHeadersHeightSizeMode ColumnHeadersHeightSizeMode
        {
            get => base.ColumnHeadersHeightSizeMode;
            set => base.ColumnHeadersHeightSizeMode = value;
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

        [AllowNull]
        public override Font Font
        {
            get => base.Font;
            set
            {
                if (value is null || !Equals(base.Font, value))
                {
                    base.Font = value;
                    return;
                }

                // Needed to prevent possible exception on some platforms when the framework disposes the self font internally.
                // NOTE: could be refactored the same way as other advanced controls in the KGySoft.WinForms package.
                clonedFont?.Dispose();
                clonedFont = (Font)value.Clone();
                base.Font = clonedFont;
            }
        }

        #endregion

        #region Internal Properties
        
        internal bool FocusCuesVisible => base.ShowFocusCues;

        #endregion

        #region Private Properties

        private Bitmap ErrorIcon => errorIcon ??= Icons.SystemError.ToScaledBitmap(this.GetScale());
        private Bitmap WarningIcon => warningIcon ??= Icons.SystemWarning.ToScaledBitmap(this.GetScale());
        private Bitmap InfoIcon => infoIcon ??= Icons.SystemInformation.ToScaledBitmap(this.GetScale());

        private AdvancedToolTip? ToolTip
        {
            get
            {
                // Framework Mono has a ToolTip tooltip_window field
                if (OSHelper.IsFrameworkMono)
                {
                    if (!toolAccessorsInitialized)
                    {
                        try
                        {
                            FieldInfo? fld = typeof(DataGridView).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("tooltip_window", StringComparison.Ordinal));
                            if (fld is not null)
                            {
                                toolTipControlField = FieldAccessor.GetAccessor(fld);
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

                // Assuming Windows implementation here (including Wine Mono): DataGridView.*toolTipControl*.*toolTip*
                if (!toolAccessorsInitialized)
                {
                    try
                    {
                        FieldInfo? fld = typeof(DataGridView).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("toolTipControl", StringComparison.Ordinal));
                        if (fld is not null)
                        {
                            toolTipControlField = FieldAccessor.GetAccessor(fld);
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
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;

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
                defaultDefaultCellStyle.SelectionBackColor = ThemeColors.Highlight;
                defaultDefaultCellStyle.SelectionForeColor = ThemeColors.HighlightText;
            }

            if (!isCustomAlternatingRowsDefaultCellStyle)
            {
                if (ThemeColors.HighContrast)
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
                EnableHeadersVisualStyles = !ThemeColors.IsThemingEnabled && !ThemeColors.HighContrast;
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

        internal void AdjustSizes(PointF factor)
        {
            RowHeadersWidth = (int)(RowHeadersWidth * factor.X);
            foreach (DataGridViewColumn column in Columns)
                column.Width = (int)(column.Width * factor.X);
            foreach (DataGridViewRow row in Rows)
                row.Height = (int)(row.Height * factor.Y);

            // Unfortunately this does not work, because the base DataGridView immediately overwrites the size
            // Forcing by Minimum/MaximumSize also does not work because the painted area still counts with the system DPI size
            //var scrollbarSize = this.GetScrollBarSize();
            //VerticalScrollBar.Width = scrollbarSize.Width;
            //HorizontalScrollBar.Height = scrollbarSize.Height;
        }

        #endregion

        #region Protected Methods

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            RowTemplate.Height = ColumnHeadersHeight;

            // Trying to avoid double invocation of ApplyTheme
            if (ThemeColors.IsThemeEverChanged && !ThemeColors.HighContrast)
                return;

            ApplyTheme();
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);

            // Unfortunately OnSystemColorsChanged is not called when the dark/light theme changes so we have an ApplyTheme call also in ThemeColors.
            // Which also means that we can ignore the base invocation if theming will be applied anyway.
            if (ThemeColors.IsThemeEverChanged && !ThemeColors.HighContrast)
                return;

            ApplyTheme();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);

            // We are here when the framework scales the control by its own platform-dependent logic.
            // Allowing it only when the handle is not created yet, which occurs when initializing the font for the first time.
            // Once the handle is created, this method is typically called on DPI change, but the behavior depends on the platform target and configurations.
            // For example, .NET Framework 4.7.2 behavior is good when app.config has NO PerMonitorV2 awareness, but is a mess when PerMonitorV2 DpiAwareness is set.
            // Considering that as a debugger visualizer, we depend on the configuration of devenv.exe, we must be prepared for any case.
            // NOTE: This logic is alright for this project, but may not suffice for a general AdvancedDataGridView.
            if (!factor.Width.Equals(1f) && !IsHandleCreated)
                AdjustSizes(factor.ToPointF());
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

                    return;

                case Constants.WM_NCPAINT:
                    if (!ThemeColors.IsDarkBaseTheme || BorderStyle != BorderStyle.FixedSingle)
                        goto default;

                    PaintDarkNCArea(m.WParam);
                    break;

                case Constants.WM_DPICHANGED_BEFOREPARENT:
                    base.WndProc(ref m);
                    ReleaseIcons();
                    return;

                default:
                    base.WndProc(ref m);
                    return;
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

            e.Graphics.FillRectangle(ThemeColors.Workspace.GetBrush(), rect);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (ColumnHeadersHeightSizeMode == DataGridViewColumnHeadersHeightSizeMode.AutoSize)
                RowTemplate.Height = ColumnHeadersHeight;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseIcons();
                clonedFont?.Dispose();
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

            if (!OSHelper.IsWindows)
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
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.DrawRectangle(ThemeColors.FixedSingleBorder.GetPen(), rect);
            }
            finally
            {
                if (hRgn == new IntPtr(1))
                    User32.ReleaseDC(hWnd, hDC);
            }
        }

        private void ReleaseIcons()
        {
            errorIcon?.Dispose();
            warningIcon?.Dispose();
            infoIcon?.Dispose();
            errorIcon = null;
            warningIcon = null;
            infoIcon = null;
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
