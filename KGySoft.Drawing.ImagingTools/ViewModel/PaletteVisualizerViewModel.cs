#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PaletteVisualizerViewModel.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class PaletteVisualizerViewModel : ViewModelBase, IViewModel<Color[]>
    {
        #region Fields

        private ICommandBinding? selectedColorEditedBinding;

        #endregion

        #region Properties

        #region Internal Properties

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - not cloning if value is null
        internal Color[] Palette { get => Get<Color[]>(); init => Set(value?.Clone() ?? throw new ArgumentNullException(nameof(value), PublicResources.ArgumentNull)); }
        internal int Count { get => Get<int>(); private set => Set(value); }
        internal bool ReadOnly { get => Get<bool>(); set => Set(value); }
        internal int SelectedColorIndex { get => Get(-1); set => Set(value); }
        internal ColorVisualizerViewModel? SelectedColorViewModel { get => Get<ColorVisualizerViewModel?>(); private set => Set(value); }
        internal Color SelectedColor { get => Get<Color>(); private set => Set(value); }
        internal string? Type { get => Get<string?>(); set => Set(value); }
        internal string? TitleCaption { get => Get(() => Res.TitlePaletteCount(Count)); set => Set(value); }
        
        #endregion

        #region Protected Properties

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            base.ViewLoaded();
            if (Palette.Length == 0)
            {
                ReadOnly = true;
                ShowInfo(Res.InfoMessagePaletteEmpty);
                CloseViewCallback?.Invoke();
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);

            switch (e.PropertyName)
            {
                case nameof(Type):
                    if (e.NewValue is string type)
                        TitleCaption = $"{Res.TitleType(type)}{Res.TextSeparator}{Res.TitleColorCount(Count)}";
                    break;

                case nameof(Palette):
                    var palette = (IList<Color>)e.NewValue!;
                    Count = palette.Count;
                    if (palette.Count == 0)
                        return;

                    SelectedColorIndex = 0;
                    break;

                case nameof(SelectedColorIndex):
                    int index = (int)e.NewValue!;
                    if (Count == 0)
                        break;
                    SelectedColorViewModel = GetSelectedColorViewModel(index);
                    break;

                case nameof(SelectedColorViewModel):
                    (e.OldValue as ColorVisualizerViewModel)?.Dispose();
                    selectedColorEditedBinding?.Dispose();
                    if (!ReadOnly && e.NewValue is ColorVisualizerViewModel newValue)
                    {
                        selectedColorEditedBinding = newValue.CreatePropertyChangedHandlerBinding(OnSelectedColorEditedCommand, nameof(SelectedColorViewModel.Color));
                        SelectedColor = newValue.Color;
                    }
                    break;

                case nameof(ReadOnly):
                    if (SelectedColorViewModel is { } selectedColorViewModel)
                        selectedColorViewModel.ReadOnly = ReadOnly;
                    break;
            }
        }

        protected virtual ColorVisualizerViewModel GetSelectedColorViewModel(int index) => new ColorVisualizerViewModel
        {
            SelectedIndex = index,
            Color = Palette[index],
            ReadOnly = ReadOnly,
        };

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                selectedColorEditedBinding?.Dispose();
                SelectedColorViewModel?.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Command Handlers

        private void OnSelectedColorEditedCommand()
        {
            Color color = SelectedColorViewModel!.Color;
            SelectedColor = color;
            Palette[SelectedColorIndex] = color;
            SetModified(true);
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Color[] IViewModel<Color[]>.GetEditedModel() => Palette;

        #endregion

        #endregion
    }
}
