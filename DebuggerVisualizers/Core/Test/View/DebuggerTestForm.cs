#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestForm.cs
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
using System.Drawing;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.DebuggerVisualizers.Core.Test.ViewModel;
using KGySoft.WinForms;
using KGySoft.WinForms.Forms;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Test.View
{
    public partial class DebuggerTestForm : BaseForm
    {
        #region Fields

        private readonly DebuggerTestViewModel viewModel = new DebuggerTestViewModel();

        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();
            gbFile.AutoSize = !OSHelper.IsMono;
            if (!IsDesignMode && !OSHelper.IsMono && SystemFonts.MessageBoxFont is Font font)
                Font = font;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            bool isLoaded = IsLoaded;
            base.OnLoad(e);
            if (!isLoaded)
                InitBindings();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                viewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitBindings()
        {
            cmbPixelFormat.DataSource = viewModel.PixelFormats;

            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.SelectedFormat), nameof(ComboBox.SelectedItem), cmbPixelFormat);
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(ComboBox.SelectedValue), nameof(viewModel.SelectedFormat), viewModel);

            CommandBindings.AddPropertyBinding(rbManagedBitmapData, nameof(RadioButton.Checked), nameof(viewModel.ManagedBitmapData), viewModel);
            CommandBindings.AddPropertyBinding(rbPalette, nameof(RadioButton.Checked), nameof(viewModel.Palette), viewModel);
            CommandBindings.AddPropertyBinding(rbColor32, nameof(RadioButton.Checked), nameof(viewModel.Color32), viewModel);
            CommandBindings.AddPropertyBinding(rbPColor32, nameof(RadioButton.Checked), nameof(viewModel.PColor32), viewModel);
            CommandBindings.AddPropertyBinding(rbColor64, nameof(RadioButton.Checked), nameof(viewModel.Color64), viewModel);
            CommandBindings.AddPropertyBinding(rbPColor64, nameof(RadioButton.Checked), nameof(viewModel.PColor64), viewModel);
            CommandBindings.AddPropertyBinding(rbColorF, nameof(RadioButton.Checked), nameof(viewModel.ColorF), viewModel);
            CommandBindings.AddPropertyBinding(rbPColorF, nameof(RadioButton.Checked), nameof(viewModel.PColorF), viewModel);
            CommandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.BitmapDataFromFile), viewModel);

            CommandBindings.AddPropertyBinding(txtFile, nameof(txtFile.Text), nameof(viewModel.FileName), viewModel);
            CommandBindings.AddPropertyBinding(rbAsNative, nameof(RadioButton.Checked), nameof(viewModel.FileAsNative), viewModel);
            CommandBindings.AddPropertyBinding(rbAsManaged, nameof(RadioButton.Checked), nameof(viewModel.FileAsManaged), viewModel);

            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormatEnabled), nameof(cmbPixelFormat.Enabled), cmbPixelFormat);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.BitmapDataFromFile), nameof(gbFile.Enabled), gbFile);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebug), nameof(Button.Enabled), btnViewDirect, btnViewByClassicDebugger);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PreviewImage), nameof(pbPreview.Image), pbPreview);

            CommandBindings.Add<EventArgs>(OnSelectFileCommand)
                .AddSource(txtFile, nameof(txtFile.Click))
                .AddSource(txtFile, nameof(txtFile.DoubleClick));
            CommandBindings.Add(viewModel.DirectViewCommand).AddSource(btnViewDirect, nameof(btnViewDirect.Click));
            CommandBindings.Add(viewModel.ClassicDebugCommand).AddSource(btnViewByClassicDebugger, nameof(btnViewByClassicDebugger.Click));
            CommandBindings.Add(viewModel.ExtensionDebugCommand).AddSource(btnViewByExtensionDebugger, nameof(btnViewByExtensionDebugger.Click));

            viewModel.GetHwndCallback = () => Handle;
            viewModel.ErrorCallback = msg => Dialogs.ErrorMessage(this, msg);
        }

        #endregion

        #region Command Handlers

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

        #endregion

        #endregion
    }
}
