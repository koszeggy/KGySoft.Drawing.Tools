#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.View
{
    public partial class DebuggerTestForm : Form
    {
        #region Fields

        private readonly CommandBindingsCollection commandBindings = new CommandBindingsCollection();
        private readonly DebuggerTestFormViewModel viewModel = new DebuggerTestFormViewModel();
        private readonly Timer? timer;

        private string? errorMessage;

        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();
            gbFile.AutoSize = !OSUtils.IsMono;
            cmbPixelFormat.DataSource = viewModel.PixelFormats;

            commandBindings.AddPropertyBinding(chbAsImage, nameof(CheckBox.Checked), nameof(viewModel.AsImage), viewModel);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormat), nameof(ComboBox.SelectedItem), cmbPixelFormat);
            commandBindings.AddPropertyBinding(cmbPixelFormat, nameof(ComboBox.SelectedValue), nameof(viewModel.PixelFormat), viewModel);

            commandBindings.AddPropertyBinding(rbBitmap, nameof(RadioButton.Checked), nameof(viewModel.Bitmap), viewModel);
            commandBindings.AddPropertyBinding(rbMetafile, nameof(RadioButton.Checked), nameof(viewModel.Metafile), viewModel);
            commandBindings.AddPropertyBinding(rbHIcon, nameof(RadioButton.Checked), nameof(viewModel.HIcon), viewModel);
            commandBindings.AddPropertyBinding(rbManagedIcon, nameof(RadioButton.Checked), nameof(viewModel.ManagedIcon), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsBitmap, nameof(RadioButton.Checked), nameof(viewModel.GraphicsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsHwnd, nameof(RadioButton.Checked), nameof(viewModel.GraphicsHwnd), viewModel);
            commandBindings.AddPropertyBinding(rbBitmapData, nameof(RadioButton.Checked), nameof(viewModel.BitmapData), viewModel);
            commandBindings.AddPropertyBinding(rbPalette, nameof(RadioButton.Checked), nameof(viewModel.Palette), viewModel);
            commandBindings.AddPropertyBinding(rbColor, nameof(RadioButton.Checked), nameof(viewModel.SingleColor), viewModel);
            commandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.ImageFromFile), viewModel);

            commandBindings.AddPropertyBinding(txtFile, nameof(txtFile.Text), nameof(viewModel.FileName), viewModel);
            commandBindings.AddPropertyBinding(rbAsImage, nameof(RadioButton.Checked), nameof(viewModel.FileAsImage), viewModel);
            commandBindings.AddPropertyBinding(rbAsBitmap, nameof(RadioButton.Checked), nameof(viewModel.FileAsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbAsMetafile, nameof(RadioButton.Checked), nameof(viewModel.FileAsMetafile), viewModel);
            commandBindings.AddPropertyBinding(rbAsIcon, nameof(RadioButton.Checked), nameof(viewModel.FileAsIcon), viewModel);

            commandBindings.AddPropertyBinding(chbAsReadOnly, nameof(CheckBox.Checked), nameof(viewModel.AsReadOnly), viewModel);

            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.AsImageEnabled), nameof(chbAsImage.Enabled), chbAsImage);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormatEnabled), nameof(cmbPixelFormat.Enabled), cmbPixelFormat);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.ImageFromFile), nameof(gbFile.Enabled), gbFile);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.AsReadOnlyEnabled), nameof(chbAsReadOnly.Enabled), chbAsReadOnly);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebug), nameof(Button.Enabled), btnViewDirect, btnViewByDebugger);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PreviewImage), nameof(pictureBox.Image), pictureBox);

            commandBindings.Add<EventArgs>(OnSelectFileCommand)
                .AddSource(txtFile, nameof(txtFile.Click))
                .AddSource(txtFile, nameof(txtFile.DoubleClick));
            commandBindings.Add(viewModel.DirectViewCommand).AddSource(btnViewDirect, nameof(btnViewDirect.Click));
            commandBindings.Add(viewModel.DebugCommand).AddSource(btnViewByDebugger, nameof(btnViewByDebugger.Click));

            viewModel.GetHwndCallback = () => Handle;
            viewModel.GetClipCallback = () => pictureBox.Bounds;

            if (OSUtils.IsWindows)
            {
                viewModel.ErrorCallback = msg => MessageBox.Show(this, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Due to some strange issue on Linux the app may crash if we show a MessageBox while changing radio buttons
            // so as a workaround we show error messages by using a timer. Another solution would be to show a custom dialog.
            timer = new Timer { Interval = 1 };
            viewModel.ErrorCallback = message =>
            {
                errorMessage = message;
                timer.Enabled = true;
            };
            commandBindings.Add(OnShowErrorCommand)
                .AddSource(timer, nameof(timer.Tick));
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
                timer?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void OnSelectFileCommand(ICommandSource<EventArgs> source)
        {
            // simple click opens the file dialog only if text was empty
            if (txtFile.Text.Length != 0 && source.TriggeringEvent == nameof(txtFile.Click))
                return;
            using (var ofd = new OpenFileDialog { FileName = txtFile.Text })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    txtFile.Text = ofd.FileName;
            }
        }

        private void OnShowErrorCommand()
        {
            timer!.Enabled = false;
            if (errorMessage != null)
                MessageBox.Show(this, errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            errorMessage = null;
        }

        #endregion

        #endregion
    }
}
