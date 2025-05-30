﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ControlExtensions.cs
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

using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Drawing.ImagingTools.View.UserControls;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;

#endregion

#region Suppressions

#if NETCOREAPP3_0
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type. - Controls/Columns/DropDownItems never have null elements
#pragma warning disable CS8602 // Dereference of a possibly null reference. - Controls/Columns/DropDownItems never have null elements
#pragma warning disable CS8604 // Possible null reference argument. - Controls/Columns/DropDownItems never have null elements
#endif

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal static class ControlExtensions
    {
        #region Constants

        internal const string ToolTipPropertyName = "ToolTipText";

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>
        /// Sets the double buffering state of a control
        /// </summary>
        /// <param name="control">The control to set.</param>
        /// <param name="useDoubleBuffering"><see langword="true"/>, if <paramref name="control"/> should use double buffering; otherwise, <see langword="false"/>.</param>
        internal static void SetDoubleBuffered(this Control control, bool useDoubleBuffering)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            Reflector.SetProperty(control, "DoubleBuffered", useDoubleBuffering);
        }

        internal static PointF GetScale(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            return OSUtils.GetScale(control.Handle);
        }

        internal static Size ScaleSize(this Control control, Size size) => size.Scale(control.GetScale());

        internal static int ScaleWidth(this Control control, int width) => width.Scale(control.GetScale().X);
        internal static int ScaleHeight(this Control control, int height) => height.Scale(control.GetScale().Y);

        /// <summary>
        /// Applies fixed string resources (which do not change unless language is changed) to a control.
        /// </summary>
        internal static void ApplyStringResources(this Control control, ToolTip? toolTip = null)
        {
            #region Local Methods

            static void ApplyToolTip(Control control, string name, ToolTip toolTip)
            {
                string? value = Res.GetStringOrNull(name + "." + ToolTipPropertyName);
                toolTip.SetToolTip(control, value);
            }

            static void ApplyToolStripResources(ToolStripItemCollection items)
            {
                foreach (ToolStripItem item in items)
                {
                    // to self
                    Res.ApplyStringResources(item, item.Name);

                    // to children
                    if (item is ToolStripDropDownItem dropDownItem)
                        ApplyToolStripResources(dropDownItem.DropDownItems);
                }
            }

            #endregion

            string name = control.Name;
            if (String.IsNullOrEmpty(name))
                name = control.GetType().Name;

            // custom localization
            if (control is ICustomLocalizable customLocalizable)
            {
                customLocalizable.ApplyStringResources(toolTip);
                return;
            }

            // to self
            Res.ApplyStringResources(control, name);

            // applying tool tip
            if (toolTip != null)
                ApplyToolTip(control, name, toolTip);

            // to children
            switch (control)
            {
                case ToolStrip toolStrip:
                    ApplyToolStripResources(toolStrip.Items);
                    break;

                case DataGridView dataGridView:
                    foreach (DataGridViewColumn item in dataGridView.Columns)
                        Res.ApplyStringResources(item, item.Name);
                    break;

                default:
                    foreach (Control child in control.Controls)
                    {
                        // MvvmBaseUserControl triggers ApplyStringResources on its own, so skipping it as a child control here
                        if (child is MvvmBaseUserControl)
                            continue;

                        child.ApplyStringResources(toolTip);
                    }

                    break;
            }
        }

        internal static void ApplyTheme(this Control control)
        {
#if NET9_0_OR_GREATER && SYSTEM_THEMING
            return;
#endif

            // special handling for controls by type
            switch (control)
            {
                case Form form:
                    // skipping everything if the theme has never changed
                    if (!ThemeColors.IsThemeEverChanged)
                        return;

                    // setting the caption theme
                    if (ThemeColors.IsBaseThemeEverChanged && OSUtils.IsWindows10OrLater)
                    {
                        try
                        {
                            User32.SetCaptionTheme(form.Handle, ThemeColors.IsDarkBaseTheme);
                        }
                        catch (Exception e) when (!e.IsCritical())
                        {
                        }
                    }

                    // setting the form's background and foreground color
                    form.BackColor = ThemeColors.Control;
                    form.ForeColor = ThemeColors.ControlText;
                    break;

                case TextBoxBase textBox:
                    textBox.ApplyVisualStyleTheme();
                    if (!textBox.Enabled)
                    {
                        textBox.BackColor = ThemeColors.Control;
                        textBox.ForeColor = ThemeColors.ControlTextDisabled;
                    }
                    else if (textBox.ReadOnly)
                    {
                        textBox.BackColor = ThemeColors.Control;
                        textBox.ForeColor = ThemeColors.ControlText;
                    }
                    else
                    {
                        textBox.BackColor = ThemeColors.Window;
                        textBox.ForeColor = ThemeColors.WindowText;
                    }
                    break;

                case Button button:
                    // TODO: Set FlatStyle to Flat if custom colors are set; otherwise, set it to System and call ApplyVisualStyleTheme
                    button.ApplyVisualStyleTheme();
                    break;

                case ButtonBase buttonBase and (CheckBox or RadioButton):
                    // ISSUE: The text of FlatStyle.System appearance is always black with visual styles, even in dark mode. TODO: Use KGySoft.WinForms.Controls.AdvancedCheckBox/RadioButton
                    //buttonBase.FlatStyle = ThemeColors.IsDarkBaseTheme ? FlatStyle.Standard : FlatStyle.System;
                    buttonBase.ApplyVisualStyleTheme();
                    break;

                case ComboBox comboBox:
                    comboBox.BackColor = ThemeColors.Window;
                    comboBox.ForeColor = ThemeColors.WindowText;
                    comboBox.ApplyVisualStyleTheme();
                    break;

                case GroupBox groupBox:
                    groupBox.ForeColor = ThemeColors.GroupBoxText;
                    break;

                case AdvancedDataGridView dataGridView:
                    dataGridView.ApplyTheme();
                    break;

                case ScrollBar scrollBar:
                    scrollBar.ApplyVisualStyleTheme();
                    break;

#if !SYSTEM_THEMING
                case AdvancedToolStrip toolStrip:
                    toolStrip.ApplyTheme();
                    break;
#endif
            }

            foreach (Control child in control.Controls)
                child.ApplyTheme();
        }

        #endregion
        
        #region Private Methods
        
        private static void ApplyVisualStyleTheme(this Control control)
        {
            if (!OSUtils.IsWindows10OrLater || !Application.RenderWithVisualStyles)
                return;

            const string darkTheme = "DarkMode_Explorer";
            const string lightTheme = "Explorer";
            const string textBoxTheme = "CFD";

            switch (control)
            {
                case TextBoxBase { Multiline: false }:
                    IntPtr handle = control.Handle;
                    UxTheme.SetWindowTheme(handle, textBoxTheme, null);
                    UxTheme.SetWindowDarkMode(handle, ThemeColors.IsDarkBaseTheme);
                    User32.SendMessage(handle, Constants.WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);
                    break;

                case TextBoxBase { Multiline: true }:
                    UxTheme.SetWindowTheme(control.Handle, ThemeColors.IsDarkBaseTheme ? darkTheme : lightTheme, null);
                    break;

                case ButtonBase or ScrollBar:
                    UxTheme.SetWindowTheme(control.Handle, ThemeColors.IsDarkBaseTheme ? darkTheme : null, null);
                    break;

                case ComboBox:
                    handle = control.Handle;
                    UxTheme.SetWindowTheme(handle, textBoxTheme, null);
                    UxTheme.SetWindowDarkMode(handle, ThemeColors.IsDarkBaseTheme);
                    User32.SendMessage(handle, Constants.WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);

                    // The scrollbar of the drop-down area
                    unsafe
                    {
                        COMBOBOXINFO cInfo = default;
                        cInfo.cbSize = (uint)sizeof(COMBOBOXINFO);
                        if (User32.GetComboBoxInfo(handle, ref cInfo))
                            UxTheme.SetWindowTheme(cInfo.hwndList, ThemeColors.IsDarkBaseTheme ? darkTheme : null, null); 
                    }
                    break;
            }
        }

        #endregion

        #endregion
    }
}
