#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Constants.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
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

        internal const int WS_BORDER = 0x00800000;

        internal const int WM_PAINT = 0x0F;
        internal const int WM_MOUSEACTIVATE = 0x021;
        internal const int WM_INITDIALOG = 0x0110;
        internal const int WM_MOUSEHWHEEL = 0x020E;

        internal const int MA_ACTIVATEANDEAT = 2;
        internal const int MA_ACTIVATE = 1;
#if !NET5_0_OR_GREATER
        internal const int WM_NCHITTEST = 0x0084;
#endif
        internal const int WM_NCPAINT = 0x0085;
        internal const int WM_NCACTIVATE = 0x0086;
        internal const int WM_THEMECHANGED = 0x031A;

        internal const int SWP_NOSIZE = 0x0001;
        internal const int SWP_NOMOVE = 0x0002;
        internal const int SWP_NOZORDER = 0x0004;
        internal const int SWP_NOACTIVATE = 0x0010;
        internal const int SWP_FRAMECHANGED = 0x0020;  // The frame changed: send
        internal const int SWP_DRAWFRAME = SWP_FRAMECHANGED;

        internal const int WH_CALLWNDPROCRET = 12;

        internal const int IDOK = 1;
        internal const int IDCANCEL = 2;

        internal const string ClassNameDialogBox = "#32770";
        internal const string ClassNameButton = "Button";
        internal const string ClassNameStatic = "Static";

        // ReSharper restore InconsistentNaming
        #endregion
    }
}