﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustBrightnessControl.cs
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

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    internal sealed partial class AdjustBrightnessControl : AdjustColorsControlBase
    {
        #region Constructors

        #region Internal Constructors

        internal AdjustBrightnessControl(AdjustBrightnessViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
            BackColor = Color.Transparent; // to make the resize grip in the parent form visible
        }

        #endregion

        #region Private Constructors

        private AdjustBrightnessControl() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion
    }
}
