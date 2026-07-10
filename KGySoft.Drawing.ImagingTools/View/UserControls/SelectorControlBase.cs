#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: SelectorControlBase.cs
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

using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal class SelectorControlBase : MvvmBaseUserControl
    {
        #region Constants

        // Adjusted for Segoe UI 9 font on 100% DPI
        private const int settingRefHeight = 25;
        private const float panelColumnRefWidth = 140f;

        #endregion

        #region Fields

        private static readonly Padding checkBoxRefPadding = new Padding(4, 0, 0, 0);
        private static readonly Padding tableItemRefMargin = new Padding(4, 0, 4, 0);
        private static readonly Padding tablePanelRefMargin = new Padding(3);

        #endregion

        #region Methods

        protected override void ApplySizeAdjustments(PointF? dynamicSizesScale)
        {
            PointF scale = this.GetScale();
            foreach (Control control in Controls)
            {
                switch (control)
                {
                    case CheckBox checkBox:
                        checkBox.Height = settingRefHeight.Scale(scale.Y);
                        checkBox.Padding = checkBoxRefPadding.Scale(scale);
                        break;

                    case TableLayoutPanel table:
                        float columnWidth = panelColumnRefWidth * scale.X;
                        table.Height = settingRefHeight.Scale(scale.Y);
                        table.ColumnStyles[0].Width = table.ColumnStyles[1].Width = columnWidth;
                        foreach (Control childControl in table.Controls)
                        {
                            switch (childControl)
                            {
                                case Label or TextBox:
                                    childControl.Margin = tableItemRefMargin.Scale(scale);
                                    break;

                                case Panel panel:
                                    panel.Margin = tablePanelRefMargin.Scale(scale);
                                    break;
                            }
                        }

                        break;
                }
            }

            ResetParentSize();
        }

        protected void ResetParentSize()
        {
            Control? parent = Parent?.Parent;
            if (parent == null)
                return;

            int height = 0;
            foreach (Control control in Controls)
            {
                if (!control.Visible)
                    continue;

                height += control.Height;
            }

            parent.Height = height + (parent.Height - parent.DisplayRectangle.Height);
        }

        #endregion
    }
}
