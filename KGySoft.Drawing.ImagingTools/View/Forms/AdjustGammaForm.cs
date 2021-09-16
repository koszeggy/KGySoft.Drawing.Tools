#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustGammaViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AdjustGammaForm : AdjustColorsFormBase
    {
        #region Constructors

        #region Internal Constructors

        internal AdjustGammaForm(AdjustGammaViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private AdjustGammaForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion
    }
}