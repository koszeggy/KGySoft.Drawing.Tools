#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedToolTip.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.ComponentModel;
using System.Drawing;
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

        public AdvancedToolTip()
        {
            OwnerDraw = true;
            Draw += AdvancedToolTip_Draw;
        }

        public AdvancedToolTip(IContainer container) : base(container)
        {
            OwnerDraw = true;
            Draw += AdvancedToolTip_Draw;
        }

        #endregion

        #region Methods

        #region Static Methods

        private static void AdvancedToolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            // same as DrawBackground but will not recreate the brush again and again
            e.Graphics.FillRectangle(SystemBrushes.Info, e.Bounds);
            e.DrawBorder();

            var flags = TextFormatFlags.HidePrefix | TextFormatFlags.VerticalCenter;
            if (LanguageSettings.DisplayLanguage.TextInfo.IsRightToLeft)
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;

            e.DrawText(flags);
        }

        #endregion

        #region Instance Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Draw -= AdvancedToolTip_Draw;

            base.Dispose(disposing);
        }

        #endregion

        #endregion
    }
}
