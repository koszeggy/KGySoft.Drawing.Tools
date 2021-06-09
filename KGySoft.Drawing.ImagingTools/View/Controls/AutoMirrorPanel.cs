#region Usings

using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
