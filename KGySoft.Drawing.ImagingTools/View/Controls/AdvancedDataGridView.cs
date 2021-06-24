#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridView.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    #region Usings

#if NET35 || NET40
    using ValidationList = IList<ValidationResult>;
#else
    using ValidationList = IReadOnlyList<ValidationResult>;
#endif

    #endregion

    /// <summary>
    /// Just a DataGridView that
    /// - provides some default style with a few fixed issues
    /// - scales the columns automatically
    /// - provides scaling error/warning/info icons, which appear also on Linux/Mono
    /// </summary>
    internal class AdvancedDataGridView : DataGridView
    {
        #region Fields

        private readonly DataGridViewCellStyle defaultDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultColumnHeadersDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultAlternatingRowsDefaultCellStyle;

        private bool isRightToLeft;
        private Bitmap? errorIcon;
        private Bitmap? warningIcon;
        private Bitmap? infoIcon;

        #endregion

        #region Properties

        #region Public Properties

        // these are reintroduced just for the ShouldSerialize... methods
        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle DefaultCellStyle
        {
            get => base.DefaultCellStyle;
            set => base.DefaultCellStyle = value;
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle ColumnHeadersDefaultCellStyle
        {
            get => base.ColumnHeadersDefaultCellStyle;
            set => base.ColumnHeadersDefaultCellStyle = value;
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle AlternatingRowsDefaultCellStyle
        {
            get => base.AlternatingRowsDefaultCellStyle;
            set => base.AlternatingRowsDefaultCellStyle = value;
        }

        #endregion

        #region Private Properties

        private Bitmap ErrorIcon => errorIcon ??= Icons.SystemError.ToScaledBitmap(this.GetScale());
        private Bitmap WarningIcon => warningIcon ??= Icons.SystemWarning.ToScaledBitmap(this.GetScale());
        private Bitmap InfoIcon => infoIcon ??= Icons.SystemInformation.ToScaledBitmap(this.GetScale());

        #endregion

        #endregion

        #region Constructors

        public AdvancedDataGridView()
        {
            DefaultCellStyle = defaultDefaultCellStyle = new DataGridViewCellStyle(DefaultCellStyle)
            {
                // Base default uses Window back color with ControlText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
            };

            ColumnHeadersDefaultCellStyle = defaultColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                // Base default uses Control back color with WindowText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.ControlText,
            };

            AlternatingRowsDefaultCellStyle = defaultAlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.ControlLight,
                ForeColor = SystemColors.ControlText,
            };
        }

        #endregion

        #region Methods

        #region Static Methods

        private static string GetRowValidationText(ValidationResultsCollection validationResults) => validationResults.Count switch
        {
            0 => String.Empty,
            1 => validationResults[0].Message,
            _ => validationResults.Select(r => r.Message).Join(Environment.NewLine)
        };

        private static string GetCellValidationText(ValidationResultsCollection validationResults, string propertyName)
        {
            int len = validationResults.Count;
            if (len == 0)
                return String.Empty;

            for (var s = ValidationSeverity.Error; s >= ValidationSeverity.Information; s--)
            {
                for (int i = 0; i < len; i++)
                {
                    if (validationResults[i].Severity == s && validationResults[i].PropertyName == propertyName)
                        return validationResults[i].Message;
                }
            }

            return String.Empty;
        }

        #endregion

        #region Instance Methods

        #region Protected Methods

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            AdjustAlternatingRowsColors();
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            AdjustAlternatingRowsColors();
            AlternatingRowsDefaultCellStyle = SystemInformation.HighContrast
                ? null
                : new DataGridViewCellStyle { BackColor = SystemColors.ControlLight, ForeColor = SystemColors.ControlText };
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

            e.ErrorText = GetRowValidationText(validatingObject.ValidationResults);
        }

        protected override void OnCellErrorTextNeeded(DataGridViewCellErrorTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || Rows[e.RowIndex].DataBoundItem is not IValidatingObject validatingObject)
                return;

            ValidationResultsCollection validationResults = validatingObject.ValidationResults;
            e.ErrorText = validationResults.Count == 0 ? String.Empty : GetCellValidationText(validationResults, Columns[e.ColumnIndex].DataPropertyName);
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
                e.Graphics.IntersectClip(iconBounds);
            e.Graphics.DrawImage(icon, iconRect);
            if (clip)
                e.Graphics.ResetClip();
        }

        private Bitmap? GetCellIcon(DataGridViewCellPaintingEventArgs e)
        {
            #region Local Methods

            static bool HasMatch(ValidationList list, string name)
            {
                // not using LINQ and delegates for better performance
                int len = list.Count;
                for (int i = 0; i < len; i++)
                {
                    if (list[i].PropertyName == name)
                        return true;
                }

                return false;
            }

            #endregion

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
            string propertyName = Columns[e.ColumnIndex].DataPropertyName;
            return HasMatch(validationResults.Errors, propertyName) ? ErrorIcon
                : HasMatch(validationResults.Warnings, propertyName) ? WarningIcon
                : HasMatch(validationResults.Infos, propertyName) ? InfoIcon
                : null;
        }

        private void EnsureValidationText(DataGridViewCellPaintingEventArgs e, IValidatingObject validatingObject)
        {
            DataGridViewRow row = Rows[e.RowIndex];
            DataGridViewCell cell = e.ColumnIndex < 0 ? row.HeaderCell : row.Cells[e.ColumnIndex];
            ValidationResultsCollection validationResults = validatingObject.ValidationResults;

            cell.ErrorText = e.ColumnIndex < 0
                ? GetRowValidationText(validationResults)
                : GetCellValidationText(validationResults, Columns[e.ColumnIndex].DataPropertyName);
        }

        private void AdjustAlternatingRowsColors()
        {
            if (!Equals(AlternatingRowsDefaultCellStyle, defaultAlternatingRowsDefaultCellStyle))
                return;

            AlternatingRowsDefaultCellStyle = SystemInformation.HighContrast
                ? null
                : defaultAlternatingRowsDefaultCellStyle;
        }

        private bool ShouldSerializeAlternatingRowsDefaultCellStyle() => !Equals(AlternatingRowsDefaultCellStyle, defaultAlternatingRowsDefaultCellStyle);
        private bool ShouldSerializeColumnHeadersDefaultCellStyle() => !Equals(ColumnHeadersDefaultCellStyle, defaultColumnHeadersDefaultCellStyle);
        private bool ShouldSerializeDefaultCellStyle() => !Equals(DefaultCellStyle, defaultDefaultCellStyle);

        #endregion

        #endregion

        #endregion
    }
}
