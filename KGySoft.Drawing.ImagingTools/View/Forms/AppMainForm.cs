#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AppMainForm.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AppMainForm : ImageVisualizerForm
    {
        #region Fields

        private string? title;

        #endregion

        #region Properties

        #region Public Properties

        [AllowNull]
        public override string Text
        {
            // base has VM.TitleCaption -> Text binding so this solution makes possible to enrich it in a compatible way
            get => base.Text;
            set => base.Text = FormatText(value);
        }

        #endregion

        #region Private Properties

        private new DefaultViewModel ViewModel => (DefaultViewModel)base.ViewModel;

        #endregion

        #endregion

        #region Constructors

        #region Internal Constructors

        internal AppMainForm(DefaultViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }

        #endregion

        #region Private Constructors

        private AppMainForm() : this(null!)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyStringResources()
        {
            base.ApplyStringResources();
            title = Res.TitleAppNameAndVersion(typeof(Res).Assembly.GetName().Version!);
            if (CommandBindings.Count > 0)
                Text = ViewModel.TitleCaption;
        }

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            base.ApplyViewModel();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && ViewModel.IsModified)
            {
                e.Cancel = !ViewModel.ConfirmIfModified();
                if (e.Cancel)
                    DialogResult = DialogResult.None;
            }

            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void InitPropertyBindings()
        {
            // Base updates Text when ViewModel.TitleCaption changes.
            // Here adding an update also for FileName and IsModified changes in a compatible way
            CommandBindings.AddPropertyChangedHandler(() => Text = ViewModel.TitleCaption, ViewModel,
                nameof(ViewModel.FileName), nameof(ViewModel.IsModified));
        }

        private string FormatText(string? value)
        {
            string? fileName = ViewModel.FileName;
            string name = fileName == null ? Res.TextUnnamed : Path.GetFileName(fileName);
            return String.IsNullOrEmpty(value) ? title! : Res.TitleAppNameWithFileName(title!, name, ViewModel.IsModified ? "*" : String.Empty, value!);
        }

        #endregion

        #endregion
    }
}
