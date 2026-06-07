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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;
using KGySoft.Serialization.Binary;
using KGySoft.Threading;
using KGySoft.WinForms;

#endregion

#region Used Aliases

using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using IWinFormsDataObject = System.Windows.Forms.IDataObject;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;
using Timer = System.Windows.Forms.Timer;

#endregion

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class ClipboardHelper
    {
        #region Nested Classes

        #region ClipboardListener class

        private sealed class ClipboardListener : Control
        {
            #region Fields

            private readonly Timer? timer;

            private IntPtr nextListener;
            private string[]? lastFormats;

            #endregion

            #region Constructors

            internal ClipboardListener()
            {
                if (!OSHelper.IsWindows)
                {
                    timer = new Timer { Interval = 250 };
                    timer.Tick += Timer_Tick;
                }

                CreateHandle();
            }

            #endregion

            #region Methods
            
            #region Protected Methods

            protected override void CreateHandle()
            {
                base.CreateHandle();
                Debug.Assert(IsHandleCreated);
                if (OSHelper.IsWindowsVistaOrLater)
                {
                    if (User32.AddClipboardFormatListener(Handle))
                        return;
                }

                // Legacy solution (works also on Windows XP)
                if (OSHelper.IsWindows)
                {
                    nextListener = User32.SetClipboardViewer(Handle);
                    return;
                }

                // Fallback for non-Windows platforms: polling by managed API
                lastFormats = GetImageFormats();
                timer?.Enabled = true;
            }

            protected override void DestroyHandle()
            {
                Debug.Assert(IsHandleCreated);
                if (OSHelper.IsWindowsVistaOrLater && nextListener == IntPtr.Zero)
                    User32.RemoveClipboardFormatListener(Handle);
                else if (OSHelper.IsWindows)
                {
                    User32.ChangeClipboardChain(Handle, nextListener);
                    nextListener = IntPtr.Zero;
                }
                else
                    timer?.Enabled = false;

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

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    timer?.Dispose();
                base.Dispose(disposing);
            }

            #endregion

            #region Event Handlers

            private void Timer_Tick(object sender, EventArgs e)
            {
                string[] formats = GetImageFormats();
                if (formats.SequenceEqual(lastFormats ?? Reflector.EmptyArray<string>()))
                    return;

                lastFormats = formats;
                OnClipboardChanged();
            }

            #endregion

            #endregion
        }

        #endregion

        #region ClipboardFormatId class

        /// <summary>
        /// Like <see cref="DataFormats"/> and <see cref="DataFormats.Format"/> in WinForms.
        /// The problems with DataFormats:
        /// - It does not recognize DIBv5: it returns the name "Format17" for it, whereas GetFormat("Format17").Id returns a random newly registered number instead of CF_DIBV5.
        /// - For such unrecognised standard formats it's not possible to get the original standard id, or at least differentiate standard and registered formats.
        /// - On top of those, on Mono/Windows DataFormats.GetFormat closes the clipboard, making SetClipboardData fail if we try to retrieve the id by it, for example.
        /// </summary>
        internal sealed class ClipboardFormatId
        {
            #region Constants

            private const string clipboardFormatPrefix = "CF_";

            #endregion

            #region Properties

            internal ClipboardFormat Id { get; }
            internal string? RegisteredName { get; }
            internal string Name { get; }

            #endregion

            #region Constructors

            internal ClipboardFormatId(ClipboardFormat format)
            {
                if (format == ClipboardFormat.None)
                    throw new ArgumentOutOfRangeException(PublicResources.ArgumentOutOfRange, nameof(format));

                Id = format;

                // Returning a friendly name for the known image formats only. Using the CF_* enum names for other predefined formats.
                string? name = format switch
                {
                    ClipboardFormat.CF_BITMAP => bitmapFormat,
                    ClipboardFormat.CF_METAFILEPICT => metafilePictFormat,
                    ClipboardFormat.CF_TIFF => tiffFormat,
                    ClipboardFormat.CF_DIB => dibFormat,
                    ClipboardFormat.CF_ENHMETAFILE => enhMetafileFormat,
                    ClipboardFormat.CF_DIBV5 => dibV5Format,
                    _ => Enum<ClipboardFormat>.GetName(format)
                };

                if (name != null)
                {
                    Name = name;
                    return;
                }

                // RegisteredName will be null if a standard format is not defined in ClipboardFormat
                RegisteredName = OSHelper.IsWindows ? User32.GetClipboardFormatName(format) : DataFormats.GetFormat((int)format).Name;
                Name = RegisteredName ?? (Name = $"{clipboardFormatPrefix}{format}");
            }

            internal ClipboardFormatId(string name)
            {
                if (name == null)
                    throw new ArgumentNullException(nameof(name), PublicResources.ArgumentNull);
                if (name.Length == 0)
                    throw new ArgumentException(PublicResources.ArgumentEmpty, nameof(name));

                Name = name;
                Id = name switch
                {
                    bitmapFormat => ClipboardFormat.CF_BITMAP,
                    metafilePictFormat => ClipboardFormat.CF_METAFILEPICT,
                    tiffFormat => ClipboardFormat.CF_TIFF,
                    dibFormat => ClipboardFormat.CF_DIB,
                    enhMetafileFormat => ClipboardFormat.CF_ENHMETAFILE,
                    dibV5Format => ClipboardFormat.CF_DIBV5,
                    _ => ClipboardFormat.None //User32.GetClipboardFormat(name)
                };

                if (Id != ClipboardFormat.None)
                    return;

                // Further standard names here are resolved from CF_*.
                if (name.StartsWith(clipboardFormatPrefix, StringComparison.Ordinal))
                {
                    if (Enum<ClipboardFormat>.TryParse(name, out var result))
                    {
                        Id = result;
                        return;
                    }

                    if (UInt32.TryParse(name.Substring(clipboardFormatPrefix.Length), NumberStyles.None, NumberFormatInfo.InvariantInfo, out uint id))
                    {
                        Id = (ClipboardFormat)id;
                        return;
                    }
                }

                RegisteredName = Name;
                Id = OSHelper.IsWindows ? User32.RegisterClipboardFormat(RegisteredName) : (ClipboardFormat)DataFormats.GetFormat(name).Id;
            }

            #endregion

            #region Methods

            #region Static Methods

            internal static string FromWinFormsFormat(string format)
                => format is null ? throw new ArgumentNullException(nameof(format), PublicResources.ArgumentNull)
                : format == DataFormats.Bitmap ? bitmapFormat
                : format == DataFormats.MetafilePict ? metafilePictFormat
                : format == DataFormats.Tiff ? tiffFormat
                : format == DataFormats.Dib ? dibFormat
                : format == DataFormats.EnhancedMetafile ? enhMetafileFormat
                : format == "Format17" ? dibV5Format
                : new ClipboardFormatId((ClipboardFormat)DataFormats.GetFormat(format).Id).Name;

            internal static string ToWinFormsFormat(string format)
            {
                var formatId = new ClipboardFormatId(format);
                return formatId.RegisteredName ?? DataFormats.GetFormat((int)formatId.Id).Name;
            }

            #endregion

            #region Instance Methods

            public override string ToString() => Name;

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region Constants

        // standard format names
        private const string bitmapFormat = "Bitmap"; // CF_BITMAP - using the same name as WinForms, because this is handled also by WinForms
        private const string metafilePictFormat = "MetaFilePict"; // CF_METAFILEPICT
        private const string dibFormat = "DIB"; // CF_DIB
        private const string tiffFormat = "TIFF"; // CF_TIFF
        private const string enhMetafileFormat = "EnhancedMetafile"; // CF_ENHMETAFILE
        private const string dibV5Format = "DIB V5"; // CF_DIBV5

        // custom formats
        private const string emfFormat = "EMF";
        private const string wmfFormat = "WMF";
        private const string gifFormat = "GIF";
        private const string pngFormat = "PNG";
        private const string iconFormat = "ICO";
        private const string jpegFormat = "JFIF";

        private const string pngAliasFormat = "image/png";
        private const string gifAliasFormat = "image/gif";
        private const string jpegAliasFormat = "image/jpeg";

        #endregion

        #region Fields

        private readonly static Lock syncRoot = new();
        private readonly static HashSet<string> bitmapFormats =
        [
            bitmapFormat, dibFormat, tiffFormat, typeof(Bitmap).FullName!, pngFormat, gifFormat, dibV5Format, jpegFormat,
            pngAliasFormat, gifAliasFormat, jpegAliasFormat,
        ];
        private readonly static string[] metafileFormats = [enhMetafileFormat, metafilePictFormat, emfFormat, wmfFormat];
        private readonly static string[] iconFormats = [iconFormat];
        private readonly static string[] supportedFormats = [..bitmapFormats, ..metafileFormats, ..iconFormats];

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

        #region Properties

        internal static bool ContainsSupportedImage => GetImageFormats().Length > 0;

        #endregion

        #region Methods

        #region Internal Methods

        internal static bool IsBitmap(string format) => bitmapFormats.Contains(format);
        internal static bool IsMetafile(string format) => metafileFormats.Contains(format);
        internal static bool IsIcon(string format) => iconFormats.Contains(format);

        internal static string[] GetImageFormats() => GetClipboardFormats().Intersect(supportedFormats).ToArray();

        internal static void CopyToClipboard(ImageInfoBase info, AsyncTaskContext task)
        {
            // NOTE: Using SetImage as a fallback solution only, because it does not support the native metafile formats, and puts a BinaryFormatter
            // entry on the clipboard a with potentially poorly chosen encoder (supporting such formats in TryPasteFromClipboard, though).
            // Instead, preparing the native Bitmap format as a well-known fallback (which is supported by the managed clipboard API as well) explicitly.
            
            // Value is IntPtr or MemoryStream. Using Stream is better than byte[], because in the managed API Stream is just converted to a byte[],
            // whereas byte[] is serialized in a BinaryFormatter-compatible way.
            var formats = new Dictionary<string, object>();

            try
            {
                // compound image or single frame image
                if (info is ImageInfo imageInfo)
                {
                    switch (imageInfo.Type)
                    {
                        case ImageInfoType.None:
                            Debug.Fail("Copying expected to be disabled when there is no loaded image");
                            return;

                        case ImageInfoType.SingleImage when imageInfo.Image is Metafile metafile:
                            CopyMetafile(formats, metafile, task);
                            break;

                        case ImageInfoType.Pages:
                            CopyTiff(formats, imageInfo, task);
                            break;

                        case ImageInfoType.MultiRes:
                        case ImageInfoType.Icon:
                            CopyIcon(formats, imageInfo, task);
                            break;

                        case ImageInfoType.Animation:
                            CopyAnimGif(formats, imageInfo, task);
                            break;
                    }

                    if (task.IsCanceled)
                        return;
                }
                // single icon frame
                else if (info.Icon is not null || info.RawFormat == ImageFormat.Icon.Guid)
                    CopyIcon(formats, info, task);

                // GetCreateImage with no cancellation is alright here - due to the compound formats above, actual generate is expected for icon bitmaps only
                Image? image = info.GetCreateImage();
                Debug.Assert(image != null, "Failed to obtain an image to copy");
                if (image == null)
                    return;

                // Standard Bitmap format, potentially along with some custom single-frame formats
                CopyBitmap(formats, image, task);

                if (task.IsCanceled)
                    return;

                // If we managed to prepare some formats, we place them on the clipboard. If it fails, using Clipboard.SetImage as an ultimate fallback.
                if (formats.Count == 0 || !PopulateClipboard(formats, task))
                {
                    task.Context.Send(_ =>
                    {
                        lock (image)
                            Clipboard.SetImage(image);
                    }, null);
                }
            }
            finally
            {
                // If the task has been canceled before actually setting the clipboard, we must free the possibly generated native objects.
                // If the cancellation occurs after setting the clipboard, the dictionary is expected to be emptied here.
                if (task.IsCanceled && formats.Count > 0)
                {
                    foreach (KeyValuePair<string, object> item in formats)
                    {
                        if (item.Value is IntPtr nativeHandle)
                            FreeNativeHandle(nativeHandle, item.Key);
                    }
                }
            }
        }

        internal static ImageInfo? TryPasteFromClipboard(AllowedImageTypes allowedTypes, bool allowMultiFrame, AsyncTaskContext task)
            => DoTryPasteFromClipboard(null, allowedTypes, allowMultiFrame, default, task);

        internal static ImageInfo? TryPasteSpecial(string format, AllowedImageTypes allowedTypes, bool allowMultiFrame, bool detectAlpha, AsyncTaskContext task)
            => DoTryPasteFromClipboard(format, allowedTypes, allowMultiFrame, detectAlpha, task);

        #endregion

        #region Private Methods

        private static void OnClipboardChanged() => clipboardChangedHandler?.Invoke(null, EventArgs.Empty);

        private static bool PopulateClipboard(Dictionary<string, object> formats, AsyncTaskContext task)
        {
            // NOTE: Not checking cancellation while adding the items. Either none or all formats are copied to the clipboard.
            bool useNativeApi = OSHelper.IsWindows && formats.GetValueOrDefault(bitmapFormat) is not Image;
            if (useNativeApi && PopulateClipboardNatively(formats))
                return true;

            if (task.IsCanceled)
                return true; // to prevent trying the ultimate fallback path in the caller

            // Here we could not populate the clipboard natively. As a fallback, we try to use the managed way.
            var dataObject = new DataObject();
            bool result = false;
            foreach (KeyValuePair<string, object> item in formats)
            {
                switch (item.Value)
                {
                    case MemoryStream ms:
                        result = true;
                        dataObject.SetData(ClipboardFormatId.ToWinFormsFormat(item.Key), false, ms);
                        break;

                    // If we have prepared native entries, we discard them, because DataObject.SetData would just corrupt them.
                    // As the clipboard will not take over the ownership of such items now, we must free them to prevent memory leaks.
                    case IntPtr nativeHandle:
                        FreeNativeHandle(nativeHandle, item.Key);
                        break;

                    case Image image:
                        result = true;
                        lock (image)
                            dataObject.SetImage(image);
                        break;

                    default:
                        throw new InvalidOperationException(Res.InternalError($"Unhandled clipboard format to populate in a managed way: {item.Key}: {item.Value.GetType()}"));
                }
            }

            // as we already freed native items, clearing the dictionary to prevent double freeing if the task gets canceled after this point
            formats.Clear();
            if (result)
                task.Context.Send(_ => Clipboard.SetDataObject(dataObject, true), null);
            return result;
        }

        private static bool PopulateClipboardNatively(Dictionary<string, object> formats)
        {
            try
            {
                if (!User32.OpenClipboard())
                    return false;

                bool result = false;
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
                            IntPtr handle => handle,
                            MemoryStream ms => StreamToHGlobal(ms),
                            //GlobalHandleStream gs => gs.HGlobal == IntPtr.Zero ? gs.HGlobal = StreamToHGlobal(gs.Stream) : gs.HGlobal, // sharing a HGlobal under multiple formats does not work, the main thread crashes with heap corruption detection
                            _ => throw new InvalidOperationException(Res.InternalError($"Unhandled clipboard format to populate natively: {item.Key}: {item.Value.GetType()}"))
                        };

                        ClipboardFormat format = new ClipboardFormatId(item.Key).Id;
                        if (hMem != IntPtr.Zero && !User32.SetClipboardData(format, hMem))
                            Debug.Fail($"Failed to add format '{item.Key}' ({format}) to the clipboard natively: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                    }

                    return result = true;
                }
                finally
                {
                    // clearing the formats on success, indicating that no cleanup should be performed even if the task gets canceled after this point
                    if (result)
                        formats.Clear();
                    User32.CloseClipboard();
                }
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to populate the clipboard by the native API, trying to use fall back to .NET API ({e.Message})");
                return false;
            }
        }

        private static void FreeNativeHandle(IntPtr nativeHandle, string format)
        {
            Debug.Assert(OSHelper.IsWindows);
            if (format == bitmapFormat)
                Gdi32.DeleteObject(nativeHandle);
            else if (format == enhMetafileFormat)
                Gdi32.DeleteEnhMetaFile(nativeHandle);
            else
                throw new InvalidOperationException(Res.InternalError($"Unhandled native clipboard item to free: {format}"));
        }

        private static void CopyMetafile(Dictionary<string, object> formats, Metafile metafile, AsyncTaskBase task)
        {
            if (!OSHelper.IsWindows)
                return;

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
                // Not sure whether metafiles are also affected by the "region is already locked" error,
                // but using cooperative locking on them the same way as for bitmaps.
                lock (metafile)
                {
                    if (isEmf == true)
                        metafile.SaveAsEmf(ms);
                    else
                        metafile.SaveAsWmf(ms);
                }

                formats.Add(isEmf == true ? emfFormat : wmfFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as metafile stream: {e.Message}");
            }

            if (!OSHelper.IsWindows || task.IsCanceled)
                return;

            // 2. Standard EnhancedMetafile format, which is compatible with many applications.
            // Windows automatically generates a MetaFilePict entry as well (though pasting it as non-enhanced metafile does not work, at least on XP+).
            // GetHenhmetafile makes the original image unusable, so we clone it first.
            Metafile? clone = null;
            try
            {
                // A clone must be created; otherwise, the original metafiles become unusable after the GetHenhmetafile call. Using the same cooperative locking as for bitmaps.
                lock (metafile)
                    clone = (Metafile)metafile.Clone();

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
                            formats.Add(enhMetafileFormat, hEmfCopy);
                        return;
                    }
                    finally
                    {
                        Gdi32.DeleteEnhMetaFile(hEmf);
                    }
                }

                // 2.b. WMF: not just putting a MetaFilePict format to the clipboard with the handle, because it cannot be pasted by many applications.
                // Instead, converting it to EMF in memory, and putting that to the clipboard (which creates also the MetaFilePict entry).
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
                        formats.Add(enhMetafileFormat, hEmf);
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
                Debug.WriteLine($"Failed to copy as {enhMetafileFormat}: {e.Message}");
            }
            finally
            {
                clone?.Dispose();
            }
        }

        private static void CopyBitmap(Dictionary<string, object> formats, Image image, AsyncTaskBase task)
        {
            Bitmap bitmap = image.AsBitmap();
            try
            {
                PixelFormat pixelFormat = bitmap.PixelFormat;
                bool hasAlpha = pixelFormat.HasAlpha() || (pixelFormat.IsIndexed() && bitmap.Palette.Entries.Any(c => c.A != Byte.MaxValue));

                // 1. Custom PNG format. It is recognized by multiple applications, and unlike the standard Bitmap format, it preserves alpha.
                // On Linux/Mono always adding this format, because the standard Bitmap format is not supported, and Clipboard.SetImage/GetImage does not work either.
                if (hasAlpha || OSHelper.IsLinuxMono)
                {
                    try
                    {
                        // var sharedGlobal = new GlobalHandleStream(); // sharing a HGlobal under multiple formats does not work, the main thread crashes with heap corruption detection
                        var ms = new MemoryStream();
                        lock (bitmap)
                            bitmap.SaveAsPng(ms); // bitmap.SaveAsPng(sharedGlobal.Stream);
                        formats.Add(pngFormat, ms);
                        //formats.Add(pngAliasFormat, sharedGlobal);
                        if (task.IsCanceled)
                            return;
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        Debug.WriteLine($"Failed to copy as PNG stream: {e.Message}");
                    }
                }

                // 2. Custom GIF format. It is recognized by multiple applications, and unlike the standard Bitmap format, it preserves the palette.
                if (pixelFormat.IsIndexed())
                {
                    Debug.Assert(!formats.ContainsKey(gifFormat), "Here only an animated GIF is expected in the prepared formats, which don't have indexed pixel format");
                    try
                    {
                        var ms = new MemoryStream();
                        lock (bitmap)
                            bitmap.SaveAsGif(ms);
                        formats.Add(gifFormat, ms);
                        if (task.IsCanceled)
                            return;
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        Debug.WriteLine($"Failed to copy as GIF stream: {e.Message}");
                    }
                }

                // 3. Standard formats
                if (!OSHelper.IsWindows)
                {
                    // Manged fallback when Windows API cannot be used. It forces using IDataObject.SetImage in PopulateClipboard.
                    // Not doing this on Linux/Mono, because it makes IDataObject.GetFormats to throw an exception.
                    // Adding a clone here would prevent the exception, but the Bitmap format will not appear among the clipboard formats.
                    // And though Clipboard.Contains image will return true, Clipboard.GetImage would still return null.
                    if (!OSHelper.IsLinuxMono)
                        formats.Add(bitmapFormat, bitmap);
                    return;
                }

                // 3.a. Standard Format17 (DIBv5) format: the standard way of storing alpha image on the clipboard, though not too many applications handle it.
                // Only on Windows, because we need to populate the clipboard natively to add the Format17 format with the standard CF_DIBV5 ID
                // instead of a newly registered random one, and to auto generate the standard DeviceIndependentBitmap and Bitmap formats as well.
                if (hasAlpha && BitConverter.IsLittleEndian)
                {
                    try
                    {
                        var ms = new MemoryStream();
                        lock (bitmap)
                            SaveAsDibV5(bitmap, ms);
                        formats.Add(dibV5Format, ms);
                        return;
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        Debug.WriteLine($"Failed to copy as {dibV5Format}: {e.Message}");
                    }
                }

                // 3.b. Standard Bitmap format.
                // When using SetClipboardData, Windows automatically generates DeviceIndependentBitmap and Format17 (DIB/DIBv5) entries as well.
                // This is different from the managed Clipboard/DataObject.SetImage APIs, which don't generate these additional formats,
                // but add a BinaryFormatter-serialized System.Drawing.Bitmap entry (along with the standard Bitmap format).
                IntPtr hbmSrc = IntPtr.Zero;
                IntPtr hbmDst = IntPtr.Zero;
                try
                {
                    lock (bitmap)
                        hbmSrc = bitmap.GetHbitmap();
                    if (hbmSrc != IntPtr.Zero && (hbmDst = CreateCompatibleBitmap(hbmSrc, bitmap.Size)) != IntPtr.Zero)
                        formats.Add(bitmapFormat, hbmDst);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to copy as {bitmapFormat}: {e.Message}");
                }
                finally
                {
                    if (hbmSrc != IntPtr.Zero)
                        Gdi32.DeleteObject(hbmSrc);

                    // Ultimate fallback: indicating to use DataObject.SetImage when populating the clipboard
                    if (hbmDst == IntPtr.Zero)
                        formats.Add(bitmapFormat, bitmap);
                }
            }
            finally
            {
                if (!ReferenceEquals(bitmap, image))
                    bitmap.Dispose();
            }
        }

        private static IntPtr CreateCompatibleBitmap(IntPtr hbmSrc, Size size)
        {
            // The clipboard expects a bitmap created by CreateCompatibleBitmap. If we just returned the result of GetHbitmap,
            // retrieving the Bitmap format may cause even out of memory exception (sometimes just after terminating the source process),
            // and the generated DIB format (if it doesn't throw - may depend on the pixel format) might be interpreted incorrectly.
            IntPtr dcScreen = IntPtr.Zero;
            try
            {
                dcScreen = User32.GetDC(IntPtr.Zero);
                IntPtr dcSrc = Gdi32.CreateCompatibleDC(dcScreen);
                IntPtr prevSrc = Gdi32.SelectObject(dcSrc, hbmSrc);

                IntPtr dcDst = Gdi32.CreateCompatibleDC(dcScreen);
                IntPtr hbmDst = Gdi32.CreateCompatibleBitmap(dcScreen, size.Width, size.Height);
                IntPtr prevDst = Gdi32.SelectObject(dcDst, hbmDst);

                // copying the content and storing the result
                Gdi32.BitBlt(dcDst, dcSrc, size);

                // cleanup
                Gdi32.SelectObject(dcSrc, prevSrc);
                Gdi32.SelectObject(dcDst, prevDst);
                Gdi32.DeleteDC(dcSrc);
                Gdi32.DeleteDC(dcDst);

                return hbmDst;
            }
            finally
            {
                if (dcScreen != IntPtr.Zero)
                    User32.ReleaseDC(IntPtr.Zero, dcScreen);
            }
        }

        private static void SaveAsDibV5(Bitmap bitmap, MemoryStream ms)
        {
            using IReadableBitmapData bitmapData = bitmap.GetReadableBitmapData();
            Debug.Assert(bitmapData.PixelFormat.HasAlpha || bitmapData.Palette?.HasAlpha == true, "Saving as DIBv5 is expected only when the bitmap has alpha");

            // Not in using to leave the stream open (on older frameworks there is no leaveOpen parameter)
            var writer = new BinaryWriter(ms);

            // NOTE: The SetClipboardData Windows API behaves inconsistently when it auto-generates bitmap formats:
            // - When setting an HBitmap with CF_BITMAP, the compression of the auto-generated DIB streams will be BI_BITFIELDS, and the DIBv5 stream contains the extra 12 bytes
            //   between the V5 header and the actual content. It seems to be consistent with the docs at https://learn.microsoft.com/en-us/windows/win32/gdi/bitmap-header-types
            //   ("The red, green, and blue bitfield masks for BI_BITFIELD bitmaps immediately follow the BITMAPINFOHEADER, BITMAPV4HEADER, and BITMAPV5HEADER structures.
            //   The BITMAPV4HEADER and BITMAPV5HEADER structures contain additional members for red, green, and blue masks") - but some apps handle it incorrectly, such as Paint.NET.
            // - But when we set an HGlobal with CF_DIBV5, and we configure the BI_BITFIELDS compression and add the three bmiColors entries after the header exactly the same way
            //   as Windows generates it, the auto generated DIB and Bitmap formats will be corrupted, containing those BITMAPINFO.bmiColors entries in the image itself.
            //   This corrupts most applications that prefer obtaining the Bitmap/DIB formats instead of Format17 (almost every app that can paste bitmaps).
            // Therefore, we set BI_RGB compression instead of BI_BITFIELDS to remove the ambiguity whether the 3 entries of BITMAPINFO.bmiColors are needed after the header.
            // They are redundant anyway, as the V5 header already contains the RGBA masks. The disadvantage of this can be that now consuming apps may ignore the masks
            // in the V5 header, treating the bitmap as if it had no alpha at all.
            var header = new BITMAPV5HEADER
            {
                bV5Width = bitmapData.Width,
                bV5Height = bitmapData.Height,
                bV5Planes = 1,
                bV5BitCount = 32,
                bV5Compression = Constants.BI_RGB, // see the comments above
                bV5SizeImage = (uint)(bitmapData.Width * bitmapData.Height * 4),
                bV5RedMask = 0x00FF0000,
                bV5GreenMask = 0x0000FF00,
                bV5BlueMask = 0x000000FF,
                bV5AlphaMask = 0xFF000000,
                bV5CSType = Constants.LCS_sRGB,
                bV5Intent = Constants.LCS_GM_GRAPHICS
            };
            unsafe { header.bV5Size = (uint)sizeof(BITMAPV5HEADER); }
            writer.Write(BinarySerializer.SerializeValueType(header));

            IReadableBitmapDataRowMovable? row = null;
            for (int y = bitmapData.Height - 1; y >= 0; y--)
            {
                if (row == null)
                    row = bitmapData.GetMovableRow(y);
                else
                    row.MoveToRow(y);

                for (int x = 0; x < bitmapData.Width; x++)
                    writer.Write(row[x].ToArgb());
            }
        }

        private static void CopyTiff(Dictionary<string, object> formats, ImageInfo info, AsyncTaskBase task)
        {
            try
            {
                // If the main image is already a TIFF, using that one.
                var ms = new MemoryStream();
                if (info.Image is Image image && image.RawFormat.Guid == ImageFormat.Tiff.Guid)
                {
                    lock (image)
                        image.SaveAsTiff(ms, false);
                }
                else
                    info.IterateFrameImages(task).SaveAsMultipageTiff(ms);
                formats.Add(tiffFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as TIFF stream: {e.Message}");
            }
        }

        private static void CopyIcon(Dictionary<string, object> formats, ImageInfoBase info, AsyncTaskBase task)
        {
            try
            {
                var ms = new MemoryStream();
                if (info is ImageInfo imageInfo)
                    Icons.Combine(imageInfo.IterateFrameIcons(task)).Save(ms);
                else
                    info.GetCreateIcon()!.SaveAsIcon(ms);
                formats.Add(iconFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as ICO stream: {e.Message}");
            }
        }

        private static void CopyAnimGif(Dictionary<string, object> formats, ImageInfo info, AsyncTaskBase task)
        {
            try
            {
                // If the animation is not generated, this may take a lot of time (though the default quantizer is quite fast).
                var ms = new MemoryStream();
                Debug.Assert(info.Type == ImageInfoType.Animation);
                var config = new AnimatedGifConfiguration(info.IterateFramesBitmapData(task), info.Frames!.Select(f => TimeSpan.FromMilliseconds(f.Duration)))
                {
                    Size = info.Size,
                    SizeHandling = AnimationFramesSizeHandling.Center
                };

                // NOTE: Begin/End like this is alright, we are already on a pool thread. We could just use an EncodeAnimation overload with ParallelConfig if existed.
                var asyncConfig = new AsyncConfig { IsCancelRequestedCallback = () => task.IsCanceled, ThrowIfCanceled = false };
                GifEncoder.EndEncodeAnimation(GifEncoder.BeginEncodeAnimation(config, ms, asyncConfig));
                if (task.IsCanceled)
                    return;
                ms.Position = 0L;
                info.Image = new Bitmap(ms);
                if (task.IsCanceled)
                    return;

                formats.Add(gifFormat, ms);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to copy as TIFF stream: {e.Message}");
            }
        }

        internal static ImageInfo? DoTryPasteFromClipboard(string? format, AllowedImageTypes allowedTypes, bool allowMultiFrame, bool detectAlpha, AsyncTaskContext task)
        {
            Debug.Assert(allowedTypes != AllowedImageTypes.None);

            try
            {
                // Not using Clipboard.GetImage (or only as a fallback), because it always gets Bitmap format only, which is an RGB32 format with no alpha.
                IWinFormsDataObject? dataObject = null;
                HashSet<string>? formats = null;
                task.Context.Send(_ =>
                {
                    // Though GetFormats would not throw from a worker thread, it does not always return native formats from non UI-threads.
                    formats = new(format != null ? [format] : GetClipboardFormats());
                    dataObject = Clipboard.GetDataObject();
                }, null);

                if (dataObject == null || formats == null || task.IsCanceled)
                    return null;

                Debug.WriteLine($"Clipboard formats: {formats.Join(", ")}");
                bool preferMetafile = (allowedTypes & AllowedImageTypes.Metafile) != 0;

                // 1. Metafile
                if (preferMetafile && TryGetMetafile(formats, dataObject, allowedTypes, task, out ImageInfo? imageInfo))
                    return imageInfo;
                if (task.IsCanceled)
                    return null;

                // 2. Multiframe Bitmap or Icon with custom encoding
                if (allowMultiFrame)
                {
                    // 2.a. TIFF or Icon
                    if (TryGetImageFromStream(formats.Intersect([tiffFormat, iconFormat]), dataObject, allowedTypes, true, task, out imageInfo))
                        return imageInfo;

                    // 2.b. [animated] GIF
                    if (formats.Contains(gifFormat) && TryGetImageFromStream([gifFormat], dataObject, allowedTypes, true, task, out imageInfo))
                    {
                        // if a single-frame GIF found, accepting it if there is no PNG on the clipboard as well
                        if (imageInfo!.Type == ImageInfoType.Animation || !formats.Contains(pngFormat))
                            return imageInfo;

                        // dropping the single frame GIF to obtain the PNG instead
                        imageInfo.Dispose();
                    }
                }
                // 2.c. icon frame
                else if (allowedTypes == AllowedImageTypes.Icon && TryGetImageFromStream(formats.Intersect(iconFormats), dataObject, allowedTypes, false, task, out imageInfo))
                    return imageInfo;

                if (task.IsCanceled)
                    return null;

                // 3. Single-frame custom encoded Bitmap or Icon. The way of the Intersect call ensures the order of the tried formats.
                //    Not using bitmapFormats here because we include icon and exclude some other bitmap formats.
                string[] customBitmapFormats = [pngFormat, pngAliasFormat, tiffFormat, gifFormat, gifAliasFormat, iconFormat, jpegFormat, jpegAliasFormat];
                if (TryGetImageFromStream(customBitmapFormats.Intersect(formats), dataObject, allowedTypes, false, task, out imageInfo))
                    return imageInfo;
                if (task.IsCanceled)
                    return null;

                // 4. Standard DIB formats
                if (formats.Contains(dibV5Format) && TryGetDeviceIndependentBitmap(dibV5Format, dataObject, allowedTypes, detectAlpha, task, out imageInfo))
                    return imageInfo;
                if (formats.Contains(dibFormat) && TryGetDeviceIndependentBitmap(dibFormat, dataObject, allowedTypes, detectAlpha, task, out imageInfo))
                    return imageInfo;
                if (task.IsCanceled)
                    return null;

                // 5. Standard Bitmap format
                if (formats.Contains(bitmapFormat) && TryGetNativeBitmap(dataObject, allowedTypes, detectAlpha, task, out imageInfo))
                    return imageInfo;
                if (task.IsCanceled)
                    return null;

                // 6. Metafile as a fallback, when bitmaps are preferred
                if (!preferMetafile && TryGetMetafile(formats, dataObject, allowedTypes, task, out imageInfo))
                    return imageInfo;

                // 7. Ultimate fallback: Clipboard.GetImage - it attempts to process .NET Framework serialized System.Drawing.Bitmap entries,
                //    and also the standard Bitmap format if it wasn't processed natively above.
                if (formats.Intersect([bitmapFormat, typeof(Bitmap).FullName!]).Any())
                {
                    Image? image = null;
                    task.Context.Send(_ => image = Clipboard.GetImage(), null);
                    if (image != null)
                    {
                        imageInfo = ImageInfo.EnsureFormat(image, allowedTypes, allowMultiFrame, detectAlpha);
                        return imageInfo;
                    }
                }

                return null;

            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Error while trying to paste image from clipboard: {e.Message}");
                return null;
            }
        }

        private static bool TryGetNativeBitmap(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, bool detectAlpha, AsyncTaskContext task, out ImageInfo? result)
        {
            #region Local Methods

            static Bitmap? DoTryGetNativeBitmap(IComDataObject comDataObject)
            {
                var format = new FORMATETC
                {
                    cfFormat = (short)ClipboardFormat.CF_BITMAP,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_GDI
                };

                int hResult = comDataObject.QueryGetData(ref format);
                if (hResult != Constants.S_OK)
                {
                    Debug.WriteLine($"Failed to get native {bitmapFormat} format: HRESULT is {hResult}");
                    return null;
                }

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                try
                {
                    if (medium.tymed != TYMED.TYMED_GDI)
                    {
                        Debug.Fail($"Expected vs. actual format of {bitmapFormat}: {format.tymed} <-> {medium.tymed}");
                        return null;
                    }

                    // NOTE: Image.FromHBitmap always creates a copy, so we are safe even when we don't own the handle (that is, when medium.pUnkForRelease is not null).
                    // The managed GetData always clones the result here, but the docs says: "The FromHbitmap method makes a copy of the GDI bitmap; so you can release
                    // the incoming GDI bitmap using the GDI DeleteObject method immediately after creating the new Image."
                    return Image.FromHbitmap(medium.unionmember);
                }
                finally
                {
                    // If the type was TYMED_GDI, this may call Gdi32.DeleteObject, depending on medium.pUnkForRelease
                    Ole32.ReleaseStgMedium(ref medium);
                }
            }

            #endregion

            result = null;
            if (!OSHelper.IsWindows || dataObject is not IComDataObject comDataObject)
                return false;

            try
            {
                Bitmap? bitmap = null;
                task.Context.Send(_ => bitmap = DoTryGetNativeBitmap(comDataObject), null);
                result = ImageInfo.EnsureFormat(bitmap, allowedTypes, false, detectAlpha);
                return result != null;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get native {bitmapFormat} format: {e.Message}");
                return false;
            }
        }

        private static bool TryGetDeviceIndependentBitmap(string format, IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, bool detectAlpha, AsyncTaskContext task, out ImageInfo? result)
        {
            result = null;

            // Endianness check: because we just do casts instead of actual marshaling.
            // The layout of the structs ensure the same result on different architectures, exception with big-endian ones.
            if (!BitConverter.IsLittleEndian)
                return false;

            byte[]? buf = TryGetBytes(format, dataObject, task);
            if (buf == null)
                return false;

            try
            {
                unsafe
                {
                    int headerSize = format == dibFormat ? sizeof(BITMAPINFOHEADER)
                            : format == dibV5Format ? sizeof(BITMAPV5HEADER)
                            : throw new InvalidOperationException(Res.InternalError($"Unexpected format: {format}"));
                    if (buf.Length <= headerSize)
                        return false;

                    fixed (byte* pBuf = buf)
                    {
                        // Casting to the lowest version, which is compatible with V4 and V5 formats.
                        BITMAPINFOHEADER* header = (BITMAPINFOHEADER*)pBuf;
                        var size = new Size(header->biWidth, Math.Abs(header->biHeight));
                        if (header->biSize != headerSize || header->biPlanes != 1 || (uint)size.Width > UInt16.MaxValue || (uint)size.Height > UInt16.MaxValue)
                            return false;

                        // The number of RGBQUAD entries in BITMAPINFO.bmiColors that are after the header.
                        int paletteSize = (int)header->biClrUsed;
                        if (paletteSize == 0)
                        {
                            if (header->biCompression == Constants.BI_BITFIELDS)
                                paletteSize = 3;
                            else if (header->biBitCount is > 0 and < 16)
                                paletteSize = 1 << header->biBitCount;
                        }

                        int bitmapInfoSize = headerSize + paletteSize * 4;

                        // DIB stride is always divisible by 4, that's why we calculate with +31 and not +7
                        int stride = (((size.Width * header->biBitCount) + 31) & ~31) >> 3;
                        int contentSize = (int)header->biSizeImage;
                        if (contentSize == 0u && header->biCompression is Constants.BI_RGB or Constants.BI_BITFIELDS)
                            contentSize = stride * size.Height;

                        // We only checked the header size above, now validating also with BITMAPINFO.bmiColors length and the actual content size if available.
                        if ((uint)buf.Length < (uint)(bitmapInfoSize + contentSize))
                        {
                            // Workaround for DIBv5 with no bmiColors after the header (e.g. Paint.NET)
                            if (headerSize == sizeof(BITMAPV5HEADER) && header->biCompression == Constants.BI_BITFIELDS && buf.Length == headerSize + contentSize)
                                bitmapInfoSize = headerSize;
                            else
                                return false;
                        }

                        // 24/32 bpp uncompressed formats: Creating them without Windows API
                        if (header->biCompression is Constants.BI_RGB or Constants.BI_BITFIELDS && header->biBitCount is 24 or 32 && contentSize > 0)
                        {
                            bool hasAlpha = header->biBitCount == 32 && headerSize == sizeof(BITMAPV5HEADER) && ((BITMAPV5HEADER*)header)->bV5AlphaMask != 0u;
                            PixelFormat pixelFormat = header->biBitCount is 24 ? PixelFormat.Format24bppRgb
                                : hasAlpha ? PixelFormat.Format32bppArgb
                                : PixelFormat.Format32bppRgb;

                            if (!hasAlpha && header->biBitCount == 32 && detectAlpha)
                            {
                                // Officially DIB does not support uncompressed alpha bitmaps, and DIBv5 header may indicate no alpha even if it is actually present.
                                // NOTE: we must be careful, because x might be zeroed out, even if RGB is nonzero, in which case it's better assume that the image has no alpha.
                                //       Casting to Color32 is alright, because it has the same layout for the RGB values as the pixels in the 32-bit uncompressed DIB.
                                CastArray<byte, Color32> pixels = buf.AsSection(bitmapInfoSize).Cast<byte, Color32>();
                                Color32 firstPixel = pixels.GetElementReferenceUnsafe(0);
                                pixelFormat = firstPixel.A is Byte.MinValue or Byte.MaxValue ? PixelFormat.Format32bppRgb : PixelFormat.Format32bppArgb;

                                // First pixel is opaque: assuming no alpha unless there is a pixel with alpha
                                // First pixel is transparent: if the whole bitmap seems to be completely transparent, we assume no alpha
                                if (pixelFormat == PixelFormat.Format32bppRgb)
                                {
                                    for (int i = 1; i < pixels.Length; i++)
                                    {
                                        if (pixels.GetElementReferenceUnsafe(i).A != firstPixel.A)
                                        {
                                            pixelFormat = PixelFormat.Format32bppArgb;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Reinterpreting the buffer as a bitmap with the determined format.
                            // if height is positive, stride has to be negative due to DIBs bottom-up representation, and scan0 must point to the last row.
                            // As the wrapper bitmap does not own the buffer (that will go out of scope anyway), we must create a clone.
                            using var dibWrapper = header->biHeight > 0
                                ? new Bitmap(size.Width, size.Height, -stride, pixelFormat, (IntPtr)(pBuf + bitmapInfoSize + stride * (size.Height - 1)))
                                : new Bitmap(size.Width, size.Height, stride, pixelFormat, (IntPtr)(pBuf + bitmapInfoSize));
                            result = ImageInfo.EnsureFormat(dibWrapper.CloneCurrentFrame(), allowedTypes, false);
                            return true;
                        }

                        // Fallback case: letting Windows create the bitmap.
                        // We could actually process the content if the compression is BI_JPEG or BI_PNG, but they are not really expected here.
                        if (!OSHelper.IsWindows)
                            return false;

                        IntPtr dcScreen = User32.GetDC(IntPtr.Zero);
                        IntPtr dcDst = Gdi32.CreateCompatibleDC(dcScreen);
                        IntPtr hBitmap = IntPtr.Zero;
                        try
                        {
                            // We could use a BITMAPINFO type with an unsafe fixed uint bmiColors[256] field,
                            // and copy the calculated bitmapInfoSize number of bytes into it, but using pBuf makes it unnecessary.
                            hBitmap = Gdi32.CreateDibSection(dcDst, (IntPtr)pBuf, out IntPtr bits);
#if NET46_OR_GREATER
                            Buffer.MemoryCopy(pBuf + bitmapInfoSize, bits.ToPointer(), buf.Length - bitmapInfoSize, buf.Length - bitmapInfoSize);
#else
                            Marshal.Copy(buf, bitmapInfoSize, bits, buf.Length - bitmapInfoSize);
#endif
                            result = ImageInfo.EnsureFormat(Image.FromHbitmap(hBitmap), allowedTypes, false);
                            return true;
                        }
                        finally
                        {
                            if (hBitmap != IntPtr.Zero)
                                Gdi32.DeleteObject(hBitmap);
                            if (dcDst != IntPtr.Zero)
                                Gdi32.DeleteDC(dcDst);
                            if (dcScreen != IntPtr.Zero)
                                User32.ReleaseDC(IntPtr.Zero, dcScreen);
                        }
                    }
                }
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get native {dibFormat} format: {e.Message}");
                return false;
            }
        }

        private static bool TryGetMetafile(HashSet<string> formats, IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, AsyncTaskContext task, out ImageInfo? result)
        {
            // custom encodings first, and then by standard native formats
            if (TryGetImageFromStream(formats.Intersect([emfFormat, wmfFormat]), dataObject, allowedTypes, false, task, out result))
                return true;
            if (formats.Contains(enhMetafileFormat) && TryGetNativeEmf(dataObject, allowedTypes, task, out result))
                return true;
            if (formats.Contains(metafilePictFormat) && TryGetNativeWmf(dataObject, allowedTypes, task, out result))
                return true;

            return false;
        }

        private static bool TryGetNativeEmf(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, AsyncTaskContext task, out ImageInfo? result)
        {
            #region Local Methods

            static Metafile? DoTryGetNativeMetafile(IComDataObject comDataObject)
            {
                var format = new FORMATETC
                {
                    cfFormat = (short)ClipboardFormat.CF_ENHMETAFILE,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = TYMED.TYMED_ENHMF
                };

                int hResult = comDataObject.QueryGetData(ref format);
                if (hResult != Constants.S_OK)
                {
                    Debug.WriteLine($"Failed to get native {enhMetafileFormat} format: HRESULT is {hResult}");
                    return null;
                }

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                if (medium.tymed != TYMED.TYMED_ENHMF)
                {
                    Ole32.ReleaseStgMedium(ref medium);
                    Debug.Fail($"Expected vs. actual format of {enhMetafileFormat}: {format.tymed} <-> {medium.tymed}");
                    return null;
                }

                // If we do not own the medium, we create a copy to prevent the metafile from becoming unusable when the owner frees it up.
                // Disposing the result metafile calls DeleteEnhancedMetafile due to the deleteEmf parameter, so not calling ReleaseStgMedium here.
                IntPtr hEmf = medium.pUnkForRelease == null ? medium.unionmember : Gdi32.CopyEnhMetaFile(medium.unionmember);
                return new Metafile(hEmf, true);
            }

            #endregion

            result = null;
            if (!OSHelper.IsWindows || dataObject is not IComDataObject comDataObject)
                return false;

            try
            {
                Metafile? metafile = null;
                task.Context.Send(_ => metafile = DoTryGetNativeMetafile(comDataObject), null);
                result = ImageInfo.EnsureFormat(metafile, allowedTypes, false);
                return result != null;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get native {enhMetafileFormat} format: {e.Message}");
                return false;
            }
        }

        private static bool TryGetNativeWmf(IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, AsyncTaskContext task, out ImageInfo? result)
        {
            // After failing in every way of getting a valid metafile handle from CF_METAFILEPICT, it turns out that we can obtain CF_ENHMETAFILE,
            // even if it is not present on the clipboard, as Windows does the conversion for us. It's just we must do that on the UI thread to avoid "Invalid FORMATETC structure" error.
            return TryGetNativeEmf(dataObject, allowedTypes, task, out result);

            // If we really wanted, we could convert the result back to WMF:
            //TryGetNativeEmf(dataObject, allowedTypes, out result);
            //if (result?.Image is not Metafile metafile)
            //    return result != null;

            //try
            //{
            //    var ms = new MemoryStream();
            //    metafile.SaveAsWmf(ms);
            //    ms.Position = 0L;
            //    result.Dispose();
            //    result = new ImageInfo(new Metafile(ms));
            //    return true;
            //}
            //catch (Exception e) when (!e.IsCriticalGdi())
            //{
            //    return false;
            //}
        }

        private static bool TryGetImageFromStream(IEnumerable<string> formats, IWinFormsDataObject dataObject, AllowedImageTypes allowedTypes, bool allowMultiFrame, AsyncTaskContext task, out ImageInfo? imageInfo)
        {
            Debug.Assert(allowedTypes != AllowedImageTypes.None);

            foreach (string format in formats)
            {
                MemoryStream? stream = TryGetStream(format, dataObject, task);
                if (stream == null)
                    continue;

                try
                {
                    imageInfo = ImageInfo.EnsureFormat(format is iconFormat ? Icons.FromStream(stream) : Image.FromStream(stream), allowedTypes, allowMultiFrame);
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

        private static MemoryStream? TryGetStream(string format, IWinFormsDataObject dataObject, AsyncTaskContext task)
        {
            // 1. First, trying to use the native COM IDataObject interface on the current background thread.
            // This way we can only get HGlobal content, because IStream is usable on the UI thread only.
            IComDataObject? comDataObject = dataObject as IComDataObject;
            if (comDataObject != null && OSHelper.IsWindows)
            {
                if (TryGetBytesNative(format, comDataObject, TYMED.TYMED_HGLOBAL) is byte[] buf)
                    return new MemoryStream(buf);
            }

            // 2. Switching to the UI thread
            MemoryStream? result = null;
            task.Context.Send(_ =>
            {
                // 2.a. IStream via native COM IDataObject
                if (OSHelper.IsWindows && comDataObject != null)
                {
                    if (TryGetBytesNative(format, comDataObject, TYMED.TYMED_ISTREAM) is byte[] buf)
                    {
                        result = new MemoryStream(buf);
                        return;
                    }
                }

                // 2.b. Trying to use the managed IDataObject interface. This will attempt to obtain both HGlobal and IStream on the UI thread.
                result = TryGetStreamManaged(format, dataObject);
            }, null);

            return result is MemoryStream { Length: > 0L } ? result : null;
        }

        private static byte[]? TryGetBytes(string format, IWinFormsDataObject dataObject, AsyncTaskContext task)
        {
            // 1. First, trying to use the native COM IDataObject interface on the current background thread.
            // This way we can only get HGlobal content, because IStream is usable on the UI thread only.
            IComDataObject? comDataObject = dataObject as IComDataObject;
            if (comDataObject != null && OSHelper.IsWindows)
            {
                if (TryGetBytesNative(format, comDataObject, TYMED.TYMED_HGLOBAL) is byte[] buf)
                    return buf;
            }

            // 2. Switching to the UI thread
            byte[]? result = null;
            task.Context.Send(_ =>
            {
                // 2.a. IStream via native COM IDataObject
                if (OSHelper.IsWindows && comDataObject != null)
                {
                    if (TryGetBytesNative(format, comDataObject, TYMED.TYMED_ISTREAM) is byte[] buf)
                    {
                        result = buf;
                        return;
                    }
                }

                // 2.b. Trying to use the managed IDataObject interface. This will attempt to obtain both HGlobal and IStream on the UI thread.
                result = TryGetStreamManaged(format, dataObject) is MemoryStream { Length: > 0L } ms ? ms.ToArray() : null;
            }, null);

            return result;
        }

        private static byte[]? TryGetBytesNative(string format, IComDataObject comDataObject, TYMED mediumType)
        {
            Debug.Assert(OSHelper.IsWindows);
            try
            {
                var comFormat = new FORMATETC
                {
                    cfFormat = (short)new ClipboardFormatId(format).Id,
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = mediumType
                };

                int hResult = comDataObject.QueryGetData(ref comFormat);
                if (hResult != Constants.S_OK)
                {
                    Debug.WriteLine($"Failed to get COM {format} data: HRESULT is {hResult}");
                    return null;
                }

                comDataObject.GetData(ref comFormat, out STGMEDIUM medium);
                try
                {
                    switch (medium.tymed)
                    {
                        case TYMED.TYMED_ISTREAM:
                            return ReadComIStream(ref medium);

                        case TYMED.TYMED_HGLOBAL:
                            return ReadHGlobal(ref medium);

                        default:
                            // Not failing this time, so we can still try the fallback with the non-COM interface
                            Debug.WriteLine($"Expected vs. actual format of {format}: {comFormat.tymed} <-> {medium.tymed}");
                            return null;
                    }
                }
                finally
                {
                    // It's much simpler to release the medium this way than knowing what to do
                    // for the different TYMED and pUnkForRelease combinations.
                    Ole32.ReleaseStgMedium(ref medium);
                }
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get COM {format} data: {e.Message}");
                return null;
            }
        }

        private static MemoryStream? TryGetStreamManaged(string format, IWinFormsDataObject dataObject)
        {
            try
            {
#if NET10_0_OR_GREATER
                if (dataObject.TryGetData<MemoryStream>(ClipboardFormatId.ToWinFormsFormat(format), out MemoryStream? stream))
                    return stream;
                Debug.WriteLine($"Failed to get a stream for format {format}");
                return null;
#else
                object? data = dataObject.GetData(ClipboardFormatId.ToWinFormsFormat(format));
                switch (data)
                {
                    case MemoryStream stream:
                        return stream;

                    case null:
                        Debug.WriteLine($"Failed to get a stream for format {format}: GetData returned null");
                        return null;

                    default:
                        Debug.Fail($"Unexpected non-stream type for format '{format}': {data.GetType()}");
                        return null;
                }
#endif
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                Debug.WriteLine($"Failed to get a stream for format {format}: {e.Message}");
                return null;
            }
        }

        private static byte[]? ReadComIStream(ref STGMEDIUM medium)
        {
            Debug.Assert(OSHelper.IsWindows && medium.tymed == TYMED.TYMED_ISTREAM);
            IStream comStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
            Marshal.Release(medium.unionmember);
            comStream.Stat(out STATSTG stat, 0);
            if (stat.cbSize is <= 0 or > Constants.MaxArrayLength)
                return null;

            byte[] content = new byte[stat.cbSize];

            // Sometimes the stream position is at the end when we obtain the stream
            comStream.Seek(0, Constants.STREAM_SEEK_SET, IntPtr.Zero);
            int read;
            unsafe { comStream.Read(content, content.Length, (IntPtr)(&read)); }

            Debug.Assert(read == content.Length, "IStream.Read should not read less bytes than requested, unless the end of stream is reached.");
            return content;
        }

        private static byte[]? ReadHGlobal(ref STGMEDIUM medium)
        {
            Debug.Assert(OSHelper.IsWindows && medium.tymed == TYMED.TYMED_HGLOBAL);
            IntPtr ptrStream = Kernel32.GlobalLock(medium.unionmember);
            try
            {
                if (ptrStream == IntPtr.Zero)
                    return null;

                nuint size = Kernel32.GlobalSize(medium.unionmember);
                if (size is 0 or > Constants.MaxArrayLength)
                    return null;

                byte[] content = new byte[size];
                Marshal.Copy(ptrStream, content, 0, (int)size);
                return content;
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

        private static string[] GetClipboardFormats()
        {
            try
            {
                // Using Clipboard.GetDataObject on non-Windows platforms only.
                // On Linux/Mono this still may throw an exception (as well as Clipboard.ContainsImage) if we tried to use Clipboard/IDataObject.SetImage earlier:
                // XplatUI11.ClipboardAvailableFormats->BinaryFormatter.Serialize->Image.GetObjectData->Image.RawFormat->GDI+ error
                if (!OSHelper.IsWindows)
                    return Clipboard.GetDataObject()?.GetFormats(false).Select(ClipboardFormatId.FromWinFormsFormat).ToArray() ?? Reflector.EmptyArray<string>();

                // On Windows using pure WinAPI due to issues with DataFormats (see the description of ClipboardFormatId). Windows/Mono has also further issues:
                // - Clipboard.GetDataObject() may crash the runtime, not even try-catch helps. Callstack is unknown, only "Error: 6" is printed to the console.
                // - Clipboard.ContainsImage may cause that other processes cannot access the clipboard until the application is closed: OpenClipboard() returns ERROR_ACCESS_DENIED
                // - Calling DataFormats.GetFormat(format) with non-standard format closes the clipboard
                if (!User32.OpenClipboard())
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    string[] formats = new string[User32.CountClipboardFormats()];
                    ClipboardFormat format = ClipboardFormat.None;
                    for (int i = 0; i < formats.Length; i++)
                    {
                        format = User32.EnumClipboardFormats(format);
                        Debug.Assert(format != ClipboardFormat.None);
                        formats[i] = new ClipboardFormatId(format).Name;
                    }

                    Debug.WriteLine($"Clipboard formats: {formats.Join(", ")}");
                    return formats;
                }
                finally
                {
                    User32.CloseClipboard();
                }
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Debug.WriteLine($"Failed to obtain clipboard formats: {e.Message}");
                return Reflector.EmptyArray<string>();
            }
        }

        #endregion

        #endregion
    }
}