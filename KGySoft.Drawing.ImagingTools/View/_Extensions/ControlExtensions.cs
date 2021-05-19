#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ControlExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Drawing;
using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ControlExtensions
    {
        #region Constants

        internal const string ToolTipPropertyName = "ToolTipText";

        #endregion

        #region Methods

        /// <summary>
        /// Sets the double buffering state of a control
        /// </summary>
        /// <param name="control">The control to set.</param>
        /// <param name="useDoubleBuffering"><see langword="true"/>, if <paramref name="control"/> should use double buffering; otherwise, <see langword="false"/>.</param>
        internal static void SetDoubleBuffered(this Control control, bool useDoubleBuffering)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            Reflector.SetProperty(control, "DoubleBuffered", useDoubleBuffering);
        }

        internal static PointF GetScale(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            return OSUtils.GetScale(control.Handle);
        }

        internal static Size ScaleSize(this Control control, Size size) => size.Scale(control.GetScale());

        internal static int ScaleWidth(this Control control, int width) => width.Scale(control.GetScale().X);

        /// <summary>
        /// Applies fixed string resources (which do not change unless language is changed) to a control.
        /// </summary>
        internal static void ApplyStringResources(this Control control, ToolTip? toolTip = null)
        {
            #region Local Methods

            static void ApplyToolTip(Control control, string name, ToolTip toolTip)
            {
                string? value = Res.GetStringOrNull(name + "." + ToolTipPropertyName);
                toolTip.SetToolTip(control, value);
            }

            static void ApplyToolStripResources(ToolStripItemCollection items)
            {
                foreach (ToolStripItem item in items)
                {
                    // to self
                    Res.ApplyStringResources(item, item.Name);

                    // to children
                    if (item is ToolStripDropDownItem dropDownItem)
                        ApplyToolStripResources(dropDownItem.DropDownItems);
                }
            }

            #endregion

            string name = control.Name;
            if (String.IsNullOrEmpty(name))
                name = control.GetType().Name;

            // custom localization
            if (control is ICustomLocalizable customLocalizable)
            {
                customLocalizable.ApplyStringResources(toolTip);
                return;
            }

            // to self
            Res.ApplyStringResources(control, name);

            // applying tool tip
            if (toolTip != null)
                ApplyToolTip(control, name, toolTip);

            // to children
            switch (control)
            {
                case ToolStrip toolStrip:
                    ApplyToolStripResources(toolStrip.Items);
                    break;

                default:
                    foreach (Control child in control.Controls)
                        child.ApplyStringResources(toolTip);
                    break;
            }
        }

        internal static void FixAppearance(this ToolStrip toolStrip)
        {
            static void FixItems(ToolStripItemCollection items, Color replacementColor)
            {
                foreach (ToolStripItem item in items)
                {
                    // to self
                    if ((item is ToolStripMenuItem || item is ToolStripLabel || item is ToolStripSeparator || item is ToolStripProgressBar) && item.BackColor.ToArgb() == replacementColor.ToArgb())
                        item.BackColor = replacementColor;

                    // to children
                    if (item is ToolStripDropDownItem dropDownItem)
                        FixItems(dropDownItem.DropDownItems, replacementColor);
                }
            }

            if (OSUtils.IsWindows || SystemInformation.HighContrast)
                return;

            // fixing "dark on dark" menu issue on Linux
            Color replacementColor = Color.FromArgb(ProfessionalColors.MenuStripGradientBegin.ToArgb());
            toolStrip.BackColor = replacementColor;
            FixItems(toolStrip.Items, replacementColor);
        }

        #endregion
    }
}
