#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorVisualizerViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Linq;
using System.Text;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomColorVisualizerViewModel : ColorVisualizerViewModel
    {
        #region Fields

        private readonly CustomColorInfo colorInfo;

        #endregion

        #region Constructors

        internal CustomColorVisualizerViewModel(CustomColorInfo colorInfo)
        {
            this.colorInfo = colorInfo;
            ReadOnly = true;
            Color = colorInfo.DisplayColor.ToColor();
        }

        #endregion

        #region Methods

        protected override void UpdateInfo()
        {
            TitleCaption = Res.TitleType(colorInfo.Type ?? nameof(System.Drawing.Color));

            var sb = new StringBuilder();
            if (SelectedIndex is int index)
                sb.AppendLine(Res.InfoSelectedIndex(index));

            if (colorInfo.Type is string type)
                sb.AppendLine(Res.TitleType(type));
            if (colorInfo.Name is string name)
                sb.AppendLine(Res.TitleColor(name));

            if (colorInfo.CustomAttributes.Count > 0)
            {
                sb.AppendLine();
                sb.Append(Res.InfoCustomProperties(colorInfo.CustomAttributes.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine)));
            }

            InfoText = sb.ToString();
        }

        #endregion
    }
}
