#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DefaultViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.IO;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DefaultViewModel : ImageVisualizerViewModel
    {
        #region Properties

        internal string[] CommandLineArguments { get => Get<string[]>(); set => Set(value); }
        internal string FileName { get => Get<string>(); set => Set(value); }

        #endregion

        #region Methods

        #region Internal Methods

        internal override void ViewLoaded()
        {
            ProcessArgs(CommandLineArguments);
            base.ViewLoaded();
        }

        #endregion

        #region Protected Methods

        protected override bool OpenFile(string path)
        {
            if (!base.OpenFile(path))
                return false;

            FileName = Path.GetFileName(path);
            return true;
        }

        protected override void Clear()
        {
            base.Clear();
            FileName = null;
        }

        #endregion

        #region Private Methods

        private void ProcessArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Notification = Res.NotificationWelcome;
                Image = null;
            }
            else
            {
                string file = args[0];
                if (!File.Exists(file))
                    ShowError(Res.ErrorMessageFileDoesNotExist(file));
                else
                    OpenFile(file);
            }
        }

        #endregion

        #endregion
    }
}
