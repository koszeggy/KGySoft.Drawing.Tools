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
using KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.ViewModel;
using KGySoft.WinForms;
using KGySoft.WinForms.Forms;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test.View
{
    public partial class DebuggerTestForm : BaseForm
    {
        #region Fields

        private readonly DebuggerTestFormViewModel viewModel = new DebuggerTestFormViewModel();
        
        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();
            gbFile.AutoSize = !OSHelper.IsFrameworkMono;
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

            CommandBindings.AddPropertyBinding(chbAsImage, nameof(CheckBox.Checked), nameof(viewModel.AsImage), viewModel);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormat), nameof(ComboBox.SelectedItem), cmbPixelFormat);
            CommandBindings.AddPropertyBinding(cmbPixelFormat, nameof(ComboBox.SelectedValue), nameof(viewModel.PixelFormat), viewModel);

            CommandBindings.AddPropertyBinding(rbBitmap, nameof(RadioButton.Checked), nameof(viewModel.Bitmap), viewModel);
            CommandBindings.AddPropertyBinding(rbMetafile, nameof(RadioButton.Checked), nameof(viewModel.Metafile), viewModel);
            CommandBindings.AddPropertyBinding(rbHIcon, nameof(RadioButton.Checked), nameof(viewModel.HIcon), viewModel);
            CommandBindings.AddPropertyBinding(rbManagedIcon, nameof(RadioButton.Checked), nameof(viewModel.ManagedIcon), viewModel);
            CommandBindings.AddPropertyBinding(rbGraphicsBitmap, nameof(RadioButton.Checked), nameof(viewModel.GraphicsBitmap), viewModel);
            CommandBindings.AddPropertyBinding(rbGraphicsHwnd, nameof(RadioButton.Checked), nameof(viewModel.GraphicsHwnd), viewModel);
            CommandBindings.AddPropertyBinding(rbBitmapData, nameof(RadioButton.Checked), nameof(viewModel.BitmapData), viewModel);
            CommandBindings.AddPropertyBinding(rbPalette, nameof(RadioButton.Checked), nameof(viewModel.Palette), viewModel);
            CommandBindings.AddPropertyBinding(rbColor, nameof(RadioButton.Checked), nameof(viewModel.SingleColor), viewModel);
            CommandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.ImageFromFile), viewModel);

            CommandBindings.AddPropertyBinding(txtFile, nameof(txtFile.Text), nameof(viewModel.FileName), viewModel);
            CommandBindings.AddPropertyBinding(rbAsImage, nameof(RadioButton.Checked), nameof(viewModel.FileAsImage), viewModel);
            CommandBindings.AddPropertyBinding(rbAsBitmap, nameof(RadioButton.Checked), nameof(viewModel.FileAsBitmap), viewModel);
            CommandBindings.AddPropertyBinding(rbAsMetafile, nameof(RadioButton.Checked), nameof(viewModel.FileAsMetafile), viewModel);
            CommandBindings.AddPropertyBinding(rbAsIcon, nameof(RadioButton.Checked), nameof(viewModel.FileAsIcon), viewModel);

            CommandBindings.AddPropertyBinding(chbAsReadOnly, nameof(CheckBox.Checked), nameof(viewModel.AsReadOnly), viewModel);

            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.AsImageEnabled), nameof(chbAsImage.Enabled), chbAsImage);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PixelFormatEnabled), nameof(cmbPixelFormat.Enabled), cmbPixelFormat);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.ImageFromFile), nameof(gbFile.Enabled), gbFile);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.AsReadOnlyEnabled), nameof(chbAsReadOnly.Enabled), chbAsReadOnly);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.CanDebug), nameof(Button.Enabled), btnViewDirect, btnViewByClassicDebugger, btnViewByExtensionDebugger);
            CommandBindings.AddPropertyBinding(viewModel, nameof(viewModel.PreviewImage), nameof(pictureBox.Image), pictureBox);

            CommandBindings.Add<EventArgs>(OnSelectFileCommand)
                .AddSource(txtFile, nameof(txtFile.Click))
                .AddSource(txtFile, nameof(txtFile.DoubleClick));
            CommandBindings.Add(viewModel.DirectViewCommand).AddSource(btnViewDirect, nameof(btnViewDirect.Click));
            CommandBindings.Add(viewModel.ClassicDebugCommand).AddSource(btnViewByClassicDebugger, nameof(btnViewByClassicDebugger.Click));
            CommandBindings.Add(viewModel.ExtensionDebugCommand).AddSource(btnViewByExtensionDebugger, nameof(btnViewByExtensionDebugger.Click));

            viewModel.GetHwndCallback = () => Handle;
            viewModel.GetClipCallback = () => pictureBox.Bounds;

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
