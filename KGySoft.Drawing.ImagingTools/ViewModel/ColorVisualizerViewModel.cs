#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ColorVisualizerViewModel.cs
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

using System.Collections.Generic;
using System.Drawing;
using System.Text;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class ColorVisualizerViewModel : ViewModelBase<Color>
    {
        #region Fields

        private static Dictionary<int, string>? knownColors;
        private static Dictionary<int, string>? systemColors;

        #endregion

        #region Properties

        #region Static Properties

        private static Dictionary<int, string> KnownColors
        {
            get
            {
                if (knownColors == null)
                {
                    knownColors = new Dictionary<int, string> { { 0, nameof(Color.Empty) } };

                    // non-system known colors: 27..168 (Transparent..YellowGreen)
                    for (KnownColor color = (KnownColor)27; color <= (KnownColor)167; color++)
                    {
                        int argb = Color.FromKnownColor(color).ToArgb();
                        if (knownColors.TryGetValue(argb, out var name))
                            knownColors[argb] = name + ", " + Enum<KnownColor>.ToString(color);
                        else
                            knownColors.Add(argb, Enum<KnownColor>.ToString(color));
                    }
                }

                return knownColors;
            }
        }

        private static Dictionary<int, string> SystemColors
        {
            get
            {
                if (systemColors == null)
                {
                    systemColors = new Dictionary<int, string>();

                    // system colors: 1.. 174, except 27..168
                    for (KnownColor color = (KnownColor)1; color <= (KnownColor)174; color++)
                    {
                        if (color == (KnownColor)27)
                            color = (KnownColor)168;
                        int argb = Color.FromKnownColor(color).ToArgb();
                        if (systemColors.TryGetValue(argb, out var name))
                            systemColors[argb] = name + ", " + Enum<KnownColor>.ToString(color);
                        else
                            systemColors.Add(argb, Enum<KnownColor>.ToString(color));
                    }
                }

                return systemColors;
            }
        }

        #endregion

        #region Instance Properties
        
        internal Color Color { get => Get<Color>(); set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }
        internal int? SelectedIndex { get => Get<int?>(); set => Set(value); }
        internal string? InfoText { get => Get<string?>(); set => Set(value); }
        internal string? TitleCaption { get => Get<string?>(); set => Set(value); }
        internal KeyValuePair<string, string>[]? CustomColorComponents { get => Get<KeyValuePair<string, string>[]>(); set => Set(value); }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        private static string GetKnownColor(Color color) => KnownColors.GetValueOrDefault(color.ToArgb(), "–")!;
        private static string GetSystemColors(Color color) => SystemColors.GetValueOrDefault(color.ToArgb(), "–")!;

        #endregion

        #region Instance Methods

        #region Public Methods

        public override Color GetEditedModel() => Color;
        public override bool TrySetModel(Color model) => TryInvokeSync(() => Color = model);

        #endregion

        #region Internal Methods

        internal void ResetSystemColors()
        {
            systemColors = null;
            UpdateInfo();
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case (nameof(Color)):
                case (nameof(SelectedIndex)):
                    UpdateInfo();
                    break;
            }
        }

        protected virtual void UpdateInfo()
        {
            Color color = Color;
            TitleCaption = Res.TitleColor(color.Name);
            var sb = new StringBuilder();
            if (SelectedIndex is int index)
                sb.AppendLine(Res.InfoSelectedIndex(index));

            sb.Append(Res.InfoColor(color.ToArgb(), GetKnownColor(color), GetSystemColors(color), color.GetHue(), color.GetSaturation() * 100f, color.GetBrightness() * 100f));
            InfoText = sb.ToString();
        }

        protected override void ApplyDisplayLanguage() => UpdateInfo();

        #endregion

        #endregion

        #endregion
    }
}