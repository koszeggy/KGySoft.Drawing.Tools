#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DataGridViewExtensions.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Reflection;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class DataGridViewExtensions
    {
        #region Properties

        extension(DataGridView grid)
        {
            internal int FirstDisplayedColumnIndex
            {
                get
                {
                    if (!grid.IsHandleCreated)
                        return -1;

                    int firstDisplayedColumnIndex = -1;
                    DataGridViewColumn? dataGridViewColumn = grid.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                    if (dataGridViewColumn is null)
                        return firstDisplayedColumnIndex;

                    if (dataGridViewColumn.Frozen)
                        firstDisplayedColumnIndex = dataGridViewColumn.Index;
                    else if (grid.FirstDisplayedScrollingColumnIndex >= 0)
                        firstDisplayedColumnIndex = grid.FirstDisplayedScrollingColumnIndex;

                    return firstDisplayedColumnIndex;
                }
            }

            internal int FirstDisplayedRowIndex
            {
                get
                {
                    if (!grid.IsHandleCreated)
                        return -1;

                    int firstDisplayedRowIndex = grid.Rows.GetFirstRow(DataGridViewElementStates.Visible);
                    if (firstDisplayedRowIndex == -1)
                        return firstDisplayedRowIndex;
                    if ((grid.Rows.GetRowState(firstDisplayedRowIndex) & DataGridViewElementStates.Frozen) == 0 && grid.FirstDisplayedScrollingRowIndex >= 0)
                        firstDisplayedRowIndex = grid.FirstDisplayedScrollingRowIndex;

                    return firstDisplayedRowIndex;
                }
            }

            internal Point? MouseDownCellAddress
            {
                get
                {
                    if (!OSHelper.IsFrameworkMono && Reflector.TryGetProperty(grid, "MouseDownCellAddress", out object? value) && value is Point result)
                        return result;

                    return null;
                }
            }

            internal bool? CellMouseDownInContentBounds
            {
                get
                {
                    if (!OSHelper.IsFrameworkMono && Reflector.TryGetProperty(grid, "CellMouseDownInContentBounds", out object? value) && value is bool result)
                        return result;

                    return null;
                }
            }
        }

        #endregion
    }
}
