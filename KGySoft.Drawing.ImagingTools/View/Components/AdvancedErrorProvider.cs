#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdvancedErrorProvider.cs
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Components
{
    /// <summary>
    /// An error provider that supports custom rendering of tooltips.
    /// </summary>
    internal class AdvancedErrorProvider : ErrorProvider
    {
        #region Nested classes

        #region ToolTipInfo class

        private sealed class ToolTipInfo
        {
            #region Fields

            internal string? Message;
            internal IntPtr Hook;

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        #region Static Fields

        private static FieldAccessor? itemsField;
        private static MethodAccessor? ensureErrorWindowMethod;
        private static FieldAccessor? tipWindowField;

        private static bool itemsFieldInitialized;
        private static bool ensureErrorWindowMethodInitialized;
        private static bool tipWindowFieldInitialized;

        #endregion

        #region Instance Fields

        // This delegate is stored as a field to prevent its possible garbage collection while used by P/Invoke call.
        private readonly HOOKPROC callWndRetProc;
        private readonly Dictionary<Control, NativeWindow> errorWindows = new();
        private readonly Dictionary<IntPtr, ToolTipInfo> toolTipInfo = new();

        private bool isCustomRendering;
        private Font? toolTipFont;

        #endregion

        #endregion

        #region Properties

        #region Static Properties

        private static FieldAccessor? ItemsField
        {
            get
            {
                if (!itemsFieldInitialized)
                {
                    try
                    {
                        FieldInfo? fld = typeof(ErrorProvider).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("items", StringComparison.Ordinal));
                        if (fld != null)
                            itemsField = FieldAccessor.GetAccessor(fld);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        itemsField = null;
                    }
                    finally
                    {
                        itemsFieldInitialized = true;
                    }
                }

                return itemsField;
            }
        }

        private static MethodAccessor? EnsureErrorWindowMethod
        {
            get
            {
                if (!ensureErrorWindowMethodInitialized)
                {
                    try
                    {
                        MethodInfo? method = typeof(ErrorProvider).GetMethod("EnsureErrorWindow", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (method != null)
                            ensureErrorWindowMethod = MethodAccessor.GetAccessor(method);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        ensureErrorWindowMethod = null;
                    }
                    finally
                    {
                        ensureErrorWindowMethodInitialized = true;
                    }
                }

                return ensureErrorWindowMethod;
            }
        }

        private static FieldAccessor? TipWindowField
        {
            get
            {
                if (!tipWindowFieldInitialized)
                {
                    try
                    {
                        Type? errorWindowType = typeof(ErrorProvider).GetNestedType("ErrorWindow", BindingFlags.NonPublic);
                        FieldInfo? fld = errorWindowType?.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(f => f.Name.Contains("tipWindow", StringComparison.Ordinal));
                        if (fld != null)
                            tipWindowField = FieldAccessor.GetAccessor(fld);
                    }
                    catch (Exception e) when (!e.IsCritical())
                    {
                        tipWindowField = null;
                    }
                    finally
                    {
                        tipWindowFieldInitialized = true;
                    }
                }

                return tipWindowField;
            }
        }

        #endregion

        #region Instance Properties

        private IDictionary? Items => ItemsField?.GetInstanceValue<ErrorProvider, IDictionary>(this);

        #endregion

        #endregion

        #region Constructors

        internal AdvancedErrorProvider(IContainer container)
            : base(container)
        {
            callWndRetProc = ToolTipWindowHookProc;
            ResetAppearance();
        }

        #endregion

        #region Methods

        #region Public Methods

        public new void SetError(Control control, string? value)
        {
            if (String.IsNullOrEmpty(value))
            {
                if (errorWindows.TryRemove(control, out var oldWindow))
                    RemoveHook(oldWindow);

                base.SetError(control, value);
                return;
            }

            base.SetError(control, value);

            if (!isCustomRendering || control.Parent == null)
                return;

            // Initializing custom hooks for the tooltip window of the ErrorProvider for custom rendering
            // NativeWindow tipWindow = ((ErrorWindow)EnsureErrorWindow(control.Parent)).*tipWindow*;
            object? errorWindow = EnsureErrorWindowMethod?.InvokeInstanceFunction<ErrorProvider, Control, object>(this, control.Parent);
            if (errorWindow == null)
                return;
            if (TipWindowField?.Get(errorWindow) is not NativeWindow tipWindow)
                return;

            errorWindows.AddOrUpdate(control, InitHook, UpdateHook);

            #region Local Methods

            NativeWindow InitHook(Control _)
            {
                IntPtr hook = User32.HookCallWndRetProc(callWndRetProc);
                toolTipInfo[tipWindow.Handle] = new ToolTipInfo { Message = value, Hook = hook };
                return tipWindow;
            }

            NativeWindow UpdateHook(Control key, NativeWindow oldWindow)
            {
                if (oldWindow != tipWindow)
                {
                    RemoveHook(oldWindow);
                    return InitHook(key);
                }

                toolTipInfo[tipWindow.Handle].Message = value;
                return tipWindow;
            }

            void RemoveHook(NativeWindow nativeWindow)
            {
                if (toolTipInfo.TryRemove(nativeWindow.Handle, out var info))
                    User32.UnhookWindowsHook(info.Hook);
            }

            #endregion
        }

        #endregion

        #region Internal Methods

        internal void ResetAppearance()
        {
            bool customToolTip = OSUtils.IsWindows && !OSUtils.IsMono
                && ThemeColors.IsSet(ThemeColor.ToolTip) || ThemeColors.IsSet(ThemeColor.ToolTipText) || ThemeColors.IsSet(ThemeColor.ToolTipBorder);
            if (isCustomRendering == customToolTip)
                return;

            Debug.Assert(isCustomRendering || toolTipInfo.Count == 0 && errorWindows.Count == 0);
            isCustomRendering = customToolTip;
            toolTipFont = null;

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

            toolTipInfo.Clear();
            errorWindows.Clear();
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (var item in toolTipInfo)
                    User32.UnhookWindowsHook(item.Value.Hook);
                errorWindows.Clear();
                toolTipInfo.Clear();
            }
        }

        #endregion

        #region Private Methods

        private unsafe IntPtr ToolTipWindowHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var msg = (CWPRETSTRUCT*)lParam;

                // NOTE: we may receive the messages even for the associated parent control of the tooltip checking toolTipInfo
                // is not just for getting the tooltip text, but also to ensure that we paint only the tooltip window.
                if (msg->message == Constants.WM_PAINT && toolTipInfo.TryGetValue(msg->hwnd, out ToolTipInfo? info))
                {
                    using Graphics g = Graphics.FromHwnd(msg->hwnd);
                    new DrawToolTipEventArgs(g, null, null, User32.GetClientRect(msg->hwnd), info.Message,
                        default, default, GetFont(msg->hwnd)).DrawToolTipAdvanced();
                }
            }

            return User32.CallNextHook(nCode, wParam, lParam);
        }

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
                    toolTipFont = SystemFonts.MessageBoxFont;
                }
            }

            return toolTipFont!;
        }

        #endregion

        #endregion
    }
}
