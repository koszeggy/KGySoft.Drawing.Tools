#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Constants.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Drawing.ImagingTools.WinApi
{
    internal static class Constants
    {
        #region Constants
        // ReSharper disable InconsistentNaming

        internal const int S_OK = 0;

        internal const int WM_PAINT = 0x0F;
        internal const int WM_MOUSEACTIVATE = 0x021;
        internal const int WM_INITDIALOG = 0x0110;
        internal const int WM_GETFONT = 0x0031;
        internal const int WM_NCCALCSIZE = 0x0083;
        internal const int WM_NCPAINT = 0x0085;
        internal const int WM_NCACTIVATE = 0x0086;
        internal const int WM_DPICHANGED_BEFOREPARENT = 0x02E2;
        internal const int WM_DPICHANGED_AFTERPARENT = 0x02E3;
        internal const int WM_DRAWCLIPBOARD = 0x0308;
        internal const int WM_CHANGECBCHAIN = 0x030D;
        internal const int WM_THEMECHANGED = 0x031A;
        internal const int WM_CLIPBOARDUPDATE = 0x031D;

        internal const nint MA_ACTIVATEANDEAT = 2;
        internal const nint MA_ACTIVATE = 1;

        internal const int SWP_NOSIZE = 0x0001;
        internal const int SWP_NOMOVE = 0x0002;
        internal const int SWP_NOZORDER = 0x0004;
        internal const int SWP_NOACTIVATE = 0x0010;
        internal const int SWP_FRAMECHANGED = 0x0020;  // The frame changed: send
        internal const int SWP_DRAWFRAME = SWP_FRAMECHANGED;

        internal const int DCX_WINDOW = 0x00000001;
        internal const int DCX_USESTYLE = 0x00010000;

        internal const int WH_CALLWNDPROCRET = 12;

        internal const uint GMEM_MOVEABLE = 2;
        internal const uint GMEM_SHARE = 0x2000;

        internal const short MM_ANISOTROPIC = 8;
        
        internal const uint SRCCOPY = 0x00CC0020;

        internal const int STREAM_SEEK_SET = 0;

        //// Needed for classic MessageBoxes. Restore if reverting from TaskDialogs
        //internal const int IDOK = 1;
        //internal const int IDCANCEL = 2;

        // ReSharper restore InconsistentNaming

        internal const string ClassNameDialogBox = "#32770";
        internal const string ClassNameButton = "Button";
        internal const string ClassNameStatic = "Static";

        internal const int MaxArrayLength = 0x7FFFFFC7;

        internal const int DIB_RGB_COLORS = 0;
        
        internal const uint BI_RGB = 0;
        internal const uint BI_BITFIELDS = 3;

        #endregion
    }
}