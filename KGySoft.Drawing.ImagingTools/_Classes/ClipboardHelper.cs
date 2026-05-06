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
            // NOTE: Using SetImage as a fallback solution only, because it does not support the native metafile formats, and puts a BinaryFormatter entry on the clipboard
            // a with potentially poorly chosen encoder (supporting such formats in TryPasteFromClipboard, though).
            // Instead, preparing the native Bitmap format as a well-known fallback (which is supported by the managed clipboard API as well) explicitly.
            try
            {
                // Value is IntPtr or MemoryStream. Using stream is better than byte[], because in the native API stream is handled natively,
                // whereas byte[] is serialized in a BinaryFormatter compatible way.
                var formats = new Dictionary<string, object>();
                Image? image = null;

                // compound image or single frame image
                if (info is ImageInfo imageInfo)
                {
                    switch (imageInfo.Type)
                    {
                        case ImageInfoType.None:
                            Debug.Fail("Copying expected to be disabled");
                            return;

                        case ImageInfoType.SingleImage when imageInfo.Image is Metafile metafile:
                            image = metafile;
                            CopyMetafile(formats, metafile);
                            break;

                        case ImageInfoType.Pages:
                            // TODO: TIFF
                            break;

                        case ImageInfoType.MultiRes:
                        case ImageInfoType.Icon:
                            // TODO: ICON
                            break;

                        case ImageInfoType.Animation:
                            // TODO: animgif
                            break;
                    }

                }

                // TODO: HBitmap, DIB

                if (formats.Count != 0)
                    PopulateClipboard(formats);
                else if (image != null)
                    Clipboard.SetImage(image);
            }
            catch (Exception e) when (!e.IsCritical())
            {
                Dialogs.WarningMessage(Res.WarningMessageCannotCopyClipboard);
            }
        }

        internal static bool TryPasteFromClipboard(AllowedImageTypes imageTypes, bool allowMultiFrame, out ImageInfo? imageInfo)
        {
            Debug.Assert(imageTypes != AllowedImageTypes.None);
            imageInfo = null;

            try
            {
                // Not using Clipboard.GetImage (or only as a fallback), because it always gets Bitmap format only, which is an RGB32 format with no alpha.
                IWinFormsDataObject? dataObject = Clipboard.GetDataObject();
                if (dataObject == null)
                    return false;

                var formats = new HashSet<string>(dataObject.GetFormats(false));

                // 1. Metafile is enabled: trying to obtain a Metafile in the first place
                if ((imageTypes & AllowedImageTypes.Metafile) != AllowedImageTypes.None)
                {
                    // 1.a. Metafile stream as we copy it.
                    if (TryGetMetafileStream(formats.Intersect([emfFormat, wmfFormat]), dataObject, out Metafile? metafile))
                    {
                        imageInfo = new ImageInfo(metafile);
                        return true;
                    }

                    // 1.b. native EMF
                    if (formats.Contains(DataFormats.EnhancedMetafile) && TryGetNativeEmf(dataObject, out metafile))
                    {
                        imageInfo = new ImageInfo(metafile);
                        return true;
                    }

                    // 1.c. native WMF
                    if (formats.Contains(DataFormats.MetafilePict) && TryGetNativeWmf(dataObject, out metafile))
                    {
                        imageInfo = new ImageInfo(metafile);
                        return true;
                    }

                    // 1.d. Metafile stream - NOTE: .NET copies metafiles with System.Drawing.Bitmap format (though the actual raw format is PNG).
                    // Not using it, as we don't put metafiles to the clipboard like this, and it is indicated by the same format as bitmaps, which is handled later.
                    // Metafiles with the System.Drawing.Bitmap format are handled when attempting to deserialize bitmaps.
                    //if (TryGetMetafileStream(formats.Intersect([typeof(Metafile).FullName!, typeof(Bitmap).FullName!]), dataObject, out metafile))
                    //{
                    //    imageInfo = new ImageInfo(metafile);
                    //    return true;
                    //}
                }

                // 2.) Icon is enabled
                // TODO

                // 3.) Bitmap is enabled
                // TODO

                // 4.) Ultimate fallback: Clipboard.GetImage - normally we should not reach this point, but on Mono the lower level interfaces may not be implemented completely.
                Image? image = Clipboard.GetImage();
                if (image != null)
                    imageInfo = new ImageInfo(image);
                return imageInfo != null;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                return false;
            }
        }

        #endregion

        #region Private Methods

        private static void OnClipboardChanged() => clipboardChangedHandler?.Invoke(null, EventArgs.Empty);

        private static void PopulateClipboard(Dictionary<string, object> formats)
        {
            bool useNativeApi = OSHelper.IsWindows && formats.ContainsKey(DataFormats.EnhancedMetafile);
            if (useNativeApi && PopulateClipboardNatively(formats))
                return;

            var dataObject = new DataObject();
            foreach (KeyValuePair<string, object> item in formats)
                dataObject.SetData(item.Key, false, item.Value);

            Clipboard.SetDataObject(dataObject, true);
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
                            _ => throw new InvalidOperationException(Res.InternalError($"Unexpected clipboard format: {item.Key}: {item.Value.GetType()}"))
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
            if (isEmf == true)
                metafile.SaveAsEmf(ms);
            else
                metafile.SaveAsWmf(ms);
            formats.Add(isEmf == true ? emfFormat : wmfFormat, ms);

            if (!OSHelper.IsWindows)
                return;

            // 2. Standard EnhancedMetafile format, which is compatible with many applications.
            // Windows automatically generates a MetaFilePict entry as well (though pasting it does not work, at least on x64 systems).
            // GetHenhmetafile makes the original image unusable, so we clone it first.
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

        private static bool TryGetNativeEmf(IWinFormsDataObject dataObject, out Metafile? metafile)
        {
            metafile = null;
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

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                if (medium.tymed != TYMED.TYMED_ENHMF)
                {
                    Debug.Fail($"Expected vs. actual format of {DataFormats.EnhancedMetafile}: {format.tymed} <-> {medium.tymed}");
                    return false;
                }

                metafile = new Metafile(medium.unionmember, medium.pUnkForRelease is null);
                return true;
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                return false;
            }
        }

        private static bool TryGetNativeWmf(IWinFormsDataObject dataObject, out Metafile? metafile)
        {
            metafile = null;
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

                comDataObject.GetData(ref format, out STGMEDIUM medium);
                if (medium.tymed != TYMED.TYMED_MFPICT)
                {
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
                    metafile = new Metafile(ms);
                    return true;

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
                    if (medium.pUnkForRelease == null && hmf != IntPtr.Zero)
                        Gdi32.DeleteMetaFile(hmf);
                    Kernel32.GlobalUnlock(medium.unionmember);
                }
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                return false;
            }
        }

        private static bool TryGetMetafileStream(IEnumerable<string> formats, IWinFormsDataObject dataObject, out Metafile? metafile)
        {
            metafile = null;
            foreach (string format in formats)
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

                        if (comDataObject.QueryGetData(ref comFormat) == Constants.S_OK)
                        {
                            comDataObject.GetData(ref comFormat, out STGMEDIUM medium);
                            switch (medium.tymed)
                            {
                                case TYMED.TYMED_ISTREAM:
                                    metafile = new Metafile(ComIStreamToStream(ref medium));
                                    return true;

                                case TYMED.TYMED_HGLOBAL:
                                    MemoryStream? stream = HGlobalToStream(ref medium);
                                    if (stream == null)
                                        break;
                                    metafile = new Metafile(stream);
                                    return true;

                                default:
                                    // Not failing this time, so we can still try the fallback with the non-COM interface
                                    Debug.WriteLine($"Expected vs. actual format of {format}: {comFormat.tymed} <-> {medium.tymed}");
                                    break;
                            }
                        }
                    }
                    catch (Exception e) when (!e.IsCriticalGdi())
                    {
                        Debug.WriteLine($"Failed to get metafile from COM {format} stream: {e.Message}");
                    }
                }

                // 2. Trying to use the managed IDataObject interface
                try
                {
                    object? data = dataObject.GetData(format);
                    switch (data)
                    {
                        case MemoryStream ms:
                            metafile = new Metafile(ms);
                            return true;

                        case null:
                            continue;

                        default:
                            Debug.Fail($"Unhandled metafile content type: {data.GetType()}");
                            continue;
                    }
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    Debug.WriteLine($"Failed to get metafile by IDataObject.GetData({format}): {e.Message}");
                }
            }

            return false;
        }

        private static MemoryStream ComIStreamToStream(ref STGMEDIUM medium)
        {
            Debug.Assert(medium.tymed == TYMED.TYMED_ISTREAM);
            IStream comStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
            try
            {
                Marshal.Release(medium.unionmember);
                comStream.Stat(out STATSTG stat, 0);
                byte[] content = new byte[stat.cbSize];

                // NOTE: No need to check pcbRead, because if the requested bytes is not larger than the stream size, it always reads all requested bytes.
                // See also https://learn.microsoft.com/en-us/windows/win32/api/objidl/nf-objidl-isequentialstream-read
                comStream.Read(content, content.Length, IntPtr.Zero);
                return new MemoryStream(content);
            }
            finally
            {
                Marshal.ReleaseComObject(comStream);
            }
        }

        private static MemoryStream? HGlobalToStream(ref STGMEDIUM medium)
        {
            Debug.Assert(medium.tymed == TYMED.TYMED_HGLOBAL);
            IntPtr ptrStream = Kernel32.GlobalLock(medium.unionmember);
            if (ptrStream == IntPtr.Zero)
                return null;
            try
            {
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