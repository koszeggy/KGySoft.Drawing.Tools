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

                    // Stopping buffered animations in dark mode to avoid flickering, which is especially apparent when it's embedded into a WPF control (modern visualizers).
                    if (Application.RenderWithVisualStyles)
                        UxTheme.BufferedPaintStopAllAnimations(Handle);
                    PaintDarkNCArea(m.WParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InvalidateNC();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            InvalidateNC();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            InvalidateNC();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // To auto apply the theme for the scrollbars. May occur after RTL change, which recreates the handle.
            if (ThemeColors.IsThemeEverChanged && Multiline)
                this.ApplyTheme();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (ThemeColors.IsThemeEverChanged)
                this.ApplyTheme();
        }

        #endregion

        #region Private Methods

        private void InvalidateNC()
        {
            if (ThemeColors.IsDarkBaseTheme && Multiline)
                User32.InvalidateNC(Handle);
        }

        private void PaintDarkNCArea(IntPtr hRgn)
        {
            var hWnd = Handle;
            IntPtr hDC = User32.GetNonClientDC(hWnd, hRgn);
            try
            {
                using var g = Graphics.FromHdc(hDC);
                Color color = isHovered ? ThemeColors.FixedSingleBorder : ThemeColors.FixedSingleBorderInactive;
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                g.DrawRectangle(color.GetPen(), rect);
                rect.Inflate(-1, -1);
                color = Color.FromArgb(unchecked((int)0xFF383838));
                g.DrawRectangle(color.GetPen(), rect);
            }
            finally
            {
                if (hRgn == (IntPtr)1)
                    User32.ReleaseDC(hWnd, hDC);
            }

            // Invalidating the ClientArea because sometimes the NC repaint corrupts the content
            Invalidate();
        }

        #endregion

        #endregion
    }
}