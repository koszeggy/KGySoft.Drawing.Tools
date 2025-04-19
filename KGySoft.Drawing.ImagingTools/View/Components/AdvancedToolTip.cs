#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolTip.cs
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
using System.ComponentModel;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// A ToolTip that supports RTL correctly
    /// </summary>
    /// <seealso cref="ToolTip" />
    internal class AdvancedToolTip : ToolTip
    {
        #region Constructors

        public AdvancedToolTip() => Initialize();

        public AdvancedToolTip(IContainer container) : base(container) => Initialize();

        #endregion

        #region Methods

        #region Static Methods

        private static void AdvancedToolTip_Draw(object? sender, DrawToolTipEventArgs e) => e.DrawToolTipAdvanced();

        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void ResetAppearance() => OwnerDraw = Res.DisplayLanguage.TextInfo.IsRightToLeft
            || ThemeColors.IsSet(ThemeColor.ToolTip) || ThemeColors.IsSet(ThemeColor.ToolTipBorder) || ThemeColors.IsSet(ThemeColor.ToolTipText);

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            Draw -= AdvancedToolTip_Draw;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            Draw += AdvancedToolTip_Draw;
            ResetAppearance();
        }

        #endregion

        #endregion

        #endregion
    }
}
