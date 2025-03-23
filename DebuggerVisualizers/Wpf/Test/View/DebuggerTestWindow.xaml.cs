#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerTestWindow.cs
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
using System.Windows;
using System.Windows.Interop;

using KGySoft.Drawing.DebuggerVisualizers.Wpf.Test.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Wpf.Test.View
{
    internal partial class DebuggerTestWindow : Window
    {
        #region Properties

        private DebuggerTestViewModel ViewModel => (DebuggerTestViewModel)DataContext;

        #endregion

        #region Constructors

        public DebuggerTestWindow()
        {
            InitializeComponent();
            ViewModel.ErrorCallback = OnShowError;
            ViewModel.GetHwndCallback = () => new WindowInteropHelper(this).Handle;
            ViewModel.ViewLoaded();
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ViewModel.Dispose();
        }

        #endregion

        #region Private Methods

        private void OnShowError(string message) => MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        #endregion

        #endregion
    }
}