#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolTip.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Draw -= AdvancedToolTip_Draw;
                Res.DisplayLanguageChanged -= Res_DisplayLanguageChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            Res.DisplayLanguageChanged += Res_DisplayLanguageChanged;
            Draw += AdvancedToolTip_Draw;
            ResetOwnerDraw();
        }

        private void ResetOwnerDraw() => OwnerDraw = Res.DisplayLanguage.TextInfo.IsRightToLeft;

        #endregion

        #region Event Handlers

        private void Res_DisplayLanguageChanged(object? sender, EventArgs e) => ResetOwnerDraw();

        #endregion

        #endregion
    }
}
