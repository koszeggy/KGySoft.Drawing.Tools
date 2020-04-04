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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Forms
{
    internal partial class AppMainForm : ImageVisualizerForm
    {
        #region Fields

        private static readonly string title = Res.TitleAppName;

        #endregion

        #region Properties

        #region Public Properties
        
        public override string Text
        {
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

        private AppMainForm() : this(null)
        {
            // this ctor is just for the designer
        }

        #endregion

        #endregion

        #region Methods

        #region Protected Methods

        protected override void ApplyViewModel()
        {
            InitPropertyBindings();
            base.ApplyViewModel();
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
            // VM.FileName -> Text = VM.TitleCaption (text will be formatted to contain file name, if any)
            CommandBindings.AddPropertyBinding(ViewModel, nameof(ViewModel.FileName), nameof(Text), _ => ViewModel.TitleCaption, this);
        }

        private string FormatText(string value)
        {
            var fileName = ViewModel.FileName;
            return String.IsNullOrEmpty(value) ? title : $"{title}{(fileName == null ? null : $" [{Path.GetFileName(fileName)}]")} - {value}";
        }

        #endregion

        #endregion
    }
}
