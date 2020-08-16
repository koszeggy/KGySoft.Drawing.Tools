#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerSelectorControl.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
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
using System.Diagnostics.CodeAnalysis;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class QuantizerSelectorControl : MvvmBaseUserControl<QuantizerSelectorViewModel>
    {
        #region Fields

        private IQuantizer quantizer;

        #endregion

        #region Events

        internal event EventHandler QuantizerChanged
        {
            add => Events.AddHandler(nameof(QuantizerChanged), value);
            remove => Events.RemoveHandler(nameof(QuantizerChanged), value);
        }

        #endregion

        #region Properties

        internal IQuantizer Quantizer => quantizer;

        #endregion

        #region Constructors

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose(bool)")]
        public QuantizerSelectorControl() : base(new QuantizerSelectorViewModel())
        {
            InitializeComponent();
            if (DesignMode)
                return;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyResources()
        {
            base.ApplyResources();
            //Icon = Properties.Resources.Palette;
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            InitCommandBindings();
            base.ApplyViewModel();
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
                ViewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // will not change so not as an actual binding
            cmbQuantizer.DataSource = ViewModel.Quantizers;

            // VM.Parameters -> pgParameters.SelectedObject
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Parameters), nameof(pgParameters.SelectedObject), pgParameters);

            // cmbQuantizer.SelectedValue -> VM.SelectedQuantizer
            CommandBindings.AddPropertyBinding(cmbQuantizer, nameof(cmbQuantizer.SelectedValue), nameof(ViewModel.SelectedQuantizer), ViewModel);
        }

        private void InitCommandBindings()
        {
            //CommandBindings.Add(ViewModel.SelectFolderCommand, ViewModel.SelectFolderCommandState)
            //    .AddSource(tbPath, nameof(tbPath.DoubleClick));
            //CommandBindings.Add(ViewModel.InstallCommand, ViewModel.InstallCommandState)
            //    .AddSource(btnInstall, nameof(btnInstall.Click));
            //CommandBindings.Add(ViewModel.RemoveCommand, ViewModel.RemoveCommandState)
            //    .AddSource(btnRemove, nameof(btnRemove.Click));
        }

        private void OnQuantizerChanged(EventArgs e) => (Events[nameof(QuantizerChanged)] as EventHandler)?.Invoke(this, e);

        #endregion

        #endregion
    }
}
