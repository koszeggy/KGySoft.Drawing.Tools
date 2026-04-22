#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ControlExtensions.cs
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

using KGySoft.WinForms;

#region Used Namespaces

using System;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View.Controls;
using KGySoft.Drawing.ImagingTools.WinApi;
using KGySoft.Reflection;
using KGySoft.WinForms.Controls;

#endregion

#region Used Aliases

using AdvancedTextBox = KGySoft.WinForms.Controls.AdvancedTextBox;

#endregion

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
                    if (ThemeColors.IsBaseThemeEverChanged && OSHelper.IsWindows10OrLater)
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

                case AdvancedTextBox textBox:
                    textBox.ApplyVisualStyleTheme();
                    textBox.DisabledForeColor = ThemeColors.WindowTextDisabled;
                    textBox.DisabledBackColor = ThemeColors.Control;
                    textBox.EnabledForeColor = textBox.ReadOnly ? ThemeColors.ControlText : ThemeColors.WindowText;
                    textBox.EnabledBackColor = ThemeColors.Window;
                    break;

                case AdvancedButton button:
                    button.DisabledForeColor = ThemeColors.ControlTextDisabled;
                    button.ApplyVisualStyleTheme();
                    break;

                case AdvancedCheckBox checkBox:
                    checkBox.DisabledForeColor = ThemeColors.ControlTextDisabled;
                    checkBox.ApplyVisualStyleTheme();
                    break;

                case AdvancedRadioButton radioButton:
                    radioButton.DisabledForeColor = ThemeColors.ControlTextDisabled;
                    radioButton.ApplyVisualStyleTheme();
                    break;

                case AdvancedLabel label:
                    label.DisabledForeColor = ThemeColors.ControlTextDisabled;
                    break;

                case AdvancedComboBox comboBox:
                    comboBox.EnabledBackColor = ThemeColors.Window;
                    comboBox.EnabledForeColor = ThemeColors.WindowText;
                    comboBox.DisabledBackColor = ThemeColors.Control;
                    comboBox.DisabledForeColor = ThemeColors.WindowTextDisabled;
                    comboBox.ApplyVisualStyleTheme();
                    break;

                case GroupBox groupBox:
                    groupBox.ForeColor = ThemeColors.GroupBoxText;
                    break;

                case AdvancedDataGridView dataGridView:
                    dataGridView.ApplyTheme();
                    dataGridView.ApplyVisualStyleTheme();
                    break;

                case ScrollBar scrollBar:
                    scrollBar.ApplyVisualStyleTheme();
                    break;

                case AdvancedProgressBar progressBar:
                    // Makes a difference only when Style is not System
                    progressBar.BackColor = ThemeColors.ProgressBarBackground;
                    progressBar.ForeColor = ThemeColors.ProgressBar;
                    progressBar.Style = ThemeColors.IsSet(ThemeColor.ProgressBar) || ThemeColors.IsSet(ThemeColor.ProgressBarBackground) ? AdvancedProgressBarStyle.ThemedShiny : AdvancedProgressBarStyle.System;
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

        internal static IntPtr GetHandleIfCreated(this Control c) => c.IsHandleCreated ? c.Handle : IntPtr.Zero;

        /// <summary>
        /// This method is to prevent accidentally disappearing controls after a faulty layout change.
        /// May occur on some platforms after changing DPI or RTL.
        /// </summary>
        internal static void EnsureSize(this Control c)
        {
            if (c.Dock is DockStyle.Left or DockStyle.Right)
            {
                int width = c.Width;
                c.Width = 0;
                c.Width = width;
                return;
            }

            int height = c.Height;
            c.Height = 0;
            c.Height = height;
        }

        #endregion

        #region Private Methods

        private static void ApplyVisualStyleTheme(this Control control)
        {
            if (!OSHelper.IsWindows10OrLater) // TODO: || !VisualStyleHelper.InitializedWithVisualStyles
                return;

            const string darkTheme = "DarkMode_Explorer";
            const string lightTheme = "Explorer";
            const string textBoxTheme = "CFD";

            control.HandleCreated -= Control_HandleCreated;
            control.HandleCreated += Control_HandleCreated;
            control.Disposed -= Control_Disposed;
            control.Disposed += Control_Disposed;
            if (!control.IsHandleCreated || !ThemeColors.RenderWithVisualStyles)
                return;

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

                case ButtonBase or ScrollBar or AdvancedDataGridView: // AdvancedDataGridView: for the checkbox columns
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

        #region Event Handlers

        private static void Control_HandleCreated(object? sender, EventArgs e) => ((Control)sender!).ApplyVisualStyleTheme();

        private static void Control_Disposed(object? sender, EventArgs e)
        {
            Control control = (Control)sender!;
            control.HandleCreated -= Control_HandleCreated;
            control.Disposed -= Control_Disposed;
        }

        #endregion

        #endregion
    }
}
