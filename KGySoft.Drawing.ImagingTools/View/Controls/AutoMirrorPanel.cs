#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AutoMirrorPanel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Windows.Forms;

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - Controls items are never null
#pragma warning disable CS8602 // Dereference of a possibly null reference. - Controls items are never null
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just for mirroring content for RTL languages.
    /// In this project all relevant controls are docked so handling the Dock property only.
    /// </summary>
    internal class AutoMirrorPanel : Panel
    {
        #region Fields

        private readonly List<Control> toBeAdjusted = new List<Control>();

        #endregion

        #region Methods

        protected override void OnControlAdded(ControlEventArgs e)
        {
            // there is no public IsLayoutSuspended property but in this project we can assume that controls are either added
            // in suspended state or before setting RightToLeft
            base.OnControlAdded(e);
            toBeAdjusted.Add(e.Control);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (toBeAdjusted.Count == 0)
                return;

            if (RightToLeft == RightToLeft.Yes)
            {
                foreach (Control control in toBeAdjusted)
                {
                    // Adjusting docking only
                    DockStyle dockStyle = control.Dock;
                    if (dockStyle == DockStyle.Left)
                        control.Dock = DockStyle.Right;
                    else if (dockStyle == DockStyle.Right)
                        control.Dock = DockStyle.Left;
                }
            }

            toBeAdjusted.Clear();
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            toBeAdjusted.Clear();
            foreach (Control control in Controls)
            {
                // Adjusting docking only
                DockStyle dockStyle = control.Dock;
                if (dockStyle == DockStyle.Left)
                    control.Dock = DockStyle.Right;
                else if (dockStyle == DockStyle.Right)
                    control.Dock = DockStyle.Left;
            }
        }

        #endregion
    }
}
