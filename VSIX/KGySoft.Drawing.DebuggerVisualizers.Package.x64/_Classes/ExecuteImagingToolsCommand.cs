#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ExecuteImagingToolsCommand.cs
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
using System.Threading;
using System.Threading.Tasks;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Extensibility;
using Microsoft.VisualStudio.Extensibility.Commands;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    [VisualStudioContribution]
    internal class ExecuteImagingToolsCommand : Command
    {
        #region Fields

        private static volatile IView? imagingToolsView;

        #endregion

        #region Properties

        public override CommandConfiguration CommandConfiguration => new("%Commands.ExecuteImagingToolsCommand.Text%")
        {
            Icon = new(ImageMoniker.Custom("ImagingTools"), IconSettings.IconAndText),
            Placements = [CommandPlacement.KnownPlacements.ExtensionsMenu]
        };

        #endregion

        #region Methods

        #region Static Methods

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

        #region Instance Methods

        #region Public Methods

        public override Task ExecuteCommandAsync(IClientContext context, CancellationToken cancellationToken)
        {
            ExecuteImagingTools();
            return Task.CompletedTask;
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            imagingToolsView?.Dispose();
            imagingToolsView = null;
            base.Dispose(disposing);
        }

        #endregion

        #endregion

        #endregion
    }
}
