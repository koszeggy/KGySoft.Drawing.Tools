#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridViewCheckBoxCell.cs
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

#region Used Namespaces

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using KGySoft.WinForms;

#endregion

#region Used Aliases

using ContentAlignment = System.Drawing.ContentAlignment;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just a CheckBox cell that
    /// - Supports per-monitor DPI
    /// - Supports dark mode (if enabled for the parent DataGridView)
    /// - Respects Alignment Fixes broken Framework Mono behavior: mouse down on another cell, then mouse up inside the checkbox visually checks the checkbox, but not in the underlying binding.
    ///   Now the checkbox is not checked visually, though the next normal click will not toggle the checkbox. Not perfect, but it's still better than the original pseudo checked state.
    /// - Respects Alignment also in Mono Framework
    /// ! Always uses standard flat style
    /// </summary>
    internal class AdvancedDataGridViewCheckBoxCell : DataGridViewCheckBoxCell
    {
        #region Constants

        private const int margin = 2;  // horizontal and vertical margins for preferred sizes (not scaled)

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size refCheckBoxSize = new Size(13, 13);

        #endregion

        #region Instance Fields

        private bool isHovered;
        private bool isPressed;

        #endregion
        
        #endregion

        #region Properties

        #region Public Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new FlatStyle FlatStyle
        {
            get => FlatStyle.Standard;
            set
            {
                if (value is not (FlatStyle.Standard or FlatStyle.System))
                    throw new ArgumentOutOfRangeException(nameof(value), PublicResources.ArgumentOutOfRange);
            }
        }

        #endregion

        #region Private Properties

        // These are the same as in base DataGridViewCell. TODO: put it into a common extension if it should be used in more classes.
        private Rectangle StdBorderWidths
        {
            get
            {
                if (DataGridView is null)
                    return Rectangle.Empty;

                DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new();
                DataGridViewAdvancedBorderStyle effective = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded: false, singleHorizontalBorderAdded: false, isFirstDisplayedColumn: false, isFirstDisplayedRow: false);
                return BorderWidths(effective);
            }
        }

        #endregion

        #endregion

        #region Constructors

        public AdvancedDataGridViewCheckBoxCell()
            : this(false)
        {
        }

        public AdvancedDataGridViewCheckBoxCell(bool threeState)
            : base(threeState)
        {
        }

        #endregion

        #region Methods

        #region Static Methods

        [SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "Readability")]
        private static bool? IsFreeWidth(Size constraintSize) => constraintSize is { Width: 0, Height: 0 } ? null
            : constraintSize.Width == 0 ? true
            : constraintSize.Height == 0 ? false
            : throw new ArgumentOutOfRangeException(nameof(constraintSize), PublicResources.ArgumentOutOfRange);

        // These are the same as in base DataGridViewCell. TODO: put them into a common extension if they should be used in more classes.
        private static bool PaintBorder(DataGridViewPaintParts paintParts) => (paintParts & DataGridViewPaintParts.Border) != 0;
        private static bool PaintSelectionBackground(DataGridViewPaintParts paintParts) => (paintParts & DataGridViewPaintParts.SelectionBackground) != 0;
        private static bool PaintBackground(DataGridViewPaintParts paintParts) => (paintParts & DataGridViewPaintParts.Background) != 0;
        private static bool PaintFocus(DataGridViewPaintParts paintParts) => (paintParts & DataGridViewPaintParts.Focus) != 0;
        private static bool PaintContentForeground(DataGridViewPaintParts paintParts) => (paintParts & DataGridViewPaintParts.ContentForeground) != 0;

        #endregion

        #region Instance Methods

        #region Protected Methods

        /// <summary>
        /// Executed when the auto size is calculated (e.g. when the separator line is double-clicked in the row/column header).
        /// </summary>
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (DataGridView is null)
                return new Size(-1, -1);

            if (cellStyle is null)
                throw new ArgumentNullException(nameof(cellStyle), PublicResources.ArgumentNull);

            bool? freeWidth = IsFreeWidth(constraintSize);
            Rectangle borderWidthsRect = StdBorderWidths;
            Size borderAndPadding = new(borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal,
                borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical);
            Size preferredSize;
            if (VisualStyleHelper.RenderWithVisualStyles)
            {
                var element = VisualStyleElement.Button.CheckBox.UncheckedNormal;
                Size checkBoxSize = VisualStyleHelper.GetPartSize(element.ClassName, DataGridView.GetHandleIfCreated(), graphics, element.Part, element.State, false);

                preferredSize = freeWidth switch
                {
                    true => new Size(checkBoxSize.Width + borderAndPadding.Width + 2 * margin, 0),
                    false => new Size(0, checkBoxSize.Height + borderAndPadding.Height + 2 * margin),
                    _ => new Size(checkBoxSize.Width + borderAndPadding.Width + 2 * margin, checkBoxSize.Height + borderAndPadding.Height + 2 * margin)
                };
            }
            else
            {
                Size checkBoxSize = DataGridView.ScaleSize(refCheckBoxSize) + new Size(2 * margin, 2 * margin);
                preferredSize = freeWidth switch
                {
                    true => new Size(checkBoxSize.Width + borderAndPadding.Width, 0),
                    false => new Size(0, checkBoxSize.Height + borderAndPadding.Height),
                    _ => checkBoxSize + borderAndPadding
                };
            }

            // We should consider the border size when calculating the preferred size.
            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle effectiveStyle, out var _, out var _);
            Rectangle borderWidths = BorderWidths(effectiveStyle);
            preferredSize.Width += borderWidths.X;
            preferredSize.Height += borderWidths.Y;
            return preferredSize;
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
                throw new ArgumentNullException(nameof(cellStyle), PublicResources.ArgumentNull);

            if (DataGridView is null || rowIndex < 0 || OwningColumn is null)
                return Rectangle.Empty;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle effectiveStyle, out DataGridViewElementStates cellState, out Rectangle cellBounds);
            Rectangle checkBoxBounds = PaintPrivate(graphics, cellBounds, cellBounds, rowIndex, cellState,
                null, cellStyle, effectiveStyle, DataGridViewPaintParts.ContentForeground, true, false);

            return checkBoxBounds;
        }

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object? value,
            object? formattedValue, string? errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (cellStyle is null)
                throw new ArgumentNullException(nameof(cellStyle), PublicResources.ArgumentNull);

            if (DataGridView is null)
                return;

            PaintPrivate(graphics, clipBounds, cellBounds, rowIndex, elementState, formattedValue, cellStyle, advancedBorderStyle,
                paintParts, false, true);
        }

        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (DataGridView is null || ReadOnly)
                return;

            bool prevHovered = isHovered;
            isHovered = GetContentBounds(e.RowIndex).Contains(e.X, e.Y);
            if (prevHovered == isHovered)
                return;

            if (VisualStyleHelper.RenderWithVisualStyles)
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);

            Point? mouseDownCell = DataGridView.MouseDownCellAddress;
            bool? mouseDownInCheckBox = DataGridView.CellMouseDownInContentBounds;
            if (mouseDownCell == null || mouseDownInCheckBox == null)
                return;

            if (e.ColumnIndex == mouseDownCell.Value.X && e.RowIndex == mouseDownCell.Value.Y && Control.MouseButtons == MouseButtons.Left)
            {
                if (!isPressed && mouseDownInCheckBox == true)
                    isPressed = true;
                else
                    isPressed = false;
            }
        }

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DataGridView is null || ReadOnly)
                return;

            if (e.Button == MouseButtons.Left && isHovered)
            {
                isPressed = true;
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
            }
        }

        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (DataGridView is null || ReadOnly)
                return;

            if (e.Button == MouseButtons.Left)
            {
                isPressed = false;
                DataGridView.InvalidateCell(ColumnIndex, e.RowIndex);
            }
        }

        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            if (DataGridView is null || ReadOnly)
                return;

            if (isHovered)
            {
                isHovered = false;
                if (ColumnIndex >= 0 && rowIndex >= 0 && VisualStyleHelper.RenderWithVisualStyles)
                    DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }

            // For a normal control this would not be needed, but once the mouse leaves the cell, there will be no OnMouseUp. Resetting isReset in OnMouseMove.
            if (isPressed)
            {
                isPressed = false;
                DataGridView.InvalidateCell(ColumnIndex, rowIndex);
            }
        }

        #endregion

        #region Private Methods

        // NOTE: Changes to the original code:
        // - Removed Flat/Popup cases
        // - Removed error bounds calculation
        // - Per-monitor DPI scaling
        // - Rendering respects dark mode if enabled for DataGridView control
        // - Unlike the base method, it is called from two methods only (Paint, GetContentBounds), because GetErrorBounds is not overridden for the following reasons:
        //    - In this project the checkbox column contains no icons
        //    - The AdvancedDataGridView has special icons, but not even other cell types calculate with their size. To do that, every column should have some advanced column type.
        private Rectangle PaintPrivate(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState,
            object? formattedValue, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts,
            bool computeContentBounds, bool paint)
        {
            Rectangle resultBounds;

            if (paint && PaintBorder(paintParts))
                PaintBorder(g, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;
            bool cellSelected = (elementState & DataGridViewElementStates.Selected) != 0;
            bool isIndeterminate = formattedValue is CheckState.Indeterminate;
            bool isChecked = formattedValue is true or CheckState.Checked;
            Debug.Assert(DataGridView is not null);
            Point ptCurrentCell = DataGridView!.CurrentCellAddress;

            Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
                ? cellStyle.SelectionBackColor
                : cellStyle.BackColor;

            if (paint && PaintBackground(paintParts) && brushColor.A == Byte.MaxValue)
                g.FillRectangle(brushColor.GetBrush(), valBounds);

            if (cellStyle.Padding != Padding.Empty)
            {
                if (DataGridView.RightToLeft == RightToLeft.Yes)
                    valBounds.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
                else
                    valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);

                valBounds.Width -= cellStyle.Padding.Horizontal;
                valBounds.Height -= cellStyle.Padding.Vertical;
            }

            if (paint && PaintFocus(paintParts) && (DataGridView as AdvancedDataGridView)?.FocusCuesVisible == true
                && DataGridView.Focused && ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex)
            {
                ControlPaint.DrawFocusRectangle(g, valBounds, cellStyle.BackColor, cellStyle.ForeColor);
            }

            valBounds.Inflate(-margin, -margin);

            Size checkBoxSize;
            bool needsScaledRendering = false;
            IntPtr hwnd = DataGridView.GetHandleIfCreated();

            var element = VisualStyleElement.Button.CheckBox.UncheckedNormal;
            if (VisualStyleHelper.RenderWithVisualStyles)
            {
                if (isIndeterminate)
                {
                    element = isPressed ? VisualStyleElement.Button.CheckBox.MixedPressed
                        : isHovered ? VisualStyleElement.Button.CheckBox.MixedHot
                        : VisualStyleElement.Button.CheckBox.MixedNormal;
                }
                else if (isChecked)
                {
                    element = isPressed ? VisualStyleElement.Button.CheckBox.CheckedPressed
                        : isHovered ? VisualStyleElement.Button.CheckBox.CheckedHot
                        : VisualStyleElement.Button.CheckBox.CheckedNormal;
                }
                else
                {
                    element = isPressed ? VisualStyleElement.Button.CheckBox.UncheckedPressed
                        : isHovered ? VisualStyleElement.Button.CheckBox.UncheckedHot
                        : VisualStyleElement.Button.CheckBox.UncheckedNormal;
                }

                checkBoxSize = VisualStyleHelper.GetPartSize(element.ClassName, hwnd, g, element.Part, element.State, false);
                needsScaledRendering = !ScaleHelper.IsDefaultSystemScale && checkBoxSize != VisualStyleHelper.GetPartSize(element.ClassName, hwnd, g, element.Part, element.State, true);
            }
            else
                checkBoxSize = DataGridView.ScaleSize(refCheckBoxSize);

            if (valBounds.Width >= checkBoxSize.Width && valBounds.Height >= checkBoxSize.Height && (paint || computeContentBounds))
            {
                int checkBoxY;
                int checkBoxX;

                ContentAlignment alignment = ((ContentAlignment)cellStyle.Alignment).RtlTranslateContent(DataGridView);
                if (alignment.AnyRight())
                    checkBoxX = valBounds.Right - checkBoxSize.Width;
                else if (alignment.AnyCenter())
                    checkBoxX = valBounds.Left + (valBounds.Width - checkBoxSize.Width) / 2;
                else
                    checkBoxX = valBounds.Left;

                if (alignment.AnyBottom())
                    checkBoxY = valBounds.Bottom - checkBoxSize.Height;
                else if (alignment.AnyMiddle())
                    checkBoxY = valBounds.Top + (valBounds.Height - checkBoxSize.Height) / 2;
                else
                    checkBoxY = valBounds.Top;

                resultBounds = new Rectangle(checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height);

                if (VisualStyleHelper.RenderWithVisualStyles)
                {
                    if (paint && PaintContentForeground(paintParts))
                    {
                        if (needsScaledRendering)
                            VisualStyleHelper.RenderScaled(element.ClassName, hwnd, g, element.Part, element.State, resultBounds);
                        else
                            VisualStyleHelper.Render(element.ClassName, hwnd, g, element.Part, element.State, resultBounds);
                    }
                }
                else
                {
                    ButtonState bs;
                    if (formattedValue is CheckState state)
                        bs = (state == CheckState.Unchecked) ? ButtonState.Normal : ButtonState.Checked;
                    else if (formattedValue is bool formattedValueAsBool)
                        bs = formattedValueAsBool ? ButtonState.Checked : ButtonState.Normal;
                    else
                        // The provided formatted value has a wrong type. We raised a DataError event while formatting.
                        bs = ButtonState.Normal; // Default rendering of the checkbox with wrong formatted value type.

                    if (isPressed)
                        bs |= ButtonState.Pushed;

                    if (paint && PaintContentForeground(paintParts))
                    {
                        if (isIndeterminate)
                            ControlPaint.DrawMixedCheckBox(g, checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height, bs);
                        else
                            ControlPaint.DrawCheckBox(g, checkBoxX, checkBoxY, checkBoxSize.Width, checkBoxSize.Height, bs);
                    }
                }
            }
            else
                resultBounds = Rectangle.Empty;

            return resultBounds;
        }

        // The following methods are the same as in base DataGridViewCell. TODO: put them into a common extension if they should be used in more classes.
        private void ComputeBorderStyleCellStateAndCellBounds(int rowIndex, out DataGridViewAdvancedBorderStyle effectiveStyle, out DataGridViewElementStates cellState, out Rectangle cellBounds)
        {
            bool singleVerticalBorderAdded = !DataGridView!.RowHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            bool singleHorizontalBorderAdded = !DataGridView.ColumnHeadersVisible && DataGridView.AdvancedCellBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Single;
            DataGridViewAdvancedBorderStyle dataGridViewAdvancedBorderStylePlaceholder = new();

            if (rowIndex > -1 && OwningColumn is not null)
            {
                // Inner cell case
                effectiveStyle = AdjustCellBorderStyle(DataGridView.AdvancedCellBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded, singleHorizontalBorderAdded,
                    ColumnIndex == DataGridView.FirstDisplayedColumnIndex, rowIndex == DataGridView.FirstDisplayedRowIndex);
                DataGridViewElementStates rowState = DataGridView.Rows.GetRowState(rowIndex);
                cellState = CellStateFromColumnRowStates(rowState);
                cellState |= State;
            }
            else if (OwningColumn is not null)
            {
                // Column header cell case
                DataGridViewColumn? dataGridViewColumn = DataGridView.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                bool isLastVisibleColumn = (dataGridViewColumn is not null && dataGridViewColumn.Index == ColumnIndex);
                effectiveStyle = DataGridView.AdjustColumnHeaderBorderStyle(DataGridView.AdvancedColumnHeadersBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                    ColumnIndex == DataGridView.FirstDisplayedColumnIndex, isLastVisibleColumn);
                cellState = OwningColumn.State | State;
            }
            else if (OwningRow is not null)
            {
                // Row header cell case
                effectiveStyle = OwningRow.AdjustRowHeaderBorderStyle(DataGridView.AdvancedRowHeadersBorderStyle, dataGridViewAdvancedBorderStylePlaceholder,
                    singleVerticalBorderAdded, singleHorizontalBorderAdded,
                    rowIndex == DataGridView.FirstDisplayedRowIndex, rowIndex == DataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible));
                cellState = OwningRow.GetState(rowIndex) | State;
            }
            else
            {
                // TopLeft header cell case
                effectiveStyle = DataGridView.AdjustedTopLeftHeaderBorderStyle;
                cellState = State;
            }

            cellBounds = new Rectangle(Point.Empty, GetSize(rowIndex));
        }

        private DataGridViewElementStates CellStateFromColumnRowStates(DataGridViewElementStates rowState)
        {
            DataGridViewElementStates orFlags = DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.Selected;
            DataGridViewElementStates andFlags = DataGridViewElementStates.Displayed | DataGridViewElementStates.Frozen | DataGridViewElementStates.Visible;
            DataGridViewElementStates cellState = OwningColumn!.State & orFlags;
            cellState |= rowState & orFlags;
            cellState |= OwningColumn.State & andFlags & rowState & andFlags;
            return cellState;
        }

        #endregion

        #endregion

        #endregion
    }
}
