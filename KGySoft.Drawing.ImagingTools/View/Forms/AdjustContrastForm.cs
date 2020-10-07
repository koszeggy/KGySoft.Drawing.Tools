#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustContrastForm.cs
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

        private AdjustContrastForm() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion
    }
}