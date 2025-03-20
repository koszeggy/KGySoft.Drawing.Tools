#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TransformBitmapControlBase.cs
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

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class TransformBitmapControlBase : MvvmBaseUserControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;

        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties
        {
            get
            {
                if (parentProperties == null)
                {
                    parentProperties = new ParentViewProperties();
                    InitParentProperties(parentProperties);
                }

                return parentProperties;
            }
        }

        #endregion

        #region Private Properties

        private new TransformBitmapViewModelBase ViewModel => (TransformBitmapViewModelBase)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Protected Constructors

        protected TransformBitmapControlBase(TransformBitmapViewModelBase viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private TransformBitmapControlBase() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected virtual void InitParentProperties(ParentViewProperties properties)
        {
            properties.HideMinimizeButton = true;
            properties.AcceptButton = okCancelButtons.OKButton;
            properties.CancelButton = okCancelButtons.CancelButton;
            properties.ClosingCallback = (sender, e) =>
            {
                // if user (or system) closes the window without pressing cancel we need to execute the cancel command
                if (((Form)sender).DialogResult != DialogResult.OK && e.CloseReason != CloseReason.None)
                    okCancelButtons.CancelButton.PerformClick();
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsLoaded)
            {
                ErrorProvider.SetIconAlignment(previewImage.ImageViewer, ErrorIconAlignment.MiddleLeft);
                ValidationMapping[nameof(ViewModel.PreviewImageViewModel.PreviewImage)] = previewImage.ImageViewer;
            }

            base.OnLoad(e);
        }

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Alt | Keys.S:
                    previewImage.SmoothZooming = !previewImage.SmoothZooming;
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            // ViewModel commands
            CommandBindings.Add(ViewModel.ApplyCommand, ViewModel.ApplyCommandState)
                .AddSource(okCancelButtons.OKButton, nameof(okCancelButtons.OKButton.Click));
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(okCancelButtons.CancelButton, nameof(okCancelButtons.CancelButton.Click));

            // View commands
            CommandBindings.Add(ValidationResultsChangedCommand)
                .AddSource(ViewModel, nameof(ViewModel.ValidationResultsChanged))
                .WithParameter(() => ViewModel.ValidationResults);
        }

        private void InitPropertyBindings()
        {
            // simple initializations rather than bindings because these will not change:
            previewImage.ViewModel = ViewModel.PreviewImageViewModel;

            // VM.IsGenerating -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsGenerating), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }

        #endregion

        #endregion
    }
}
