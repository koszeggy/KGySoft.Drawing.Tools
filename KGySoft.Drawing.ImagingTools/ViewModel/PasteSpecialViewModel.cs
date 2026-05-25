#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PasteSpecialViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
#if NETFRAMEWORK
using System.Linq;
#endif

using KGySoft.ComponentModel;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class PasteSpecialViewModel : ViewModelBase<(string? Format, bool CustomAlphaDetection)>
    {
        #region Fields

        private readonly AllowedImageTypes allowedImageTypes;

        #endregion

        #region Properties

        internal string[] Formats { get => Get(Reflector.EmptyArray<string>()); set => Set(value); }
        internal string? SelectedFormat { get => Get<string?>(); set => Set(value); }
        internal bool CustomAlphaDetection { get => Get<bool>(); set => Set(value); }

        #endregion

        #region Constructors

        internal PasteSpecialViewModel(AllowedImageTypes allowedImageTypes)
        {
            Debug.Assert(allowedImageTypes != AllowedImageTypes.None);
            this.allowedImageTypes = allowedImageTypes;
            ClipboardHelper.ClipboardChanged += ClipboardHelper_ClipboardChanged;
            SelectedFormat = Configuration.PreferredClipboardFormat;
            CustomAlphaDetection = Configuration.TryDetectClipboardAlpha;
            ResetFormats();
        }

        #endregion

        #region Methods

        #region Public Methods

        public override (string? Format, bool CustomAlphaDetection) GetEditedModel() => ClosedWithAccept ? (SelectedFormat, CustomAlphaDetection) : default;

        #endregion

        #region Internal Methods

        internal override void ViewUnloading()
        {
            SetModified(ClosedWithAccept);
            if (!ClosedWithAccept)
                return;
            Configuration.PreferredClipboardFormat = SelectedFormat;
            Configuration.TryDetectClipboardAlpha = CustomAlphaDetection;
            Configuration.SaveSettings();
        }

        #endregion

        #region Protected Methods

        protected override bool AffectsModifiedState(string propertyName) => false; // set explicitly

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Formats):
                    string[] formats = Formats;
                    if (formats.Length == 0)
                    {
                        CloseViewCallback?.Invoke();
                        return;
                    }

                    if (SelectedFormat is string format && !Formats.Contains(format))
                        SelectedFormat = null;
                    break;

                case nameof(SelectedFormat):
                    AcceptWithCloseCommandState.Enabled = e.NewValue is not null;
                    break;
            }

            base.OnPropertyChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            ClipboardHelper.ClipboardChanged -= ClipboardHelper_ClipboardChanged;
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void ResetFormats() => Formats = ClipboardHelper.GetImageFormats(allowedImageTypes);

        #endregion

        #region Event handlers

        private void ClipboardHelper_ClipboardChanged(object sender, EventArgs e) => ResetFormats();

        #endregion

        #endregion
    }
}
