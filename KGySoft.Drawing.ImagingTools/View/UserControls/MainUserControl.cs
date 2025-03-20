#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MainUserControl.cs
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

using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal partial class MainUserControl : ImageVisualizerControl
    {
        #region Fields

        private ParentViewProperties? parentProperties;

        #endregion

        #region Properties

        internal override ParentViewProperties ParentViewProperties => parentProperties ??= new ParentViewProperties
        {
            BorderStyle = FormBorderStyle.Sizable,
            Icon = Properties.Resources.ImagingTools,
            MinimumSize = new Size(200, 200),
        };

        internal new DefaultViewModel ViewModel => (DefaultViewModel)base.ViewModel!;

        #endregion

        #region Constructors

        #region Internal Constructors

        internal MainUserControl(DefaultViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
            buttons.Visible = false;
        }

        #endregion

        #region Private Constructors

        private MainUserControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
