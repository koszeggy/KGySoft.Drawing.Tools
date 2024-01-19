#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: WinFormsCommandBindingsCollection.cs
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

using System.Collections.Generic;
using System.Windows.Forms;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    /// <summary>
    /// A specialized <see cref="CommandBindingsCollection"/> that can be used for commands with <see cref="Control"/> sources.
    /// By using this collection the <see cref="ICommandState"/> properties (eg. <see cref="ICommandState.Enabled"/> but also any other added property)
    /// of the added bindings will be synced with the command sources.
    /// </summary>
    internal class WinFormsCommandBindingsCollection : CommandBindingsCollection
    {
        #region Methods

        public override ICommandBinding Add(ICommand command, IDictionary<string, object?>? initialState = null, bool disposeCommand = false)
            => base.Add(command, initialState, disposeCommand)
                .AddStateUpdater(PropertyCommandStateUpdater.Updater);

        #endregion
    }
}