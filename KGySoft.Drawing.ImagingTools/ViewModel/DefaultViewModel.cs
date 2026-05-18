#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DefaultViewModel.cs
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

using System.IO;

using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DefaultViewModel : ImageVisualizerViewModel
    {
        #region Properties

        #region Internal Properties
        
        internal string[]? CommandLineArguments { get => Get<string[]?>(); init => Set(value); }
        internal string? FileName { get => Get<string?>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected override bool IsDebuggerVisualizer => false;

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            string[]? args = CommandLineArguments;
            UpdateInfo();
            ProcessArgs(args!);
            base.ViewLoaded();
        }

        internal bool ConfirmIfModified() => !IsModified || Confirm(Res.ConfirmMessageDiscardChanges, false);

        #endregion

        #region Protected Methods

        protected override bool OnFileOpening() => ConfirmIfModified();
        protected override void OnFileOpened(string path) => FileName = Path.GetFileName(path);

        protected override void OnFileSaved(string fileName, string selectedFormat)
        {
            // just a single frame was saved
            if (ImageInfo.HasFrames && !IsCompoundView)
                return;

            // not clearing the state if the compound image was not saved by its primary format
            if (ImageInfo.HasFrames
                && (ImageInfo.Type == ImageInfoType.Pages && selectedFormat != "*.tiff"
                    || ImageInfo.Type is ImageInfoType.MultiRes or ImageInfoType.Icon && selectedFormat != "*.ico"
                    || ImageInfo.Type == ImageInfoType.Animation && selectedFormat != "*.gif"))
            {
                return;
            }

            FileName = fileName;
            SetModified(false);
        }

        protected override void Clear()
        {
            if (!ConfirmIfModified())
                return;
            base.Clear();
            FileName = null;
        }

        #endregion

        #region Private Methods

        private void ProcessArgs(string[] args)
        {
            if (args.Length == 0)
                return;
            
            string file = args[0];
            if (!File.Exists(file))
                ShowError(Res.ErrorMessageFileDoesNotExist(file));
            else
                OpenFile(file);
        }

        #endregion

        #endregion
    }
}
