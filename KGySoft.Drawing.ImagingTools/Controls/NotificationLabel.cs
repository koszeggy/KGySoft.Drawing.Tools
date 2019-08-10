#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: NotificationLabel.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Controls
{
    internal class NotificationLabel : Label
    {
        #region Fields

        private Size lastProposedSize;

        #endregion

        #region Properties

        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                lastProposedSize = Size.Empty;
                Visible = !String.IsNullOrEmpty(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Image Image
        {
            get => base.Image;
            set
            {
                // if the image to set is a multi-res bitmap, then adjusting the icon size
                if (value?.RawFormat.Equals(ImageFormat.Icon) == true)
                {
                    float scale;
                    using (Graphics g = CreateGraphics())
                        scale = (float)Math.Round(Math.Max(g.DpiX, g.DpiY) / 96, 2);
                    if (scale > 1)
                    {
                        // the temp bitmap is not used for anything - it just makes value to the best fitting size
                        using (new Bitmap(value, Size.Round(new SizeF(value.Width * scale, value.Height * scale)))) { }
                    }
                }

                base.Image = value;
            }
        }

        #endregion

        #region Constructors

        public NotificationLabel()
        {
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.FromArgb(255, 255, 128);
            ForeColor = Color.Black;
            Image = Icons.Warning.ToScaledBitmap();
            TextAlign = ContentAlignment.MiddleLeft;
            ImageAlign = ContentAlignment.MiddleRight;
        }

        #endregion

        #region Methods

        #region Public Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            // Workaround: Immediately after calculating preferred size (eg. Dock == Top), another request arrives with empty proposedSize, which ruins the constrained result.
            if (proposedSize == Size.Empty && lastProposedSize != Size.Empty && Dock != DockStyle.None)
            {
                proposedSize = lastProposedSize;

                // in design mode further Empty proposedSizes may arrive so clearing only at runtime
                if (!DesignMode)
                    lastProposedSize = Size.Empty;
            }
            else
            {
                lastProposedSize = proposedSize;
            }

            //TextFormatFlags formatFlags = this.GetFormatFlags();

            Size padding = GetBordersAndPadding();
            Size proposedTextSize = proposedSize - padding;

            // 0 or 1 means unbounded
            if (proposedTextSize.Width <= 1)
                proposedTextSize.Width = Int32.MaxValue;
            if (proposedTextSize.Height <= 1)
                proposedTextSize.Height = Int32.MaxValue;

            Size preferredSize;
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                if (String.IsNullOrEmpty(base.Text))
                {
                    preferredSize = Size.Ceiling(g.MeasureString("0", base.Font, 0));
                    preferredSize.Width = 0;
                }
                else
                {
                    preferredSize = TextRenderer.MeasureText(g, base.Text, base.Font, proposedTextSize, TextFormatFlags.WordBreak);
                }
            }

            preferredSize += padding;
            if (proposedSize.Width > preferredSize.Width)
                preferredSize.Width = proposedSize.Width;
            if (proposedSize.Height > preferredSize.Height)
                preferredSize.Height = proposedSize.Height;

            //preferredSizeCache[((long)proposedSize.Height << 32) | proposedSize.Width] = preferredSize;
            return preferredSize;
        }

        #endregion

        #region Protected Methods

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Visible = false;
        }

        #endregion

        #region Private Methods

        private Size GetBordersAndPadding()
        {
            Size size = Padding.Size;
            size += SizeFromClientSize(Size.Empty);
            int borderWidth = 0;
            if (BorderStyle == BorderStyle.FixedSingle)
                borderWidth = 1;
            else if (BorderStyle == BorderStyle.Fixed3D)
                borderWidth = 2;
            size += new Size(borderWidth << 1, borderWidth << 1);
            return size;
        }

        #endregion

        #endregion
    }
}
