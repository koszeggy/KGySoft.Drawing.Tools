#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ClipboardHelper.cs
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

#region Used Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;

#if NETFRAMEWORK
using KGySoft.CoreLibraries;
#endif
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;

#endregion

#region Used Aliases

using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using IWinFormsDataObject = System.Windows.Forms.IDataObject;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ClipboardHelper
    {
        #region Nested Classes

        private sealed class ClipboardListener : Control
        {
            #region Fields

            private IntPtr nextListener;

            #endregion

            #region Constructors

            internal ClipboardListener() => CreateHandle();

            #endregion

            #region Methods

            protected override void CreateHandle()
            {
                base.CreateHandle();
                Debug.Assert(IsHandleCreated);
                if (OSHelper.IsWindowsVistaOrLater)
                {
                    if (User32.AddClipboardFormatListener(Handle))
                        return;
                }

                // Fallback to legacy solution (works also on Windows XP)
                nextListener = User32.SetClipboardViewer(Handle);
            }

            protected override void DestroyHandle()
            {
                Debug.Assert(IsHandleCreated);
                if (OSHelper.IsWindowsVistaOrLater && nextListener == IntPtr.Zero)
                    User32.RemoveClipboardFormatListener(Handle);
                else
                {
                    User32.ChangeClipboardChain(Handle, nextListener);
                    nextListener = IntPtr.Zero;
                }

                base.DestroyHandle();
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case Constants.WM_CLIPBOARDUPDATE:
                        try
                        {
                            OnClipboardChanged();
                        }
                        catch (Exception e) when (!e.IsCriticalGdi())
                        {
                        }
                        finally
                        {
                            m.Result = IntPtr.Zero;
                        }

                        break;

                    case Constants.WM_DRAWCLIPBOARD:
                        try
                        {
                            OnClipboardChanged();
                        }
                        catch (Exception e) when (!e.IsCriticalGdi())
                        {
                        }
                        finally
                        {
                            if (nextListener != IntPtr.Zero)
                                User32.SendMessage(nextListener, Constants.WM_DRAWCLIPBOARD, m.WParam, m.LParam);
                        }

                        break;

                    case Constants.WM_CHANGECBCHAIN:
                        // If the next window is closing, repairing the chain
                        if (m.WParam == nextListener)
                            nextListener = m.LParam;
                        // Otherwise, passing the message to the next link
                        else if (nextListener != IntPtr.Zero)
                            User32.SendMessage(nextListener, Constants.WM_CHANGECBCHAIN, m.WParam, m.LParam);
                        break;

                    default:
                        base.WndProc(ref m);
                        return;
                }
            }

            #endregion
        }

        #endregion

        #region Constants

        private const string emfFormat = "EMF";
        private const string wmfFormat = "WMF";
        private const string gifFormat = "GIF";
        private const string pngFormat = "PNG";
        private const string iconFormat = "ICO";
        private const string tiffFormat = "TIFF";

        #endregion

        #region Fields

        private readonly static Lock syncRoot = new();
        private static readonly byte[] binaryFormatterStreamPrefix = new Guid("FD9EA796-3B13-4370-A679-56106BB288FB").ToByteArray();

        private static EventHandler? clipboardChangedHandler;
        private static ClipboardListener? clipboardViewer;

        #endregion

        #region Events

        internal static event EventHandler ClipboardChanged
        {
            add
            {
                if (value == null!)
                    return;

                lock (syncRoot)
                {
                    if (clipboardChangedHandler == null)
                    {
                        Debug.Assert(clipboardViewer == null);
                        clipboardViewer = new ClipboardListener();
                    }

                    clipboardChangedHandler += value;
                }
            }
            remove
            {
                if (value == null!)
                    return;

                lock (syncRoot)
                {
                    clipboardChangedHandler -= value;
                    if (clipboardChangedHandler != null)
                        return;
                    clipboardViewer?.Dispose();
                    clipboardViewer = null;
                }
            }
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal static bool HasFormat(AllowedImageTypes types)
        {
            try
            {
                IWinFormsDataObject? dataObject = Clipboard.GetDataObject();

                // fallback path, on regular windows it should not happen
                if (dataObject == null)
                {
                    Debug.Fail("HasFormat: Clipboard.GetDataObject returned null");
                    return Clipboard.ContainsImage();
                }

                var formats = new HashSet<string>(dataObject.GetFormats());
                if ((types & (AllowedImageTypes.Bitmap | AllowedImageTypes.Icon)) != AllowedImageTypes.None
                    && formats.Overlaps([DataFormats.Bitmap, DataFormats.Dib, typeof(Bitmap).FullName!, pngFormat, gifFormat]))
                {
                    return true;
                }

                if ((types & AllowedImageTypes.Metafile) != AllowedImageTypes.None
                    && formats.Overlaps([DataFormats.EnhancedMetafile, DataFormats.MetafilePict, typeof(Metafile).FullName!, emfFormat, wmfFormat]))
                {
                    return true;
                }

                if ((types & AllowedImageTypes.Icon) != AllowedImageTypes.None && formats.Contains(iconFormat))
                    return true;

                if ((types & AllowedImageTypes.Bitmap) != AllowedImageTypes.None && formats.Contains(tiffFormat))
                    return true;

                return false;
            }
            catch (Exception e) when (!e.IsCritical())
            {
                return false;
            }
        }

        internal static void CopyToClipboard(ImageInfoBase info)
        {
            // NOTE: Using SetImage as a fallback solution only, because it does not support the native metafile formats, and puts a BinaryFormatter
            // entry on the clipboard a with potentially poorly chosen encoder (supporting such formats in TryPasteFromClipboard, though).
            // Instead, preparing the native Bitmap format as a well-known fallback (which is supported by the managed clipboard API as well) explicitly.
            // Value is IntPtr or MemoryStream. Using Stream is better than byte[], because in the managed API Stream is just converted to a byte[],
            // whereas byte[] is serialized in a BinaryFormatter-compatible way.
            var formats = new Dictionary<string, object>();

            // compound image or single frame image
            if (info is ImageInfo imageInfo)
            {
                switch (imageInfo.Type)
                {
                    case ImageInfoType.None:
                        Debug.Fail("Copying expected to be disabled when there is no loaded image");
                        return;

                    case ImageInfoType.SingleImage when imageInfo.Image is Metafile metafile:
                        CopyMetafile(formats, metafile);
                        break;

                    case ImageInfoType.Pages:
                        CopyTiff(formats, imageInfo);
                        break;

                    case ImageInfoType.MultiRes:
                    case ImageInfoType.Icon:
                        CopyIcon(formats, imageInfo);
                        break;

                    case ImageInfoType.Animation:
                        CopyAnimGif(formats, imageInfo);
                        break;
                }
            }

            Image? image = info.Image ?? (info as ImageInfo)?.GetCreateImage();
            Debug.Assert(image != null, "Failed to obtain an image to copy");
            if (image == null)
                return;

            // Standard Bitmap format, potentially along with some custom single-frame formats
            CopyBitmap(formats, image);

            try
            {
                // If we managed to prepare some formats, we place them on the clipboard. If it fails, using Clipboard.SetImage as an ultimate fallback.
                if (formats.Count == 0 || !PopulateClipboard(formats))
                    Clipboard.SetImage(image);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Dialogs.WarningMessage(Res.WarningMessageCannotCopyClipboard);
            }
        }

        internal static bool TryPasteFromClipboard(AllowedImageTypes allowedTypes, bool allowMultiFrame, out ImageInfo? imageInfo)
        {
            Debug.Assert(allowedTypes != AllowedImageTypes.None);
            imageInfo = null;

            try
            {
                // Not using Clipboard.GetImage (or only as a fallback), because it always gets Bitmap format only, which is an RGB32 format with no alpha.
                IWinFormsDataObject? dataObject = Clipboard.GetDataObject();
                if (dataObject == null)
                    return false;

                var formats = new HashSet<string>(dataObject.GetFormats(false));

                // 1. Metafile - custom encodings first, and then by standard native formats
                if (TryGetImageFromStream(formats.Intersect([emfFormat, wmfFormat]), dataObject, allowedTypes, false, out imageInfo))
                    return true;
                if (formats.Contains(DataFormats.EnhancedMetafile) && TryGetNativeEmf(dataObject, allowedTypes, out imageInfo))
                    return true;
                if (formats.Contains(DataFormats.MetafilePict) && TryGetNativeWmf(dataObject, allowedTypes, out imageInfo))
                    return true;

                // 2. Multiframe Bitmap or Icon with custom encoding
                if (allowMultiFrame)
                {
                    // 2.a. TIFF or Icon
                    if (TryGetImageFromStream(formats.Intersect([tiffFormat, iconFormat]), dataObject, allowedTypes, true, out imageInfo))
                        return true;

                    // 2.b. [animated] GIF
                    if (formats.Contains(gifFormat) && TryGetImageFromStream([gifFormat], dataObject, allowedTypes, true, out imageInfo))
                    {
                        // if a single-frame GIF found, accepting it if there is no PNG on the clipboard as well
                        if (imageInfo!.Type == ImageInfoType.Animation || !formats.Contains(pngFormat))
                            return true;

                        // dropping the single frame GIF to obtain the PNG instead
                        imageInfo.Dispose();
                    }
                }

                // 3. Single-frame custom encoded Bitmap or Icon. The way of the Intersect call ensures the order of the tried formats.
                if (TryGetImageFromStream(new[] { pngFormat, tiffFormat, gifFormat, iconFormat }.Intersect(formats), dataObject, allowedTypes, false, out imageInfo))
                    return true;

                // 4. Standard Bitmap format or .NET Framework serialized Image
                if (formats.Contains(DataFormats.Bitmap) && TryGetStandardBitmap(dataObject, allowedTypes, out imageInfo))
                    return true;

                // 5. TODO - DIB

                // 6. Ultimate fallback: Clipboard.GetImage - normally we should not reach this point, but on Mono the lower level interfaces may not be implemented completely.
                imageInfo = EnsureFormat(Clipboard.GetImage(), allowedTypes, allowMultiFrame);
                return imageInfo != null;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Error while trying to paste image from clipboard: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Private Methods

        private static void OnClipboardChanged() => clipboardChangedHandler?.Invoke(null, EventArgs.Empty);

        private static bool PopulateClipboard(Dictionary<string, object> formats)
        {
            bool useNativeApi = OSHelper.IsWindows && (formats.ContainsKey(DataFormats.EnhancedMetafile) || formats.ContainsKey(DataFormats.Bitmap));
            if (useNativeApi && PopulateClipboardNatively(formats))
                return true;

            // Here we could not populate the clipboard natively. As a fallback, we try to use the managed way.
            var dataObject = new DataObject();
            bool result = false;
            foreach (KeyValuePair<string, object> item in formats)
            {
                switch (item.Value)
                {
                    case MemoryStream ms:
                        result = true;
                        dataObject.SetData(item.Key, false, ms);
                        break;

                    // If we have prepared native entries, we discard them, because DataObject.SetData would just corrupt them.
                    // As the clipboard will not take over the ownership of such items now, we must free them to prevent memory leaks.
                    case IntPtr nativeHandle:
                        Debug.Assert(OSHelper.IsWindows);
                        if (item.Key == DataFormats.Bitmap)
                            Gdi32.DeleteObject(nativeHandle);
                        else if (item.Key == DataFormats.EnhancedMetafile)
                            Gdi32.DeleteEnhMetaFile(nativeHandle);
                        else
                            throw new InvalidOperationException(Res.InternalError($"Unhandled native clipboard item to free: {item.Key}"));
                        break;

                    case Image image:
                        Debug.Assert(!OSHelper.IsWindows && item.Key == DataFormats.Bitmap, "Setting an image directly is expected on non-Windows platforms only.");
                        dataObject.SetImage(image);
                        break;

                    default:
                        throw new InvalidOperationException(Res.InternalError($"Unhandled clipboard format to populate in a managed way: {item.Key}: {item.Value.GetType()}"));
                }
            }

            if (result)
                Clipboard.SetDataObject(dataObject, true);
            return result;
        }

        private static bool PopulateClipboardNatively(Dictionary<string, object> formats)
        {
            try
            {
                if (!User32.OpenClipboard())
                    return false;
                try
                {
                    // This is why we cannot mix native and managed APIs: without EmptyClipboard we get an error that the thread does not have an open clipboard,
                    // even though we called OpenClipboard, which apparently just increments a lock counter. Even using the native API first, and then
                    // calling Clipboard.GetDataObject, adding the rest and setting everything together is not possible either, because Clipboard.SetDataObject
                    // corrupts the formats that it does not support (e.g. EnhancedMetafile).
                    if (!User32.EmptyClipboard())
                        return false;

                    foreach (KeyValuePair<string, object> item in formats)
                    {
                        IntPtr hMem = item.Value switch
                        {
                            IntPtr pointer => pointer,
                            MemoryStream ms => StreamToHGlobal(ms),
                            _ => throw new InvalidOperationException(Res.InternalError($"Unhandled clipboard format to populate natively: {item.Key}: {item.Value.GetType()}"))
                        };

                        if (hMem != IntPtr.Zero && !User32.SetClipboardData(DataFormats.GetFormat(item.Key).Id, hMem))
                            Debug.Fail($"Failed to add format '{item.Key}' to the clipboard natively: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                    }

                    return true;
                }
                finally
                {
                    User32.CloseClipboard();
                }
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to populate the clipboard by the native API, trying to use fall back to .NET API ({e.Message})");
                return false;
            }
        }

        private static void CopyMetafile(Dictionary<string, object> formats, Metafile metafile)
        {
            Guid rawFormat = metafile.RawFormat.Guid;
            bool? isEmf = rawFormat == ImageFormat.Emf.Guid ? true
                : rawFormat == ImageFormat.Wmf.Guid ? false
                : null;

            // Metafile is neither EMF not WMF. Can occur when constructed from a bitmap stream, even by BinaryFormatter deserialization.
            // In this case just the Bitmap format will be saved to the clipboard.
            if (isEmf == null)
                return;

            // 1. Custom EMF/WMF format. Used to be able to restore the original raw format even from the clipboard.
            var ms = new MemoryStream();
            try
            {
                if (isEmf == true)
                    metafile.SaveAsEmf(ms);
                else
                    metafile.SaveAsWmf(ms);
                formats.Add(isEmf == true ? emfFormat : wmfFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as metafile stream: {e.Message}");
            }

            if (!OSHelper.IsWindows)
                return;

            // 2. Standard EnhancedMetafile format, which is compatible with many applications.
            // Windows automatically generates a MetaFilePict entry as well (though pasting it does not work, at least on x64 systems).
            // GetHenhmetafile makes the original image unusable, so we clone it first.
            try
            {
                using Metafile clone = (Metafile)metafile.Clone();

                // 2.a. EMF: simply putting a clone to the clipboard.
                if (isEmf == true)
                {
                    IntPtr hEmf = clone.GetHenhmetafile();
                    if (hEmf == IntPtr.Zero)
                        return;
                    try
                    {
                        // We create another copy, which goes to the clipboard. The original handle that belongs to the managed clone will be deleted (and its owner disposed).
                        IntPtr hEmfCopy = Gdi32.CopyEnhMetaFile(hEmf);
                        if (hEmfCopy != IntPtr.Zero)
                            formats.Add(DataFormats.EnhancedMetafile, hEmfCopy);
                        return;
                    }
                    finally
                    {
                        Gdi32.DeleteEnhMetaFile(hEmf);
                    }
                }

                // 2.b. WMF: cannot just put a MetaFilePict format to the clipboard with the handle, because it cannot be pasted by most applications.
                // So converting it to EMF in memory, and putting that to the clipboard (which creates also the MetaFilePict entry, even if unusable).
                IntPtr hmf = clone.GetHenhmetafile(); // NOTE: the method name is misleading: in this case this is NOT an enhanced metafile handle, we need to convert it
                IntPtr hdc = IntPtr.Zero;
                if (hmf == IntPtr.Zero)
                    return;

                try
                {
                    uint size = Gdi32.GetMetaFileBitsEx(hmf, 0, null);
                    if (size == 0)
                        return;

                    byte[] wmfData = new byte[size];
                    Gdi32.GetMetaFileBitsEx(hmf, size, wmfData);

                    var mp = new METAFILEPICT
                    {
                        mm = Constants.MM_ANISOTROPIC,
                        hMF = hmf
                    };

                    hdc = User32.GetDC(IntPtr.Zero);
                    IntPtr hEmf = Gdi32.SetWinMetaFileBits(size, wmfData, hdc, ref mp);
                    if (hEmf != IntPtr.Zero)
                        formats.Add(DataFormats.EnhancedMetafile, hEmf);
                }
                finally
                {
                    if (hdc != IntPtr.Zero)
                        User32.ReleaseDC(IntPtr.Zero, hdc);
                    Gdi32.DeleteMetaFile(hmf);
                }
            }
            catch (Exception e) when (!e.IsCritical())
            {
                // NOTE: normally we should not reach this point, because the WinAPI calls only use SetLastError,
                // but on non-native Windows the P/Invoke may fail otherwise.
                Debug.WriteLine($"Failed to copy as {DataFormats.EnhancedMetafile}: {e.Message}");
            }
        }

        private static void CopyBitmap(Dictionary<string, object> formats, Image image)
        {
            Bitmap bitmap = image as Bitmap ?? new Bitmap(image);
            try
            {
                Size size = bitmap.Size;
                PixelFormat pixelFormat = bitmap.PixelFormat;

                // 1. Custom PNG format. It is recognized by multiple applications, and unlike the standard Bitmap format, it preserves alpha.
                if (pixelFormat.HasAlpha())
                {
                    try
                    {
                        var ms = new MemoryStream();
                        bitmap.SaveAsPng(ms);
                        formats.Add(pngFormat, ms);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        Debug.WriteLine($"Failed to copy as PNG stream: {e.Message}");
                    }
                }

                // 2. Custom GIF format. It is recognized by multiple applications, and unlike the standard Bitmap format, it preserves the palette.
                if (pixelFormat.IsIndexed())
                {
                    Debug.Assert(!formats.ContainsKey(gifFormat), "Here only an animated GIF is expected in the formats, which don't have indexed pixel format");
                    try
                    {
                        var ms = new MemoryStream();
                        bitmap.SaveAsGif(ms);
                        formats.Add(gifFormat, ms);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        Debug.WriteLine($"Failed to copy as GIF stream: {e.Message}");
                    }
                }

                // 3.a. "Standard" Bitmap format on non-Windows system: indicating to use DataObject.SetImage when populating the clipboard
                if (!OSHelper.IsWindows)
                {
                    formats.Add(DataFormats.Bitmap, bitmap);
                    return;
                }

                // 3.b. Standard Bitmap format. The clipboard expects a bitmap created by CreateCompatibleBitmap, so we cannot just return the result of GetHbitmap.
                // When using SetClipboardData, Windows automatically generates DeviceIndependentBitmap (and Format17) entries as well.
                // This is different from the managed Clipboard/DataObject.SetImage APIs, which don't generate these additional formats,
                // but add a BinaryFormatter-serialized System.Drawing.Bitmap entry (along with the standard Bitmap format).
                IntPtr hbmSrc = IntPtr.Zero;
                IntPtr dcScreen = IntPtr.Zero;
                try
                {
                    try
                    {
                        dcScreen = User32.GetDC(IntPtr.Zero);
                        hbmSrc = bitmap.GetHbitmap();
                        IntPtr dcSrc = Gdi32.CreateCompatibleDC(dcScreen);
                        IntPtr prevSrc = Gdi32.SelectObject(dcSrc, hbmSrc);

                        IntPtr dcDst = Gdi32.CreateCompatibleDC(dcScreen);
                        IntPtr hbmDst = Gdi32.CreateCompatibleBitmap(dcScreen, size.Width, size.Height);
                        IntPtr prevDst = Gdi32.SelectObject(dcDst, hbmDst);

                        // copying the content and storing the result
                        Gdi32.BitBlt(dcDst, dcSrc, size);
                        formats.Add(DataFormats.Bitmap, hbmDst);

                        // cleanup
                        Gdi32.SelectObject(dcSrc, prevSrc);
                        Gdi32.SelectObject(dcDst, prevDst);
                        Gdi32.DeleteDC(dcSrc);
                        Gdi32.DeleteDC(dcDst);
                    }
                    finally
                    {
                        if (hbmSrc != IntPtr.Zero)
                            Gdi32.DeleteObject(hbmSrc);
                        if (dcScreen != IntPtr.Zero)
                            User32.ReleaseDC(IntPtr.Zero, dcScreen);
                    }
                }
                catch (Exception e) when (!e.IsCritical())
                {
                    // NOTE: normally we should not reach this point, because the WinAPI calls only use SetLastError,
                    // but on non-native Windows the P/Invoke may fail otherwise.
                    Debug.WriteLine($"Failed to copy as {DataFormats.Bitmap}: {e.Message}");
                }
            }
            finally
            {
                if (!ReferenceEquals(bitmap, image))
                    bitmap.Dispose();
            }
        }

        private static void CopyTiff(Dictionary<string, object> formats, ImageInfo info)
        {
            try
            {
                // If the main image is already a TIFF, using that one.
                var ms = new MemoryStream();
                if (info.Image is Image image && image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                    image.SaveAsTiff(ms, false);
                else
                    info.Frames!.Select(f => f.Image!).SaveAsMultipageTiff(ms);
                formats.Add(tiffFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as TIFF stream: {e.Message}");
            }
        }

        private static void CopyIcon(Dictionary<string, object> formats, ImageInfo info)
        {
            try
            {
                var ms = new MemoryStream();
                info.GetCreateIcon()!.SaveAsIcon(ms);
                formats.Add(iconFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as ICO stream: {e.Message}");
            }
        }

        private static void CopyAnimGif(Dictionary<string, object> formats, ImageInfo info)
        {
            try
            {
                // If the animation is not generated, this may take a lot of time (though the default quantizer is quite fast). TODO: async, and progress bar in the caller
                var ms = new MemoryStream();
                info.GetCreateImage()!.SaveAsGif(ms);
                formats.Add(gifFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as TIFF stream: {e.Message}");
            }
        }

        private static ImageInfo? EnsureFormat(object? data, AllowedImageTypes allowedTypes, bool allowMultiFrame)
        {
            switch (data)
            {
                case null:
                    return null;

                case Metafile metafile:
                    if ((allowedTypes & AllowedImageTypes.Metafile) != AllowedImageTypes.None)
                        return new ImageInfo(metafile);

                    Size size = metafile.Size;
                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        Bitmap bitmap = metafile.ToBitmap(size);
                        metafile.Dispose();
                        return new ImageInfo(bitmap);
                    }

                    Debug.Assert((allowedTypes & AllowedImageTypes.Icon) != 0);
                    Icon asIcon = metafile.ToIcon(Math.Min(256, Math.Max(size.Width, size.Height)), true);
                    metafile.Dispose();
                    return new ImageInfo(asIcon);

                case Bitmap bitmap:
                    Debug.Assert(allowedTypes != AllowedImageTypes.Metafile, "Not expected to be called with a Bitmap if metafiles are allowed only");
                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        if (!allowMultiFrame && bitmap.FrameDimensionsList is Guid[] { Length: > 0 } dimensions && bitmap.GetFrameCount(new FrameDimension(dimensions[0])) > 1)
                        {
                            Bitmap frame = bitmap.CloneCurrentFrame();
                            bitmap.Dispose();
                            return new ImageInfo(frame);
                        }

                        Debug.Assert(bitmap.RawFormat.Guid != ImageFormat.Icon.Guid);
                        return new ImageInfo(bitmap);
                    }

                    size = bitmap.Size;
                    if ((allowedTypes & AllowedImageTypes.Icon) != 0)
                    {
                        Icon icon = bitmap.ToIcon(Math.Min(256, Math.Max(size.Width, size.Height)), true);
                        bitmap.Dispose();
                        return new ImageInfo(icon);
                    }

                    bitmap.Dispose();
                    return null;

                case Icon icon:
                    Debug.Assert(allowedTypes != AllowedImageTypes.Metafile, "Not expected to be called with an Icon if metafiles are allowed only");
                    if ((allowedTypes & AllowedImageTypes.Icon) != AllowedImageTypes.None)
                        return new ImageInfo(icon);

                    if ((allowedTypes & AllowedImageTypes.Bitmap) != AllowedImageTypes.None)
                    {
                        Bitmap bitmap = OSHelper.IsWindows && allowMultiFrame ? icon.ToMultiResBitmap() : icon.ToAlphaBitmap();
                        icon.Dispose();
                        return new ImageInfo(bitmap);
                    }

                    icon.Dispose();
                    return null;

                default:
                    throw new InvalidOperationException(Res.InternalError($"Unexpected type in EnsureFormat: {data.GetType()}"));
            }
        }

        private static bool TryGetStandardBitmap(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, out ImageInfo? result)
        {
            result = null;

            // Trying the COM way first
            if (dataObject is IComDataObject comDataObject)
            {
                try
                {
                    var format = new FORMATETC
                    {
                        cfFormat = (short)DataFormats.GetFormat(DataFormats.Bitmap).Id,
                        dwAspect = DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        tymed = TYMED.TYMED_GDI
                    };

                    int hResult = comDataObject.QueryGetData(ref format);
                    if (hResult == Constants.S_OK)
                    {
                        comDataObject.GetData(ref format, out STGMEDIUM medium);
                        try
                        {
                            if (medium.tymed == TYMED.TYMED_GDI)
                            {
                                // NOTE: Image.FromHBitmap always creates a copy, so we are safe even when we don't own the handle (that is, when medium.pUnkForRelease is not null).
                                // The managed GetData always clones the result here, but the docs says: "The FromHbitmap method makes a copy of the GDI bitmap; so you can release
                                // the incoming GDI bitmap using the GDI DeleteObject method immediately after creating the new Image."
                                Bitmap bitmap = Image.FromHbitmap(medium.unionmember);
                                result = EnsureFormat(bitmap, allowedTypes, false);
                                if (result != null)
                                    return true;
                            }
                            else
                                Debug.WriteLine($"Expected vs. actual format of {DataFormats.Bitmap}: {format.tymed} <-> {medium.tymed}");
                        }
                        finally
                        {
                            // If the type was TYMED_GDI, this may call Gdi32.DeleteObject, depending on medium.pUnkForRelease
                            Ole32.ReleaseStgMedium(ref medium);
                        }
                    }
                    else
                        Debug.WriteLine($"Failed to get native {DataFormats.Bitmap} format: HRESULT is {hResult}");
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    Debug.WriteLine($"Failed to get native {DataFormats.Bitmap} format: {e.Message}");
                }
            }

            // 2. Trying to use the managed API - this may copy the image more times in memory than needed. It also tries to retrieve BinaryFormatter serialized System.Drawing.Bitmap entry
#if NET10_0_OR_GREATER
            if (dataObject.TryGetData<Image>(DataFormats.Bitmap, out Image? image))
#else
            if (dataObject.GetData(DataFormats.Bitmap) is Image image)
#endif
            {
                result = EnsureFormat(image, allowedTypes, false);
            }

            return result != null;
        }

        private static bool TryGetNativeEmf(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, out ImageInfo? result)
        {
            result = null;
            if (dataObject is not IComDataObject comDataObject)
                return false;

            try
            {
                var format = new FORMATETC
                {
                    cfFormat = (short)DataFormats.GetFormat(DataFormats.EnhancedMetafile).Id,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_ENHMF
                };

                int hResult = comDataObject.QueryGetData(ref format);
                if (hResult != Constants.S_OK)
                {
                    Debug.WriteLine($"Failed to get native {DataFormats.EnhancedMetafile} format: HRESULT is {hResult}");
                    return false;
                }

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                if (medium.tymed != TYMED.TYMED_ENHMF)
                {
                    Ole32.ReleaseStgMedium(ref medium);
                    Debug.Fail($"Expected vs. actual format of {DataFormats.EnhancedMetafile}: {format.tymed} <-> {medium.tymed}");
                    return false;
                }

                // If we do not own the medium, we create a copy to prevent the metafile from becoming unusable when the owner frees it up.
                // Disposing the result metafile calls DeleteEnhancedMetafile due to the deleteEmf parameter, so not calling ReleaseStgMedium here.
                IntPtr hEmf = medium.pUnkForRelease == null ? medium.unionmember : Gdi32.CopyEnhMetaFile(medium.unionmember);
                result = EnsureFormat(new Metafile(hEmf, true), allowedTypes, false);
                return result != null;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get native {DataFormats.EnhancedMetafile} format: {e.Message}");
                return false;
            }
        }

        private static bool TryGetNativeWmf(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, out ImageInfo? result)
        {
            result = null;
            if (dataObject is not IComDataObject comDataObject)
                return false;

            try
            {
                var format = new FORMATETC
                {
                    cfFormat = (short)DataFormats.GetFormat(DataFormats.MetafilePict).Id,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_MFPICT
                };

                int hResult = comDataObject.QueryGetData(ref format);
                if (hResult != Constants.S_OK)
                {
                    Debug.WriteLine($"Failed to get native {DataFormats.MetafilePict} format: HRESULT is {hResult}");
                    return false;
                }

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                if (medium.tymed != TYMED.TYMED_MFPICT)
                {
                    Ole32.ReleaseStgMedium(ref medium);
                    Debug.Fail($"Expected vs. actual format of {DataFormats.MetafilePict}: {format.tymed} <-> {medium.tymed}");
                    return false;
                }

                IntPtr ptrMetafilePict = Kernel32.GlobalLock(medium.unionmember);
                if (ptrMetafilePict == IntPtr.Zero)
                    return false;

                IntPtr hmf = IntPtr.Zero;
                try
                {
                    unsafe { hmf = ((METAFILEPICT*)ptrMetafilePict)->hMF; }

                    // NOTE: Maybe it's an architectural/Windows version thing, but this always returns 0, and sets LastWin32Error to 6 (Invalid handle).
                    // Fallback attempts (e.g. cloning the metafile first, or replaying the WMF content into a new enhanced metafile) don't work either.
                    uint size = Gdi32.GetMetaFileBitsEx(hmf, 0, null);
                    if (size == 0u)
                        return false;
                    
                    var buffer = new byte[size];
                    if (Gdi32.GetMetaFileBitsEx(hmf, size, buffer) == 0)
                        return false;

                    using var ms = new MemoryStream(buffer);
                    result = EnsureFormat(new Metafile(ms), allowedTypes, false);
                    return result != null;

                    //// Fallback attempt: creating an Enhanced Metafile, and replaying the WMF content into it.
                    //IntPtr hdc = User32.GetDC(IntPtr.Zero);

                    //IntPtr hdcEmf = Gdi32.CreateEnhMetaFile(hdc, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    //try
                    //{
                    //    bool success = Gdi32.PlayMetaFile(hdcEmf, hmf);
                    //    IntPtr hEmf = Gdi32.CloseEnhMetaFile(hdcEmf);

                    //    if (success)
                    //        metafile = new Metafile(hEmf, true);
                    //    else
                    //        Gdi32.DeleteEnhMetaFile(hEmf);
                    //    return success;
                    //}
                    //finally
                    //{
                    //    if (hdcEmf != IntPtr.Zero)
                    //        Gdi32.DeleteDC(hdcEmf);
                    //    User32.ReleaseDC(IntPtr.Zero, hdc);
                    //}
                }
                finally
                {
                    // Same as calling ReleaseStgMedium. Unlike in EMF case, we always free the metafile here, because we create the result from a new stream.
                    if (medium.pUnkForRelease == null && hmf != IntPtr.Zero)
                        Gdi32.DeleteMetaFile(hmf);
                    Kernel32.GlobalUnlock(medium.unionmember);
                }
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get native {DataFormats.MetafilePict} format: {e.Message}");
                return false;
            }
        }

        private static bool TryGetImageFromStream(IEnumerable<string> formats, IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, bool allowMultiFrame, out ImageInfo? imageInfo)
        {
            Debug.Assert(allowedTypes != AllowedImageTypes.None);

            foreach (string format in formats)
            {
                MemoryStream? stream = TryGetStream(format, dataObject);
                if (stream == null)
                    continue;

                try
                {
                    imageInfo = EnsureFormat(format is iconFormat ? Icons.FromStream(stream) : Image.FromStream(stream), allowedTypes, allowMultiFrame);
                    if (imageInfo != null)
                        return true;

                    Debug.WriteLine($"Pasting from {format} was requested, but the allowed formats are {allowedTypes}");
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    Debug.WriteLine($"Failed to paste content from format '{format}': {e.Message}");
                }
            }

            imageInfo = null;
            return false;
        }

        private static MemoryStream? TryGetStream(string format, IWinFormsDataObject dataObject)
        {
            // 1. First, trying to use the native COM IDataObject interface
            if (dataObject is IComDataObject comDataObject)
            {
                try
                {
                    var comFormat = new FORMATETC
                    {
                        cfFormat = (short)DataFormats.GetFormat(format).Id,
                        dwAspect = DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        tymed = TYMED.TYMED_ISTREAM | TYMED.TYMED_HGLOBAL
                    };

                    int hResult = comDataObject.QueryGetData(ref comFormat);
                    if (hResult == Constants.S_OK)
                    {
                        comDataObject.GetData(ref comFormat, out STGMEDIUM medium);
                        try
                        {
                            switch (medium.tymed)
                            {
                                case TYMED.TYMED_ISTREAM:
                                    return ComIStreamToStream(ref medium);

                                case TYMED.TYMED_HGLOBAL:
                                    MemoryStream? stream = HGlobalToStream(ref medium);
                                    if (stream == null)
                                        break;
                                    return stream;

                                default:
                                    // Not failing this time, so we can still try the fallback with the non-COM interface
                                    Debug.WriteLine($"Expected vs. actual format of {format}: {comFormat.tymed} <-> {medium.tymed}");
                                    break;
                            }
                        }
                        finally
                        {
                            // It's much simpler to release the medium this way than knowing what to do
                            // for the different TYMED and pUnkForRelease combinations.
                            Ole32.ReleaseStgMedium(ref medium);
                        }
                    }
                    else
                        Debug.WriteLine($"Failed to get COM {format} stream: HRESULT is {hResult}");
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    Debug.WriteLine($"Failed to get COM {format} stream: {e.Message}");
                }
            }

            // 2. Trying to use the managed IDataObject interface
            try
            {
#if NET10_0_OR_GREATER
                if (dataObject.TryGetData<MemoryStream>(format, out MemoryStream? stream))
                    return stream;
                Debug.WriteLine($"Failed to get a stream for format {format}");
                return null;
#else
                object? data = dataObject.GetData(format);
                switch (data)
                {
                    case MemoryStream stream:
                        return stream;
                    
                    case null:
                        Debug.WriteLine($"Failed to get a stream for format {format}");
                        return null;

                    default:
                        Debug.Fail($"Unexpected non-stream type for format '{format}': {data.GetType()}");
                        return null;
                }
#endif
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get metafile by IDataObject.GetData({format}): {e.Message}");
                return null;
            }
        }

        private static MemoryStream ComIStreamToStream(ref STGMEDIUM medium)
        {
            Debug.Assert(medium.tymed == TYMED.TYMED_ISTREAM);
            IStream comStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
            Marshal.Release(medium.unionmember);
            comStream.Stat(out STATSTG stat, 0);
            byte[] content = new byte[stat.cbSize];

            // Sometimes the stream position is at the end when we obtain the stream
            comStream.Seek(0, Constants.STREAM_SEEK_SET, IntPtr.Zero);
            int read;
            unsafe { comStream.Read(content, content.Length, (IntPtr)(&read)); }

            Debug.Assert(read == content.Length, "IStream.Read should not read less bytes than requested, unless the end of stream is reached.");
            return new MemoryStream(content);
        }

        private static MemoryStream? HGlobalToStream(ref STGMEDIUM medium)
        {
            Debug.Assert(medium.tymed == TYMED.TYMED_HGLOBAL);
            IntPtr ptrStream = Kernel32.GlobalLock(medium.unionmember);
            try
            {
                if (ptrStream == IntPtr.Zero)
                    return null;

                nuint size = Kernel32.GlobalSize(medium.unionmember);
                if (size == 0 || size >= Constants.MaxArrayLength)
                    return null;
                byte[] content = new byte[size];
                Marshal.Copy(ptrStream, content, 0, (int)size);

                // Ignoring BinaryFormatter content here
                if (content.Length > binaryFormatterStreamPrefix.Length)
                {
#if NETCOREAPP3_0_OR_GREATER
                    if (content.AsSpan().StartsWith(binaryFormatterStreamPrefix))
                        return null;
#else
                    if (content.AsSection(0, binaryFormatterStreamPrefix.Length).SequenceEqual(binaryFormatterStreamPrefix))
                        return null;
#endif
                }

                return new MemoryStream(content);
            }
            finally
            {
                Kernel32.GlobalUnlock(medium.unionmember);
            }
        }

        private static IntPtr StreamToHGlobal(MemoryStream ms)
        {
            int size = (int)ms.Length; // always must success, because it's an array length
            IntPtr hGlobal = Kernel32.GlobalAlloc(Constants.GMEM_SHARE | Constants.GMEM_MOVEABLE, size);
            IntPtr hMem = IntPtr.Zero;
            try
            {
                if (hGlobal != IntPtr.Zero)
                    hMem = Kernel32.GlobalLock(hGlobal);
                if (hMem == IntPtr.Zero)
                {
                    Debug.WriteLine($"Failed to allocate {ms.Length} bytes of global memory for the stream.");
                    return IntPtr.Zero;
                }

                Marshal.Copy(ms.ToArray(), 0, hMem, size);
                return hMem;
            }
            finally
            {
                if (hGlobal != IntPtr.Zero)
                {
                    Kernel32.GlobalUnlock(hGlobal);
                    if (hMem == IntPtr.Zero)
                        Kernel32.GlobalFree(hGlobal);
                }
            }
        }

        #endregion

        #endregion
    }
}