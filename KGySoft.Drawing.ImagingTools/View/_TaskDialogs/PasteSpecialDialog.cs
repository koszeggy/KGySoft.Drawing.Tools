#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PasteSpecialDialog.cs
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
using System.Linq;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.WinForms.Components;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    internal class PasteSpecialDialog : MvvmTaskDialogBase
    {
        #region Properties

        #region Protected Properties

        protected override TaskDialogStandardButtons Buttons => TaskDialogStandardButtons.OK | TaskDialogStandardButtons.Cancel;

        #endregion

        #region Private Properties

        private new PasteSpecialViewModel ViewModel => (PasteSpecialViewModel)base.ViewModel;

        #endregion
        
        #endregion

        #region Constructors

        internal PasteSpecialDialog(PasteSpecialViewModel viewModel) : base(viewModel)
        {
            ResetRadioButtons();
            TaskDialog.CustomIcon = Images.PasteSpecialIcon;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyViewModel()
        {
            InitCommandBindings();
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void ApplyStringResources()
        {
            base.ApplyStringResources();
            TaskDialog.Caption = Res.TitlePasteSpecialDialog;
            TaskDialog.Message = Res.TextPasteSpecialMessage;
            TaskDialog.CheckBoxText = Res.TextPasteSpecialCheckBox;
        }

        #endregion

        #region Private Methods

        private void InitCommandBindings()
        {
            // ViewModel.PropertyChanged(Formats) -> OnFormatsChangedCommand
            CommandBindings.AddPropertyChangedHandlerBinding(ViewModel, OnFormatsChangedCommand, nameof(ViewModel.Formats));

            // btnOK.Click -> ViewModel.AcceptWithCloseCommand - NOTE: needed only to adjust the Enabled state. The dialog would automatically close without handling the event.
            CommandBindings.Add(ViewModel.AcceptWithCloseCommand, ViewModel.AcceptWithCloseCommandState)
                .AddSource(GetButton(TaskDialogStandardButton.OK)!, nameof(TaskDialogButton.Click));
        }

        private void InitPropertyBindings()
        {
            //ViewModel.CustomAlphaDetection <-> TaskDialog.CheckBoxChecked
            CommandBindings.AddTwoWayPropertyBinding(ViewModel, nameof(ViewModel.CustomAlphaDetection), TaskDialog, nameof(TaskDialog.CheckBoxChecked));
        }

        private void ResetRadioButtons()
        {
            string[] formats = ViewModel.Formats;
            if (formats.SequenceEqual(TaskDialog.RadioButtons.Select(rb => rb.Text)))
                return;

            // this removes the previous subscriptions
            foreach (TaskDialogRadioButton radioButton in TaskDialog.RadioButtons)
                radioButton.Dispose();

            TaskDialog.RadioButtons.ReplaceRange(0, TaskDialog.RadioButtons.Count, formats.Select(f => new TaskDialogRadioButton(f)));
            foreach (TaskDialogRadioButton radioButton in TaskDialog.RadioButtons)
                radioButton.Selected += (sender, _) => ViewModel.SelectedFormat = ((TaskDialogRadioButton)sender!).Text;
            int selectedIndex = Array.IndexOf(formats, ViewModel.SelectedFormat);
            if (selectedIndex >= 0)
                TaskDialog.RadioButtons[selectedIndex].Checked = true;
            else
                ViewModel.SelectedFormat = null;

            if (formats.Length > 0 && TaskDialog.Handle != IntPtr.Zero && ThemeColors.IsDarkBaseTheme)
                ApplyTheme();
        }

        #endregion

        #region Command Handlers

        private void OnFormatsChangedCommand() => ResetRadioButtons();

        #endregion

        #endregion
    }
}
