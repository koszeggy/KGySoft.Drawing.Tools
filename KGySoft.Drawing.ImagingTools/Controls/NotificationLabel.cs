using System;
using System.Drawing;
using System.Windows.Forms;

namespace KGySoft.Drawing.ImagingTools.Controls
{
    internal class NotificationLabel : Label
    {
        public NotificationLabel()
        {
            AutoSize = true;
            BorderStyle = BorderStyle.FixedSingle;
            BackColor = Color.FromArgb(255, 255, 128);
            ForeColor = Color.Black;
            Image = Icons.Warning.ToMultiResBitmap();
        }

        private Size lastProposedSize;

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                lastProposedSize = Size.Empty;
                Visible = !String.IsNullOrEmpty(value);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Visible = false;
        }

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

    }
}
