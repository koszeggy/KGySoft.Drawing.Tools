#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CountColorsControl.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class CountColorsControl : MvvmBaseUserControl
    {
        #region Constants

        // Adjusted for Segoe UI 9 font on 100% DPI
        private const int pnlButtonRefHeight = 33;

        #endregion

        #region Fields

        #region Static Fields

        private static readonly Size referenceSize = new Size(320, 100); // Adjusted for Segoe UI 9 font on 100% DPI.
        private static readonly Size buttonReferenceSize = new Size(75, 23);
        private static readonly Padding buttonReferenceMargin = new Padding(3);
        private static readonly Padding panelReferencePadding = new Padding(3);

        #endregion

        #region Instance Fields

        private ParentViewProperties? parentProperties;
        
        
        #endregion
        #endregion

        #region Properties

        #region Internal Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.FixedDialog,
            Icon = Properties.Resources.Palette,
            AcceptButton = btnClose,
            CancelButton = btnClose,
            ClosingCallback = (_,_) => ViewModel.CancelIfRunning()
        };

        #endregion

        #region Private Properties

        private new CountColorsViewModel ViewModel => (CountColorsViewModel)base.ViewModel!;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal CountColorsControl(CountColorsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private CountColorsControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        internal override Size? GetDesiredSize(PointF scale) => referenceSize.Scale(scale);

        internal override void AdjustSizes(PointF? dynamicSizesScale)
        {
            base.AdjustSizes(dynamicSizesScale);
            SuspendLayout();
            try
            {
                PointF scale = this.GetScale();
                pnlButton.Height = pnlButtonRefHeight.Scale(scale.Y);
                progress.AdjustSizes();

                Size minSize = buttonReferenceSize.Scale(scale);
                Padding margin = buttonReferenceMargin.Scale(scale);

                pnlButton.Padding = panelReferencePadding.Scale(scale);
                pnlButton.Height = minSize.Height + pnlButton.Padding.Vertical + margin.Vertical;

                btnClose.MinimumSize = minSize;
                btnClose.Size = btnClose.GetPreferredSize(new Size(0, minSize.Height));
                btnClose.Location = new Point((pnlButton.Width - btnClose.Width) / 2, (pnlButton.Height - btnClose.Height) / 2);
            }
            finally
            {
                ResumeLayout();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            // Cancelling here may be required if this control is not parented by an MvvmParentForm
            ViewModel.CancelIfRunning();
            parentProperties = null;
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            CommandBindings.Add(ViewModel.CancelCommand)
                .AddSource(btnClose, nameof(btnClose.Click));
        }

        private void InitPropertyBindings()
        {
            // VM.DisplayText <-> lblCountColorsStatus.Text
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.DisplayText), nameof(lblCountColorsStatus.Text), lblCountColorsStatus);

            // VM.IsProcessing -> progress.ProgressVisible
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.IsProcessing), nameof(progress.ProgressVisible), progress);

            // VM.Progress -> progress.Progress
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.Progress), nameof(progress.Progress), progress);
        }

        #endregion

        #endregion
    }
}
