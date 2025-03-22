#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
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
using KGySoft.Drawing.DebuggerVisualizers.Core.Test.ViewModel;
using KGySoft.Drawing.DebuggerVisualizers.Test;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Test.View
{
    public partial class DebuggerTestForm : Form
    {
        #region Fields

        private readonly CommandBindingsCollection commandBindings = new CommandBindingsCollection();
        private readonly DebuggerTestViewModel viewModel = new DebuggerTestViewModel();
        private readonly Timer? timer;

        private string? errorMessage;

        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();
            gbFile.AutoSize = !OSUtils.IsMono;
            cmbPixelFormat.DataSource = viewModel.PixelFormats;

            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.SelectedFormat), nameof(ComboBox.SelectedItem), cmbPixelFormat);
            commandBindings.AddPropertyBinding(cmbPixelFormat, nameof(ComboBox.SelectedValue), nameof(viewModel.SelectedFormat), viewModel);

            commandBindings.AddPropertyBinding(rbManagedBitmapData, nameof(RadioButton.Checked), nameof(viewModel.ManagedBitmapData), viewModel);
            commandBindings.AddPropertyBinding(rbPalette, nameof(RadioButton.Checked), nameof(viewModel.Palette), viewModel);
            commandBindings.AddPropertyBinding(rbColor32, nameof(RadioButton.Checked), nameof(viewModel.Color32), viewModel);
            commandBindings.AddPropertyBinding(rbPColor32, nameof(RadioButton.Checked), nameof(viewModel.PColor32), viewModel);
            commandBindings.AddPropertyBinding(rbColor64, nameof(RadioButton.Checked), nameof(viewModel.Color64), viewModel);
            commandBindings.AddPropertyBinding(rbPColor64, nameof(RadioButton.Checked), nameof(viewModel.PColor64), viewModel);
            commandBindings.AddPropertyBinding(rbColorF, nameof(RadioButton.Checked), nameof(viewModel.ColorF), viewModel);
            commandBindings.AddPropertyBinding(rbPColorF, nameof(RadioButton.Checked), nameof(viewModel.PColorF), viewModel);
            commandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.BitmapDataFromFile), viewModel);

            commandBindings.AddPropertyBinding(txtFile, nameof(txtFile.Text), nameof(viewModel.FileName), viewModel);
            commandBindings.AddPropertyBinding(rbAsNative, nameof(RadioButton.Checked), nameof(viewModel.FileAsNative), viewModel);
            commandBindings.AddPropertyBinding(rbAsManaged, nameof(RadioButton.Checked), nameof(viewModel.FileAsManaged), viewModel);

            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormatEnabled), nameof(cmbPixelFormat.Enabled), cmbPixelFormat);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.BitmapDataFromFile), nameof(gbFile.Enabled), gbFile);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebug), nameof(Button.Enabled), btnViewDirect, btnViewByDebugger);
            commandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PreviewImage), nameof(pbPreview.Image), pbPreview);

            commandBindings.Add<EventArgs>(OnSelectFileCommand)
                .AddSource(txtFile, nameof(txtFile.Click))
                .AddSource(txtFile, nameof(txtFile.DoubleClick));
            commandBindings.Add(viewModel.DirectViewCommand).AddSource(btnViewDirect, nameof(btnViewDirect.Click));
            commandBindings.Add(viewModel.DebugCommand).AddSource(btnViewByDebugger, nameof(btnViewByDebugger.Click));

            viewModel.GetHwndCallback = () => Handle;

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
            
            using var ofd = new OpenFileDialog();
            ofd.FileName = txtFile.Text;
            if (ofd.ShowDialog() == DialogResult.OK)
                txtFile.Text = ofd.FileName;
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
