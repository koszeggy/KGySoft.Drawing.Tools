#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolStripProgressBar.cs
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
using System.Drawing;
using System.Windows.Forms;

using KGySoft.WinForms;
using KGySoft.WinForms.Controls;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    internal class AdvancedToolStripProgressBar : ToolStripControlHost
    {
        #region Constants

        private const int referenceProgressHeight = 14;
        private const int referenceHorizontalPadding = 5;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceSize = new Size(100, 20);

        #endregion

        #region Instance Fields

        private readonly Panel panel;
        private readonly AdvancedProgressBar progressBar;

        #endregion

        #endregion

        #region Properties

        #region Public Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override Size Size
        {
            get => base.Size;
            set => base.Size = value;
        }

        #endregion

        #region Protected Properties

        protected override Size DefaultSize => referenceSize.Scale(Owner?.GetScale() ?? ScaleHelper.SystemScale);

        #endregion

        #endregion

        #region Constructors

        public AdvancedToolStripProgressBar()
            : base(new Panel())
        {
            panel = (Panel)Control;
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.Transparent;
            progressBar = new AdvancedProgressBar
            {
                IsMarquee = true,
                Dock = DockStyle.Top,
                RightToLeftLayout = true
            };
            panel.Controls.Add(progressBar);
        }

        #endregion

        #region Methods

        #region Public Methods

        public override Size GetPreferredSize(Size constrainingSize)
        {
            var defaultSize = DefaultSize;

            if (Owner == null || !Owner.IsHandleCreated || IsOnOverflow || Owner.Orientation == Orientation.Vertical)
                return defaultSize;

            // Filling the whole available area
            int width = Owner.DisplayRectangle.Width;
            if (Owner.GripStyle == ToolStripGripStyle.Visible)
                width -= Owner.GripRectangle.Width + Owner.GripMargin.Horizontal;
            if (Owner.OverflowButton.Visible)
                width -= Owner.OverflowButton.Width + Owner.OverflowButton.Margin.Horizontal;

            foreach (ToolStripItem item in Owner.Items)
            {
                if (item.IsOnOverflow || !item.Available || item == this)
                    continue;
                width -= item.Width + item.Margin.Horizontal;
            }

            return new Size(Math.Max(width, defaultSize.Width), defaultSize.Height);
        }

        #endregion

        #region Protected Methods

        protected override void OnBoundsChanged()
        {
            base.OnBoundsChanged();
            if (Owner == null)
                return;

            PointF scale = Owner.GetScale();
            int progressBarHeight = referenceProgressHeight.Scale(scale.Y);
            progressBar.Height = progressBarHeight;
            int leftPadding = referenceHorizontalPadding.Scale(scale.X);
            int rightPadding = 0;
            if (RightToLeft == RightToLeft.Yes)
                (leftPadding, rightPadding) = (rightPadding, leftPadding);
            panel.Padding = new Padding(leftPadding, (Height - progressBarHeight) >> 1, rightPadding, 0);
        }

        #endregion

        #endregion
    }
}
