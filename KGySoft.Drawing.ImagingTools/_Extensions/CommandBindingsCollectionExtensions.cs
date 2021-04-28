#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CommandBindingsCollectionExtensions.cs
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

using System;
using System.Collections.Generic;
using System.ComponentModel;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Extensions for the <see cref="CommandBindingsCollection"/> class.
    /// </summary>
    internal static class CommandBindingsCollectionExtensions
    {
        #region Constants

        private const string stateHandler = nameof(stateHandler);
        private const string statePropertyNames = nameof(statePropertyNames);

        #endregion

        #region Fields

        private static readonly ICommand propertyChangedCommand = new SourceAwareCommand<PropertyChangedEventArgs>(OnPropertyChangedCommand);

        #endregion

        #region Methods

        #region Internal Methods

        internal static void AddTwoWayPropertyBinding(this CommandBindingsCollection collection, object source, string sourcePropertyName, object target,
            string? targetPropertyName = null, Func<object?, object?>? format = null, Func<object?, object?>? parse = null)
        {
            collection.AddPropertyBinding(source, sourcePropertyName, targetPropertyName ?? sourcePropertyName, format, target);
            collection.AddPropertyBinding(target, targetPropertyName ?? sourcePropertyName, sourcePropertyName, parse, source);
        }

        internal static ICommandBinding AddPropertyChangedHandler(this CommandBindingsCollection collection, Action handler, INotifyPropertyChanged source, params string[] propertyNames)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            if (propertyNames == null)
                throw new ArgumentNullException(nameof(propertyNames));

            var state = new Dictionary<string, object?>
            {
                [stateHandler] = handler,
                [statePropertyNames] = propertyNames
            };

            return collection.Add(propertyChangedCommand, state)
                .AddSource(source, nameof(source.PropertyChanged));
        }

        #endregion

        #region Private Methods

        private static void OnPropertyChangedCommand(ICommandSource<PropertyChangedEventArgs> source, ICommandState state)
        {
            if (!source.EventArgs.PropertyName.In(state.GetValueOrDefault<string[]>(statePropertyNames)))
                return;
            state.GetValueOrDefault<Action?>(stateHandler)?.Invoke();
        }

        #endregion

        #endregion
    }
}