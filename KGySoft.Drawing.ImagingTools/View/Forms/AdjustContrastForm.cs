#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustContrastForm.cs
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
    internal partial class AdjustContrastForm : AdjustColorsFormBase
    {
        #region Constructors

        #region Internal Constructors

        internal AdjustContrastForm(AdjustContrastViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private AdjustContrastForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion
    }
}