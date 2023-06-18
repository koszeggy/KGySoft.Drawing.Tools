#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: BindingBaseExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Windows;
using System.Windows.Data;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Test
{
    internal static class BindingBaseExtensions
    {
        #region Fields

        private static readonly DependencyProperty dummyProperty = DependencyProperty.RegisterAttached("dummy", typeof(object), typeof(DependencyObject));

        #endregion

        #region Methods

        public static object? Evaluate(this BindingBase? binding, DependencyObject? target)
        {
            if (binding == null)
                return null;
            var obj = target ?? new DependencyObject();
            BindingOperations.SetBinding(obj, dummyProperty, binding);
            object result = obj.GetValue(dummyProperty);
            BindingOperations.ClearBinding(obj, dummyProperty);
            return result;
        }

        #endregion
    }
}