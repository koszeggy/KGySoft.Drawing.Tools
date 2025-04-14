#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedTextBox.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.WinApi;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Controls
{
    /// <summary>
    /// Just a TextBox that
    /// - allows Ctrl+A even if auto appending is enabled
    /// - fixes the rendering in dark mode when Multiline is true
    /// </summary>
    internal class AdvancedTextBox : TextBox
    {
        #region Fields

        private bool isHovered;

        #endregion

        #region Methods

        #region Protected Methods

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.A when ShortcutsEnabled:
                    SelectAll();
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Constants.WM_NCPAINT:
                    if (!ThemeColors.IsDarkBaseTheme || !Multiline)
                        goto default;

                    base.WndProc(ref m);
                    PaintDarkNCArea();
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (ThemeColors.IsDarkBaseTheme && Multiline)
                InvalidateNC();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            if (ThemeColors.IsDarkBaseTheme && Multiline)
                InvalidateNC();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            if (ThemeColors.IsDarkBaseTheme && Multiline)
                InvalidateNC();
        }

        #endregion

        #region Private Methods

        private void InvalidateNC()
        {
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0,
                Constants.SWP_NOMOVE | Constants.SWP_NOSIZE | Constants.SWP_NOZORDER |
                Constants.SWP_NOACTIVATE | Constants.SWP_DRAWFRAME);
        }

        private void PaintDarkNCArea()
        {
            var hWnd = Handle;
            var hDC = User32.GetWindowDC(hWnd);
            try
            {
                using var g = Graphics.FromHdc(hDC);
                using var pen = new Pen(Color.FromArgb(unchecked((int)(isHovered ? 0xFFC8C8C8 : 0xFF9B9B9B))));
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.DrawRectangle(pen, rect);
                rect.Inflate(-1, -1);
                pen.Color = Color.FromArgb(unchecked((int)0xFF383838));
                g.DrawRectangle(pen, rect);
            }
            finally
            {
                User32.ReleaseDC(hWnd, hDC);
            }
        }

        #endregion

        #endregion
    }
}