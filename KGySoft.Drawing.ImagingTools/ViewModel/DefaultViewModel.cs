﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DefaultViewModel.cs
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

        #region Constructors

        internal DefaultViewModel() => SmoothZooming = true;

        #endregion

        #region Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            string[]? args = CommandLineArguments;
            if (args.IsNullOrEmpty() || !ProcessArgs(args!))
                UpdateInfo();
            base.ViewLoaded();
        }

        internal bool ConfirmIfModified() => !IsModified || Confirm(Res.ConfirmMessageDiscardChanges, false);

        #endregion

        #region Protected Methods

        protected override void OpenFile()
        {
            if (!ConfirmIfModified())
                return;
            base.OpenFile();
        }

        protected override bool OpenFile(string path)
        {
            if (!base.OpenFile(path))
                return false;
            FileName = Path.GetFileName(path);
            return true;
        }

        protected override bool SaveFile(string fileName, string selectedFormat)
        {
            bool success = base.SaveFile(fileName, selectedFormat);

            // was not saved or just a single frame was saved
            if (!success || ImageInfo.HasFrames && !IsCompoundView)
                return false;

            // not clearing the state if the compound image was not saved by its primary format
            if (ImageInfo.HasFrames)
            {
                switch (ImageInfo.Type)
                {
                    case ImageInfoType.Pages:
                        if (selectedFormat != "*.tiff")
                            return false;
                        break;
                    case ImageInfoType.MultiRes:
                    case ImageInfoType.Icon:
                        if (selectedFormat != "*.ico")
                            return false;
                        break;
                    case ImageInfoType.Animation:
                        if (selectedFormat != "*.gif")
                            return false;
                        break;
                }
            }

            FileName = fileName;
            SetModified(false);
            return true;
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

        private bool ProcessArgs(string[] args)
        {
            if (args.Length == 0)
                return false;
            string file = args[0];
            if (!File.Exists(file))
            {
                ShowError(Res.ErrorMessageFileDoesNotExist(file));
                return false;
            }

            return OpenFile(file);
        }

        #endregion

        #endregion
    }
}
