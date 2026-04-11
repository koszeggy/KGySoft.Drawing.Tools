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

using System.Windows.Forms;

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// A CheckBox column with per-monitor DPI and dark mode support. Standard (= System) style only.
    /// </summary>
    internal class AdvancedDataGridViewCheckBoxColumn : DataGridViewCheckBoxColumn
    {
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
    }
}