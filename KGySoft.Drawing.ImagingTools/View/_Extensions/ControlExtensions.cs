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

using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ControlExtensions
    {
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
            return WindowsUtils.GetScale(control.Handle);
        }

        internal static Size ScaleSize(this Control control, Size size) => size.Scale(control.GetScale());

        /// <summary>
        /// Applies fixed string resources (which do not change unless language is changed) to a control.
        /// </summary>
        internal static void ApplyStaticStringResources(this Control control)
        {
            string name = control.Name;
            if (String.IsNullOrEmpty(name))
                name = control.GetType().Name;

            // to self
            Res.ApplyResources(control, name);

            // to children
            foreach (Control child in control.Controls) child.ApplyStaticStringResources();

            // to non-control sub-components
            switch (control)
            {
                case ToolStrip toolStrip:
                    toolStrip.Items.ApplyStaticStringResources();
                    break;
            }
        }

        internal static void ApplyStaticStringResources(this ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                // to self
                Res.ApplyResources(item, item.Name);

                // to children
                if (item is ToolStripDropDownItem dropDownItem)
                    dropDownItem.DropDownItems.ApplyStaticStringResources();
            }
        }

        #endregion
    }
}
