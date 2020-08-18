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

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    /// <summary>
    /// Extensions for the <see cref="CommandBindingsCollection"/> class.
    /// </summary>
    internal static class CommandBindingsCollectionExtensions
    {
        #region Methods

        public static void AddTwoWayPropertyBinding(this CommandBindingsCollection collection, object source, string sourcePropertyName, object target,
            string targetPropertyName = null, Func<object, object> format = null, Func<object, object> parse = null)
        {
            collection.AddPropertyBinding(source, sourcePropertyName, targetPropertyName ?? sourcePropertyName, format, target);
            collection.AddPropertyBinding(target, targetPropertyName ?? sourcePropertyName, sourcePropertyName, parse, source);
        }

        #endregion
    }
}