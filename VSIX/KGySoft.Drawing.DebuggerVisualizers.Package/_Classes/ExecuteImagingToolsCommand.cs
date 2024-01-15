#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ExecuteImagingToolsCommand.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel.Design;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Shell;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ExecuteImagingToolsCommand
    {
        #region Fields

        private static MenuCommand? commandInstance;
        private static volatile IView? imagingToolsView;

        #endregion

        #region Methods

        #region Internal Methods

        internal static MenuCommand GetCreateCommand()
            => commandInstance ??= new OleMenuCommand(OnExecuteImagingToolsCommand, new CommandID(Ids.CommandSet, Ids.ExecuteImagingToolsCommandId));

        internal static void DestroyCommand()
        {
            imagingToolsView?.Dispose();
            imagingToolsView = null;
        }

        internal static void ExecuteImagingTools()
        {
            try
            {
                if (imagingToolsView == null || imagingToolsView.IsDisposed)
                    imagingToolsView = ViewHelper.CreateViewInNewThread(ViewModelFactory.CreateDefault);
                else
                    imagingToolsView.Show();
            }
            catch (Exception ex)
            {
                imagingToolsView?.Dispose();
                imagingToolsView = null;
                Notifications.Error(Res.ErrorMessageUnexpectedError(ex.Message));
            }
        }

        #endregion

        #region Event handlers

        private static void OnExecuteImagingToolsCommand(object sender, EventArgs e) => ExecuteImagingTools();

        #endregion

        #endregion
    }
}
