#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Images.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Images
    {
        #region Fields

        private static readonly Size referenceSize = new Size(16, 16);

        private static Bitmap check;
        private static Bitmap crop;
        private static Bitmap highlightVisibleClip;
        private static Bitmap magnifier;
        private static Bitmap save;
        private static Bitmap open;
        private static Bitmap clear;
        private static Bitmap prev;
        private static Bitmap next;
        private static Bitmap palette;
        private static Bitmap settings;
        private static Bitmap animation;
        private static Bitmap multiSize;
        private static Bitmap multiPage;

        #endregion

        #region Properties

        internal static Bitmap Check => check ?? (check = GetResource(nameof(Check)));
        internal static Bitmap Crop => crop ?? (crop = GetResource(nameof(Crop)));
        internal static Bitmap HighlightVisibleClip => highlightVisibleClip ?? (highlightVisibleClip = GetResource(nameof(HighlightVisibleClip)));
        internal static Bitmap Magnifier => magnifier ?? (magnifier = GetResource(nameof(Magnifier)));
        internal static Bitmap Save => save ?? (save = GetResource(nameof(Save)));
        internal static Bitmap Open => open ?? (open = GetResource(nameof(Open)));
        internal static Bitmap Clear => clear ?? (clear = GetResource(nameof(Clear)));
        internal static Bitmap Prev => prev ?? (prev = GetResource(nameof(Prev)));
        internal static Bitmap Next => next ?? (next = GetResource(nameof(Next)));
        internal static Bitmap Palette => palette ?? (palette = GetResource(nameof(Palette)));
        internal static Bitmap Settings => settings ?? (settings = GetResource(nameof(Settings)));
        internal static Bitmap Animation => animation ?? (animation = GetResource(nameof(Animation)));
        internal static Bitmap MultiSize => multiSize ?? (multiSize = GetResource(nameof(MultiSize)));
        internal static Bitmap MultiPage => multiPage ?? (multiPage = GetResource(nameof(MultiPage)));

        #endregion

        #region Methods

        #region Internal Methods

        internal static Bitmap ToScaledBitmap(this Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon), PublicResources.ArgumentNull);
            return icon.ExtractNearestBitmap(referenceSize.Scale(WindowsUtils.SystemScale), PixelFormat.Format32bppArgb);
        }

        #endregion

        #region Private Methods

        private static Bitmap GetResource(string resourceName)
        {
            var icon = (Icon)Properties.Resources.ResourceManager.GetObject(resourceName);
            if (WindowsUtils.IsVistaOrLater)
                return icon.ToMultiResBitmap();

            // In Windows XP the multi resolution bitmap can be ugly if it has not completely transparent pixels
            return icon.ToScaledBitmap();
        }

        #endregion

        #endregion
    }
}
