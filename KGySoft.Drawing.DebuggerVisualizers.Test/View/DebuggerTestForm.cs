#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestForm.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.View.Forms;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test.View
{
    public partial class DebuggerTestForm : Form
    {
        #region Fields

        private readonly CommandBindingsCollection commandBindings = new CommandBindingsCollection();
        private readonly DebuggerTestFormViewModel viewModel = new DebuggerTestFormViewModel();

        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();

            commandBindings.Add(viewModel.DebugCommand).AddSource(btnViewByDebugger, nameof(btnViewByDebugger.Click));
            commandBindings.Add(viewModel.DirectViewCommand).AddSource(btnViewDirect, nameof(btnViewDirect.Click));
            commandBindings.Add<EventArgs>(OnSelectFileCommand)
                .AddSource(tbFile, nameof(tbFile.Click))
                .AddSource(tbFile, nameof(tbFile.DoubleClick));

            commandBindings.AddPropertyBinding(rbBitmap32, nameof(RadioButton.Checked), nameof(viewModel.Bmp32), viewModel);
            commandBindings.AddPropertyBinding(rbBitmap16, nameof(RadioButton.Checked), nameof(viewModel.Bmp16), viewModel);
            commandBindings.AddPropertyBinding(rbBitmap8, nameof(RadioButton.Checked), nameof(viewModel.Bmp8), viewModel);
            commandBindings.AddPropertyBinding(rbMetafile, nameof(RadioButton.Checked), nameof(viewModel.Metafile), viewModel);
            commandBindings.AddPropertyBinding(rbHIcon, nameof(RadioButton.Checked), nameof(viewModel.HIcon), viewModel);
            commandBindings.AddPropertyBinding(rbManagedIcon, nameof(RadioButton.Checked), nameof(viewModel.ManagedIcon), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsBitmap, nameof(RadioButton.Checked), nameof(viewModel.GraphicsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsHwnd, nameof(RadioButton.Checked), nameof(viewModel.GraphicsHwnd), viewModel);
            commandBindings.AddPropertyBinding(rbBitmapData32, nameof(RadioButton.Checked), nameof(viewModel.BitmapData32), viewModel);
            commandBindings.AddPropertyBinding(rbBitmapData8, nameof(RadioButton.Checked), nameof(viewModel.BitmapData8), viewModel);
            commandBindings.AddPropertyBinding(rbPalette256, nameof(RadioButton.Checked), nameof(viewModel.Palette256), viewModel);
            commandBindings.AddPropertyBinding(rbPalette2, nameof(RadioButton.Checked), nameof(viewModel.Palette2), viewModel);
            commandBindings.AddPropertyBinding(rbColor, nameof(RadioButton.Checked), nameof(viewModel.SingleColor), viewModel);
            commandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.ImageFromFile), viewModel);

            commandBindings.AddPropertyBinding(tbFile, nameof(tbFile.Text), nameof(viewModel.FileName), viewModel);
            commandBindings.AddPropertyBinding(rbAsImage, nameof(RadioButton.Checked), nameof(viewModel.AsImage), viewModel);
            commandBindings.AddPropertyBinding(rbAsBitmap, nameof(RadioButton.Checked), nameof(viewModel.AsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbAsMetafile, nameof(RadioButton.Checked), nameof(viewModel.AsMetafile), viewModel);
            commandBindings.AddPropertyBinding(rbAsIcon, nameof(RadioButton.Checked), nameof(viewModel.AsIcon), viewModel);

            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PreviewImage), nameof(pictureBox.Image), pictureBox);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebugDirectly), nameof(btnViewDirect.Enabled), btnViewDirect);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebugByDebugger), nameof(btnViewByDebugger.Enabled), btnViewByDebugger);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.ImageFromFile), nameof(gbFile.Enabled), gbFile);

            viewModel.GetHwndCallback = () => Handle;
            viewModel.GetClipCallback = () => pictureBox.Bounds;
            viewModel.ErrorCallback = Dialogs.ErrorMessage;
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                commandBindings.Dispose();
                viewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void OnSelectFileCommand(ICommandSource<EventArgs> source)
        {
            // simple click opens the file dialog only if text was empty
            if (tbFile.Text.Length != 0 && source.TriggeringEvent == nameof(tbFile.Click))
                return;
            using (OpenFileDialog ofd = new OpenFileDialog { FileName = tbFile.Text })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    tbFile.Text = ofd.FileName;
            }
        }

        #endregion

        #endregion
    }
}
