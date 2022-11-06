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

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomColorVisualizerViewModel : ColorVisualizerViewModel
    {
        #region Properties

        internal CustomColorInfo? CustomColorInfo { get => Get<CustomColorInfo?>(); init => Set(value); }

        #endregion

        #region Constructors

        internal CustomColorVisualizerViewModel()
        {
            ReadOnly = true;
        }

        #endregion

        #region Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(CustomColorInfo))
            {
                var info = (CustomColorInfo)e.NewValue!;
                Color = info.DisplayColor.ToColor();
            }
        }

        protected override void UpdateInfo()
        {
            CustomColorInfo? info = CustomColorInfo;
            if (info == null)
            {
                base.UpdateInfo();
                return;
            }

            TitleCaption = Res.TitleType(info.Type ?? nameof(System.Drawing.Color));

            var sb = new StringBuilder();
            if (SelectedIndex is int index)
                sb.AppendLine(Res.InfoSelectedIndex(index));

            if (info.Type is string type)
                sb.AppendLine(Res.TitleType(type));
            if (info.Name is string name)
                sb.AppendLine(Res.TitleColor(name));

            if (info.CustomAttributes.Count > 0)
            {
                sb.AppendLine();
                sb.Append(Res.InfoCustomProperties(info.CustomAttributes.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine)));
            }

            InfoText = sb.ToString();
        }

        #endregion
    }
}
