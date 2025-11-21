#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: VisualizerExtensionWpfAdapterBase.cs
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
using System.Windows.Media;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.View
{
    /// <summary>
    /// Provides a non-generic base class for a WPF adapter that provides a <see cref="UIElement"/> host for a debugger visualizer that can be
    /// created from an <see cref="IViewModel"/> implementation by the <see cref="ViewFactory"/> class.
    /// </summary>
    public partial class VisualizerExtensionWpfAdapterBase : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets whether this <see cref="VisualizerExtensionWpfAdapterBase"/> is disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerExtensionWpfAdapterBase"/> class.
        /// </summary>
        public VisualizerExtensionWpfAdapterBase()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Sets a notification error message to be displayed above the embedded visualizer.
        /// </summary>
        /// <param name="text">The notification text to display. Set <see langword="null"/> to hide the notification.</param>
        /// <param name="severity">The severity of the message.</param>
        public void SetNotification(string? text, ValidationSeverity severity = ValidationSeverity.Error)
        {
            InvokeIfRequired(() =>
            {
                border.BorderBrush = severity switch
                {
                    ValidationSeverity.Error => Brushes.Red,
                    ValidationSeverity.Warning => Brushes.Orange,
                    _ => default
                };
                txtNotification.Background = severity switch
                {
                    ValidationSeverity.Error => Brushes.LightPink,
                    ValidationSeverity.Warning => Brushes.Khaki,
                    _ => default
                };
                txtNotification.Foreground = severity switch
                {
                    ValidationSeverity.Error => Brushes.Maroon,
                    ValidationSeverity.Warning => Brushes.DarkGoldenrod,
                    _ => default
                };
                txtNotification.Text = text;
                txtNotification.Visibility = String.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        /// <summary>
        /// Disposes this <see cref="VisualizerExtensionWpfAdapterBase"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Disposes this <see cref="VisualizerExtensionWpfAdapterBase"/> instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/>, if called by the <see cref="Dispose()"/> method; otherwise, <see langword="false"/>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            if (disposing)
                host.Dispose();
        }

        #endregion

        #region Private Methods

        private void InvokeIfRequired(Action action)
        {
            if (Dispatcher.CheckAccess())
                action.Invoke();
            else
                Dispatcher.Invoke(action);
        }

        #endregion

        #endregion
    }
}
#endif