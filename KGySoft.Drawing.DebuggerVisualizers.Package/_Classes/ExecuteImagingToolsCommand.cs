#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ExecuteImagingToolsCommand.cs
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
using System.ComponentModel.Design;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Shell;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    internal static class ExecuteImagingToolsCommand
    {
        #region Fields

        private static MenuCommand? commandInstance;
        private static IServiceProvider? serviceProvider;
        private static IViewModel? imagingToolsViewModel;
        private static IView? imagingToolsView;

        #endregion

        #region Methods

        #region Internal Methods

        internal static MenuCommand GetCreateCommand(IServiceProvider package)
        {
            if (commandInstance == null)
            {
                serviceProvider = package;
                commandInstance = new OleMenuCommand(OnExecuteImagingToolsCommand, new CommandID(Ids.CommandSet, Ids.ExecuteImagingToolsCommandId));
            }

            return commandInstance;
        }

        internal static void DestroyCommand()
        {
            imagingToolsView?.Dispose();
            imagingToolsView = null;
            imagingToolsViewModel?.Dispose();
            imagingToolsViewModel = null;
        }

        #endregion

        #region Event handlers

        private static void OnExecuteImagingToolsCommand(object sender, EventArgs e)
        {
            try
            {
                if (imagingToolsView == null || imagingToolsView.IsDisposed)
                {
                    imagingToolsViewModel?.Dispose();
                    imagingToolsViewModel = ViewModelFactory.CreateDefault();
                    imagingToolsView = ViewFactory.CreateView(imagingToolsViewModel);
                }

                imagingToolsView.Show();
            }
            catch (Exception ex)
            {
                imagingToolsView?.Dispose();
                imagingToolsViewModel?.Dispose();
                imagingToolsView = null;
                ShellDialogs.Error(serviceProvider!, Res.ErrorMessageUnexpectedError(ex.Message));
            }
        }

        #endregion

        #endregion
    }
}
