#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ZoomSplitButton.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    internal class ZoomSplitButton : AdvancedToolStripSplitButton
    {
        #region Properties

        #region Public Properties
        
        // Overridden just to prevent saving a fixed low-res image in the .resx file
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image Image { get => base.Image; set => base.Image = value; }

        #endregion

        #region Internal Properties
        
        internal ToolStripMenuItem AutoZoomMenuItem { get; }
        internal ToolStripMenuItem IncreaseZoomMenuItem { get; }
        internal ToolStripMenuItem DecreaseZoomMenuItem { get; }
        internal ToolStripMenuItem ResetZoomMenuItem { get; }

        #endregion

        #endregion

        #region Constructors

        public ZoomSplitButton()
        {
            CheckOnClick = true;
            Image = Images.Magnifier;
            ToolStripItemCollection items = DropDownItems;

            AutoZoomMenuItem = new ToolStripMenuItem
            {
                Name = "miAutoZoom",
                Image = Images.Magnifier,
                CheckOnClick = true,
                ShortcutKeys = Keys.Alt | Keys.Z
            };
            AutoZoomMenuItem.CheckedChanged += (_, _) => Checked = AutoZoomMenuItem.Checked;

            IncreaseZoomMenuItem = new ToolStripMenuItem
            {
                Name = "miIncreaseZoom",
                Image = Images.MagnifierPlus,
                ShortcutKeys = Keys.Control | Keys.Add,
                ShortcutKeyDisplayString = @"Ctrl++",
            };

            DecreaseZoomMenuItem = new ToolStripMenuItem
            {
                Name = "miDecreaseZoom",
                Image = Images.MagnifierMinus,
                ShortcutKeys = Keys.Control | Keys.Subtract,
                ShortcutKeyDisplayString = @"Ctrl+-",
            };

            ResetZoomMenuItem = new ToolStripMenuItem
            {
                Name = "miResetZoom",
                Image = Images.Magnifier1,
                ShortcutKeys = Keys.Control | Keys.NumPad0,
                ShortcutKeyDisplayString = @"Ctrl+0",
            };

            items.AddRange(new ToolStripItem[] { AutoZoomMenuItem, IncreaseZoomMenuItem, DecreaseZoomMenuItem, ResetZoomMenuItem });
        }

        #endregion

        #region Methods

        protected override void OnCheckedChanged(EventArgs e)
        {
            base.OnCheckedChanged(e);
            AutoZoomMenuItem.Checked = Checked;
        }

        #endregion
    }
}
