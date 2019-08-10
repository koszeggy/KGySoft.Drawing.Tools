#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AppMainForm.cs
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
using System.Drawing.Imaging;
using System.IO;

#endregion

namespace KGySoft.Drawing.ImagingTools.Forms
{
    internal partial class ImagingToolsForm : ImageDebuggerVisualizerForm
    {
        #region Constants

        private const string title = "KGy SOFT Imaging Tools";

        #endregion

        #region Fields

        private string fileName;
        private string captionInfo;

        #endregion

        #region Properties

        public override string Text
        {
            get => base.Text;
            set
            {
                captionInfo = value;
                UpdateText();
            }
        }

        #endregion

        #region Constructors

        public ImagingToolsForm() => InitializeComponent();

        public ImagingToolsForm(string[] args) : this()
        {
            if (args == null || args.Length == 0)
            {
                Notification = $"As a standalone application, {title} can be used to load images, save them in various formats, extract frames or pages, examine or change palette entries of indexed images, etc.{Environment.NewLine}{Environment.NewLine}"
                    + $"But it can be used also as a debugger visualizer for {nameof(Image)}, {nameof(Bitmap)}, {nameof(Metafile)}, {nameof(BitmapData)}, {nameof(Graphics)}, {nameof(ColorPalette)} and {nameof(Color)} types.{Environment.NewLine}"
                    + $"See the '{btnConfiguration.Text}' button.";

                return;
            }

            string file = args[0];
            if (!File.Exists(file))
            {
                Dialogs.ErrorMessage("File does not exist: {0}", file);
                return;
            }

            OpenFile(file);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        protected override bool OpenFile(string path)
        {
            if (!base.OpenFile(path))
                return false;
            fileName = Path.GetFileName(path);
            UpdateText();
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            fileName = null;
            UpdateText();
        }

        #endregion

        #region Private Methods

        private void UpdateText() => base.Text = String.IsNullOrEmpty(captionInfo) ? title : $"{title}{(fileName == null ? null : $" [{Path.GetFileName(fileName)}]")} - {captionInfo}";

        #endregion

        #endregion
    }
}
