#region Used namespaces

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KGySoft.DebuggerVisualizers.Common.Properties;

#endregion

namespace KGySoft.DebuggerVisualizers.Common
{
    internal partial class GraphicsDebuggerVisualizerForm : ImageDebuggerVisualizerForm
    {
        #region Fields

        private ToolStripButton btnCrop;
        private ToolStripButton btnHighlightClip;
        private Image origImage;

        #endregion

        #region Properties

        internal Rectangle VisibleRect { get; set; }

        internal Matrix Transform { get; set; }

        internal string SpecialInfo { get; set; }

        internal override Image Image
        {
            get { return base.Image; }
            set
            {
                origImage = value;
                UpdateGraphicImage();
                btnCrop.Enabled = btnHighlightClip.Enabled = value != null && (value.Size != VisibleRect.Size || VisibleRect.Location != Point.Empty);
            }
        }

        #endregion

        #region Construction and Destruction

        #region Constructors

        public GraphicsDebuggerVisualizerForm()
        {
            InitializeComponent();
            ImageTypes = ImageTypes.None;
            ToolStripItem separator = new ToolStripSeparator();
            btnCrop = new ToolStripButton(Resources.Crop)
            {
                Text = "Show visible clip area only",
                CheckOnClick = true,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            btnCrop.Click += new EventHandler(btnCrop_Click);
            btnHighlightClip = new ToolStripButton(Resources.HighlightVisibleClip)
            {
                Text = "Highlight visible clip area",
                CheckOnClick = true,
                Checked = true,
                DisplayStyle = ToolStripItemDisplayStyle.Image
            };

            btnHighlightClip.Click += new EventHandler(btnHighlightClip_Click);

            tsMenu.Items.AddRange(new ToolStripItem[] { separator, btnCrop, btnHighlightClip });
        }

        #endregion

        #region Explicit Disposing

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            btnCrop.Click -= btnHighlightClip_Click;
            btnHighlightClip.Click -= btnHighlightClip_Click;

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                if (origImage != null)
                    origImage.Dispose();
            }

            origImage = null;

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void UpdateInfo()
        {
            if (Transform == null || SpecialInfo == null)
                return;

            Text = String.Format("Type: Graphics; {1}Visible Clip Bounds: {0}", VisibleRect, Transform.IsIdentity ? String.Empty : "Untransformed ");
            txtInfo.Text = SpecialInfo;
        }

        #endregion

        #region Private Methods

        private void UpdateGraphicImage()
        {
            Image oldImage = base.Image;
            if (oldImage != null)
                oldImage.Dispose();
            base.Image = null;

            if (origImage == null)
                return;

            Rectangle visibleRect = VisibleRect;
            if (btnCrop.Checked && (visibleRect.Size != origImage.Size || visibleRect.Location != Point.Empty))
            {
                if (visibleRect.Width <= 0 || visibleRect.Height <= 0)
                    return;

                Bitmap newImage = new Bitmap(visibleRect.Width, visibleRect.Height);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(origImage, new Rectangle(Point.Empty, visibleRect.Size), visibleRect, GraphicsUnit.Pixel);
                }

                base.Image = newImage;
                return;
            }

            if (btnHighlightClip.Checked && (visibleRect.Size != origImage.Size || visibleRect.Location != Point.Empty))
            {
                Bitmap newImage = new Bitmap(origImage);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    using (Brush b = new SolidBrush(Color.FromArgb(128, Color.Black)))
                    {
                        g.FillRectangle(b, 0, 0, newImage.Width, visibleRect.Top);
                        g.FillRectangle(b, 0, visibleRect.Bottom, newImage.Width, newImage.Height - visibleRect.Bottom);
                        g.FillRectangle(b, 0, visibleRect.Top, visibleRect.Left, visibleRect.Height);
                        g.FillRectangle(b, visibleRect.Right, visibleRect.Top, newImage.Width - visibleRect.Height, visibleRect.Height);
                        visibleRect.Inflate(1, 1);
                        ControlPaint.DrawFocusRectangle(g, visibleRect, Color.White, Color.Black);
                    }
                }

                base.Image = newImage;
                return;
            }

            base.Image = (Image)origImage.Clone();
        }

        #endregion

        #region Handled Events
        // ReSharper disable InconsistentNaming

        void btnHighlightClip_Click(object sender, EventArgs e)
        {
            UpdateGraphicImage();
        }

        void btnCrop_Click(object sender, EventArgs e)
        {
            btnHighlightClip.Enabled = !btnCrop.Checked;
            UpdateGraphicImage();
        }

        // ReSharper restore InconsistentNaming
        #endregion

        #endregion
    }
}
