﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: MvvmParentForm.cs
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
using System.Threading;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View.UserControls;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal class MvvmParentForm : BaseForm
    {
        #region Fields

        private readonly ManualResetEventSlim handleCreated;
        private readonly MvvmBaseUserControl mvvmChild;

        private bool isLoaded;
        private Point location;
        private Func<MvvmParentForm, Keys, bool>? processKeyCallback;

        #endregion

        #region Properties

        #region Internal Properties
        
        internal bool IsRtlChanging { get; private set; }

        #endregion

        #region Protected Properties

        protected MvvmBaseUserControl MvvmChild => mvvmChild;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal MvvmParentForm(MvvmBaseUserControl mvvmChild)
        {
            this.mvvmChild = mvvmChild;
            handleCreated = new ManualResetEventSlim();
            ApplyRightToLeft();
            InitializeForm();

            // In .NET Framework the DefaultFont is always MS Sans Serif, 8.25pt, instead of the actual system font for controls.
            // Note that we should set the Font for the Form only, because setting it also for the user controls would cause double scaling.
            // Not setting it for the user controls is alright even when docking them into a WPF host, because its default font is correct.
            // Also note that setting it in BaseForm would sometimes cause wrong scaling when opening a view from Visual Studio extension.
            if (!IsDesignMode && !OSUtils.IsMono && SystemFonts.MessageBoxFont is Font font)
                base.Font = font;

            StartPosition = OSUtils.IsMono && OSUtils.IsWindows ? FormStartPosition.WindowsDefaultLocation : FormStartPosition.CenterParent;
        }

        #endregion

        #region Private Constructors

        private MvvmParentForm()
            : this((MvvmBaseUserControl)ViewFactory.CreateView(ViewModelFactory.CreateDefault()))
        {
            // this ctor is just for the designer when opening the derived main form
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnLoad(EventArgs e)
        {
            // Not Using tool window appearance on Linux because looks bad an on high DPI the close is too small
            if (OSUtils.IsMono && OSUtils.IsLinux && FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MinimizeBox = false;
            }

            base.OnLoad(e);

            // Loaded can be true if handle was recreated
            if (isLoaded || IsDesignMode)
            {
                if (!IsRtlChanging)
                    return;

                // dialog has been reopened after changing RTL
                IsRtlChanging = false;
                Location = location;
                return;
            }

            isLoaded = true;
            ApplyStringResources();
            ApplyBindings();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            handleCreated.Set();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Changing RightToLeft causes the dialog close. We let it happen because the parent may also change,
            // and if we cancel the closing here, then a dialog may turn a non-modal form. Reopen as a dialog is handled in IView.ShowDialog
            if (IsRtlChanging)
            {
                if (DialogResult != DialogResult.Retry)
                    IsRtlChanging = false;
                else
                    location = Location;
            }

            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (processKeyCallback?.Invoke(this, keyData) == true)
                return true;
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected virtual void ApplyStringResources() => this.ApplyStringResources(null);

        protected virtual void ApplyBindings()
        {
            InitPropertyBindings();
            InitCommandBindings();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                handleCreated.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitializeForm()
        {
            SuspendLayout();
            Size clientSize = mvvmChild.Size;
            mvvmChild.Dock = DockStyle.Fill;
            Controls.Add(mvvmChild);
            ParentViewProperties properties = mvvmChild.ParentViewProperties ?? throw new InvalidOperationException(Res.InternalError($"{mvvmChild.Name} should override ParentViewProperties"));
            string childName = mvvmChild.Name;
            Name = childName.EndsWith("Control", StringComparison.Ordinal) ? $"{childName.Substring(0, childName.Length - 7)}Form" : childName;
            FormBorderStyle = properties.BorderStyle;
            Icon = properties.Icon;
            AcceptButton = properties.AcceptButton;
            CancelButton = properties.CancelButton;
            processKeyCallback = properties.ProcessKeyCallback;
            if (properties.HideMinimizeButton)
                MinimizeBox = false;
            if (properties.BorderStyle is FormBorderStyle.FixedDialog)
                MinimizeBox = MaximizeBox = false;
            if (!properties.MinimumSize.IsEmpty)
                MinimumSize = properties.MinimumSize;
            if (properties.ClosingCallback is FormClosingEventHandler handler)
                FormClosing += handler; // removed in base.Dispose
            ClientSize = clientSize;
            AutoScaleMode = AutoScaleMode.Font;
            RightToLeftLayout = true;
            ResumeLayout();

            // removed in BaseUserControl.Dispose
            mvvmChild.ViewModelChanged += (_, _) => ApplyBindings();
        }

        private void InitPropertyBindings()
        {
            mvvmChild.ParentViewPropertyBindingsInitializer?.Invoke(this);
        }

        private void InitCommandBindings()
        {
            mvvmChild.CommandBindings.Add(OnDisplayLanguageChangedCommand)
                .AddSource(typeof(Res), nameof(Res.DisplayLanguageChanged));

            mvvmChild.ParentViewCommandBindingsInitializer?.Invoke(this);
        }

        private void ApplyRightToLeft()
        {
            RightToLeft rtl = Res.DisplayLanguage.TextInfo.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            if (RightToLeft == rtl)
                return;

            if (!OSUtils.IsMono && IsHandleCreated)
                IsRtlChanging = true;

            RightToLeft = rtl;

            // Modal forms on Windows: when changing RTL, the DialogResult is set to Cancel in older framework targets, causing the dialog to close.
            // To make it work the same way on all platforms, we set it to Retry, signaling the check in OnClosing that the dialog should be reopened.
            // Without the reopening, the dialog would turn into a non-modal form, allowing the user to interact with the caller form.
            if (Modal && !OSUtils.IsMono && OSUtils.IsWindows)
                DialogResult = DialogResult.Retry;
        }

        #endregion

        #region Command Handlers

        private void OnDisplayLanguageChangedCommand() => mvvmChild.InvokeIfRequired(() =>
        {
            ApplyRightToLeft();
            ApplyStringResources();
        });

        #endregion

        #endregion
    }
}
