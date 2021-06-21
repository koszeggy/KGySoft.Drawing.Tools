#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Constants.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
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