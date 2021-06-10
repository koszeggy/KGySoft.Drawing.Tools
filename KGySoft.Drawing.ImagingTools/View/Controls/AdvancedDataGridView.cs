#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedDataGridView.cs
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

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just a DataGridView that
    /// - provides some default style with a few fixed issues
    /// - scales the columns automatically
    /// </summary>
    internal class AdvancedDataGridView : DataGridView
    {
        #region Fields

        private readonly DataGridViewCellStyle defaultDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultColumnHeadersDefaultCellStyle;
        private readonly DataGridViewCellStyle defaultAlternatingRowsDefaultCellStyle;

        #endregion

        #region Properties

        // these are reintroduced just for the ShouldSerialize... methods
        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle DefaultCellStyle
        {
            get => base.DefaultCellStyle;
            set => base.DefaultCellStyle = value; 
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle ColumnHeadersDefaultCellStyle
        {
            get => base.ColumnHeadersDefaultCellStyle;
            set => base.ColumnHeadersDefaultCellStyle = value;
        }

        [AmbientValue(null)]
        [AllowNull]
        public new DataGridViewCellStyle AlternatingRowsDefaultCellStyle
        {
            get => base.AlternatingRowsDefaultCellStyle;
            set => base.AlternatingRowsDefaultCellStyle = value;
        }

        #endregion

        #region Constructors

        public AdvancedDataGridView()
        {
            DefaultCellStyle = defaultDefaultCellStyle = new DataGridViewCellStyle(DefaultCellStyle)
            {
                // Base default uses Window back color with ControlText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
            };

            ColumnHeadersDefaultCellStyle = defaultColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                // Base default uses Control back color with WindowText fore color. Most cases it's not an issue unless Window/Control colors are close to inverted.
                BackColor = SystemColors.Control,
                ForeColor = SystemColors.ControlText,
            };

            AlternatingRowsDefaultCellStyle = defaultAlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SystemColors.ControlLight,
                ForeColor = SystemColors.ControlText,
            };
        }

        #endregion

        #region Methods

        #region Protected Methods
        
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            AdjustAlternatingRowsColors();
        }

        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            AdjustAlternatingRowsColors();
            AlternatingRowsDefaultCellStyle = SystemInformation.HighContrast
                ? null
                : new DataGridViewCellStyle { BackColor = SystemColors.ControlLight, ForeColor = SystemColors.ControlText };
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            if (factor.Width.Equals(1f))
                return;
            foreach (DataGridViewColumn column in Columns)
                column.Width = (int)(column.Width * factor.Width);
        }

        #endregion

        #region Private Methods
        
        private void AdjustAlternatingRowsColors()
        {
            if (!Equals(AlternatingRowsDefaultCellStyle, defaultAlternatingRowsDefaultCellStyle))
                return;

            AlternatingRowsDefaultCellStyle = SystemInformation.HighContrast
                ? null
                : defaultAlternatingRowsDefaultCellStyle;
        }

        private bool ShouldSerializeAlternatingRowsDefaultCellStyle() => !Equals(AlternatingRowsDefaultCellStyle, defaultAlternatingRowsDefaultCellStyle);
        private bool ShouldSerializeColumnHeadersDefaultCellStyle() => !Equals(ColumnHeadersDefaultCellStyle, defaultColumnHeadersDefaultCellStyle);
        private bool ShouldSerializeDefaultCellStyle() => !Equals(DefaultCellStyle, defaultDefaultCellStyle);

        #endregion

        #endregion
    }
}
