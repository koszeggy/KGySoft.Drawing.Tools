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
using System.Globalization;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class Images
    {
        #region Fields

        #region Private Fields

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
        private static Bitmap smoothZoom;
        private static Bitmap edit;
        private static Bitmap rotateLeft;
        private static Bitmap rotateRight;

        #endregion

        #region Internal Fields

        internal static readonly Size ReferenceSize = new Size(16, 16);

        #endregion

        #endregion

        #region Properties

        internal static Bitmap Check => check ??= GetResource(nameof(Check));
        internal static Bitmap Crop => crop ??= GetResource(nameof(Crop));
        internal static Bitmap HighlightVisibleClip => highlightVisibleClip ??= GetResource(nameof(HighlightVisibleClip));
        internal static Bitmap Magnifier => magnifier ??= GetResource(nameof(Magnifier));
        internal static Bitmap Save => save ??= GetResource(nameof(Save));
        internal static Bitmap Open => open ??= GetResource(nameof(Open));
        internal static Bitmap Clear => clear ??= GetResource(nameof(Clear));
        internal static Bitmap Prev => prev ??= GetResource(nameof(Prev));
        internal static Bitmap Next => next ??= GetResource(nameof(Next));
        internal static Bitmap Palette => palette ??= GetResource(nameof(Palette));
        internal static Bitmap Settings => settings ??= GetResource(nameof(Settings));
        internal static Bitmap Animation => animation ??= GetResource(nameof(Animation));
        internal static Bitmap MultiSize => multiSize ??= GetResource(nameof(MultiSize));
        internal static Bitmap MultiPage => multiPage ??= GetResource(nameof(MultiPage));
        internal static Bitmap SmoothZoom => smoothZoom ??= GetResource(nameof(SmoothZoom));
        internal static Bitmap Edit => edit ??= GetResource(nameof(Edit));
        internal static Bitmap RotateLeft => rotateLeft ??= GetResource(nameof(RotateLeft));
        internal static Bitmap RotateRight => rotateRight ??= GetResource(nameof(RotateRight));

        #endregion

        #region Methods

        #region Internal Methods

        internal static Bitmap ToScaledBitmap(this Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon), PublicResources.ArgumentNull);
            return icon.ExtractNearestBitmap(ReferenceSize.Scale(OSUtils.SystemScale), PixelFormat.Format32bppArgb);
        }

        internal static Icon ToScaledIcon(this Icon icon, bool legacyScaling = true)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon), PublicResources.ArgumentNull);

            Size size = ReferenceSize.Scale(OSUtils.SystemScale);
            Icon result = icon.ExtractNearestIcon(size, PixelFormat.Format32bppArgb);
            int mod = 0;
            if (!legacyScaling || OSUtils.IsWindows8OrLater || !OSUtils.IsWindows || (mod = result.Width & 0xF) == 0)
                return result;

            // Windows XP-Windows 7 with legacy scaling: we need to make sure that icon size is dividable by 16
            // so it will not be corrupted (eg. ErrorProvider)
            using Bitmap iconImage = icon.ExtractBitmap(result.Size);
            result.Dispose();

            // returning a larger icon without scaling so apparently it will have the same size as the original one
            return iconImage.ToIcon(size.Width + (16 - mod), ScalingMode.NoScaling);
        }

        #endregion

        #region Private Methods

        private static Bitmap GetResource(string resourceName)
        {
            var icon = (Icon)Properties.Resources.ResourceManager.GetObject(resourceName, CultureInfo.InvariantCulture);
            if (OSUtils.IsVistaOrLater)
                return icon.ToMultiResBitmap();

            // In Windows XP the multi resolution bitmap can be ugly if it has not completely transparent pixels
            return icon.ToScaledBitmap();
        }

        #endregion

        #endregion
    }
}
