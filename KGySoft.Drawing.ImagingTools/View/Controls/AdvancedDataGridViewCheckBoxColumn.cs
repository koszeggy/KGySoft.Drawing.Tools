#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridViewCheckBoxColumn.cs
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
using System.Windows.Forms;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A CheckBox column that
    /// - Supports per-monitor DPI
    /// - Supports dark mode (Standard/System) style only.
    /// - Takes the tool tip from resources instead of constant True/False
    /// </summary>
    internal class AdvancedDataGridViewCheckBoxColumn : DataGridViewCheckBoxColumn
    {
        #region Fields

        private DataGridView? grid;

        #endregion

        #region Properties

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

        #region Constructors

        public AdvancedDataGridViewCheckBoxColumn()
            : this(false)
        {
        }

        public AdvancedDataGridViewCheckBoxColumn(bool threeState)
            : base(threeState)
        {
            CellTemplate = new AdvancedDataGridViewCheckBoxCell(threeState);

            // NOTE: Framework Mono does not set alignment, and only the default rendering shows the checkbox in the middle.
            // But as our custom rendering respects Alignment, we must explicitly set centered alignment in the default style.
            if (OSHelper.IsFrameworkMono)
            {
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    NullValue = threeState ? CheckState.Indeterminate : false
                };
            }
        }

        #endregion

        #region Methods

        #region Protected Methods
        
        protected override void OnDataGridViewChanged()
        {
            base.OnDataGridViewChanged();
            if (DataGridView?.IsDisposed != false)
                return;

            grid?.CellToolTipTextNeeded -= DataGridView_CellToolTipTextNeeded;
            DataGridView?.CellToolTipTextNeeded += DataGridView_CellToolTipTextNeeded;
            grid = DataGridView;
        }

        protected override void Dispose(bool disposing)
        {
            grid?.CellToolTipTextNeeded -= DataGridView_CellToolTipTextNeeded;
            grid = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers
        
        private void DataGridView_CellToolTipTextNeeded(object? sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            Debug.Assert(sender == DataGridView);
            if (DataGridView is null || e.ColumnIndex < 0 || e.RowIndex < 0 || e.ColumnIndex >= DataGridView.ColumnCount || DataGridView.Columns[e.ColumnIndex] != this)
                return;

            // already set
            if (!String.IsNullOrEmpty(e.ToolTipText))
                return;

            e.ToolTipText = DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value switch
            {
                true or CheckState.Checked => Res.Get(CheckState.Checked),
                false or CheckState.Unchecked => Res.Get(CheckState.Unchecked),
                _ => Res.Get(CheckState.Indeterminate)
            };
        }

        #endregion

        #endregion
    }
}