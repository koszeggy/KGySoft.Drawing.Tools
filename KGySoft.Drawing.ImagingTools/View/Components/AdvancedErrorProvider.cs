#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedErrorProvider.cs
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Reflection;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.WinForms;

#endregion

#region Suppressions

#if NETFRAMEWORK
#pragma warning disable CS8601 // Possible null reference assignment - false alarm, older frameworks handle String.IsNullOrEmpty incorrectly
#pragma warning disable CS8604 // Possible null reference argument - false alarm, older frameworks handle String.IsNullOrEmpty incorrectly
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// An error provider that supports custom rendering of tooltips.
    /// </summary>
    internal class AdvancedErrorProvider : KGySoft.WinForms.Components.AdvancedErrorProvider
    {
        #region Nested classes

        #region ToolTipWindow class

        private sealed class ToolTipWindow : NativeWindow
        {
            #region Fields

            private Font? toolTipFont;

            #endregion

            #region Properties

            internal string Message { get; set; }

            #endregion

            #region Constructors

            internal ToolTipWindow(IntPtr handle, string message)
            {
                Message = message;
                AssignHandle(handle);
            }

            #endregion

            #region Methods

            #region Public Methods

            public override void ReleaseHandle()
            {
                base.ReleaseHandle();
                toolTipFont?.Dispose();
                toolTipFont = null;
            }

            #endregion

            #region Protected Methods

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case Constants.WM_PAINT:
                        Debug.Assert(m.HWnd == Handle);
                        base.WndProc(ref m);
                        using (Graphics g = Graphics.FromHwnd(Handle))
                        {
                            new DrawToolTipEventArgs(g, null, null, User32.GetClientRect(Handle), Message,
                                default, default, GetFont(Handle)).DrawToolTipAdvanced();
                            break;
                        }

                    case Constants.WM_DPICHANGED:
                        base.WndProc(ref m);
                        toolTipFont?.Dispose();
                        toolTipFont = null;
                        break;

                    default:
                        base.WndProc(ref m);
                        return;
                }
            }

            #endregion

            #region Private Methods

            private Font GetFont(IntPtr hwnd)
            {
                if (toolTipFont == null)
                {
                    try
                    {
                        toolTipFont = Font.FromHfont(User32.SendMessage(hwnd, Constants.WM_GETFONT, IntPtr.Zero, IntPtr.Zero));
                    }
                    catch (ArgumentException)
                    {
                        // If the current default tooltip font is a non-TrueType font, then
                        // Font.FromHfont throws this exception, so fall back to the default control font.
                        toolTipFont = SystemFonts.MessageBoxFont ?? (Font)ScaleHelper.DefaultFont.Clone();
                    }
                }

                return toolTipFont;
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private Dictionary<Control, ToolTipWindow>? customToolTipWindow;
        private bool isCustomRendering;

        #endregion

        #region Properties

        private IDictionary? Items => this.TryGetItems();

        #endregion

        #region Constructors

        internal AdvancedErrorProvider(IContainer container)
            : base(container)
        {
            ResetAppearance();
        }

        #endregion

        #region Methods

        #region Public Methods

        public new void SetError(Control control, string? value)
        {
            ToolTipWindow? customWindow;
            if (String.IsNullOrEmpty(value))
            {
                if (customToolTipWindow?.TryRemove(control, out customWindow) == true)
                    customWindow.ReleaseHandle();

                base.SetError(control, value);
                return;
            }

            base.SetError(control, value);

            if (!isCustomRendering || control.Parent == null)
                return;

            // Initializing custom hooks for the tooltip window of the ErrorProvider for custom rendering
            // NativeWindow tipWindow = ((ErrorWindow)EnsureErrorWindow(control.Parent)).*tipWindow*;
            if (this.TryGetNativeWindow(control.Parent) is not NativeWindow tipWindow)
                return;

            customToolTipWindow ??= new Dictionary<Control, ToolTipWindow>();
            if (customToolTipWindow.TryGetValue(control, out customWindow))
            {
                if (tipWindow.Handle != customWindow.Handle)
                {
                    customWindow.ReleaseHandle();
                    customWindow.AssignHandle(tipWindow.Handle);
                }

                customWindow.Message = value;
            }
            else
                customToolTipWindow.Add(control, new ToolTipWindow(tipWindow.Handle, value));
        }

        #endregion

        #region Internal Methods

        internal void ResetAppearance()
        {
            bool customToolTip = !OSHelper.IsFrameworkMono
                && ThemeColors.IsSet(ThemeColor.ToolTip) || ThemeColors.IsSet(ThemeColor.ToolTipText) || ThemeColors.IsSet(ThemeColor.ToolTipBorder);
            if (isCustomRendering == customToolTip)
                return;

            Debug.Assert(isCustomRendering || (customToolTipWindow?.Count).GetValueOrDefault() == 0);
            isCustomRendering = customToolTip;

            // turning custom tooltips on/off: resetting all tooltips
            IDictionary? items = Items;
            if (items == null)
                return;

            foreach (Control control in items.Keys)
            {
                string message = GetError(control);
                if (!String.IsNullOrEmpty(message))
                    SetError(control, message);
            }
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Dictionary<Control, ToolTipWindow>? customToolTips = customToolTipWindow;
                if (customToolTips == null)
                    return;
                foreach (ToolTipWindow customWindow in customToolTips.Values)
                    customWindow.ReleaseHandle();
                customToolTipWindow = null;
            }
        }

        #endregion

        #endregion
    }
}
