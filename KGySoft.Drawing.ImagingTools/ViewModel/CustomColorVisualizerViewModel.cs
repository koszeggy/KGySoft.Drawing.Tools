#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomColorVisualizerViewModel.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal sealed class CustomColorVisualizerViewModel : ColorVisualizerViewModel, IViewModel<CustomColorInfo?>
    {
        #region Fields

        private CustomColorInfo? colorInfo;

        #endregion

        #region Properties

        internal string? Type { get; set; }

        #endregion

        #region Constructors

        internal CustomColorVisualizerViewModel(CustomColorInfo? colorInfo)
        {
            ReadOnly = true;
            ResetColorInfo(colorInfo);
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void UpdateInfo()
        {
            TitleCaption = Res.TitleType(Type ?? nameof(System.Drawing.Color));

            var sb = new StringBuilder();
            if (SelectedIndex is int index)
                sb.AppendLine(Res.InfoSelectedIndex(index));

            if (Type is string type)
                sb.AppendLine(Res.TitleType(type));
            if (colorInfo?.Name is string name)
                sb.AppendLine(Res.TitleColor(name));

            if (colorInfo?.CustomAttributes.Count > 0 || colorInfo?.CustomColorComponents is { Length: > 4 } )
            {
                var attrs = new List<KeyValuePair<string, string>>(colorInfo.CustomAttributes);
                if (colorInfo.CustomColorComponents is { Length: > 4 })
                    attrs.InsertRange(0, colorInfo.CustomAttributes);
                sb.AppendLine();
                sb.Append(Res.InfoCustomProperties(attrs.Select(a => $"{a.Key}: {a.Value}").Join(Environment.NewLine)));
            }

            InfoText = sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            colorInfo = null;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetColorInfo(CustomColorInfo? model)
        {
            colorInfo = model;
            Type = colorInfo?.Type;
            Color = colorInfo?.DisplayColor.ToColor() ?? default;
            CustomColorComponents = colorInfo?.CustomColorComponents;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        CustomColorInfo? IViewModel<CustomColorInfo?>.GetEditedModel() => null; // read-only
        bool IViewModel<CustomColorInfo?>.TrySetModel(CustomColorInfo? model) => TryInvokeSync(() => ResetColorInfo(model));

        #endregion

        #endregion
    }
}
