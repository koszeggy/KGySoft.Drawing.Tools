#if NET472_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: VisualizerExtensionWpfAdapter.cs
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
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.View
{
    /// <summary>
    /// Represents a WPF adapter that provides a <see cref="UIElement"/> host for a debugger visualizer that can be
    /// created from an <see cref="IViewModel{TModel}"/> implementation by the <see cref="ViewFactory"/> class.
    /// </summary>
    /// <typeparam name="TModel">The model type of the debugged object.</typeparam>
    public class VisualizerExtensionWpfAdapter<TModel> : VisualizerExtensionWpfAdapterBase
    {
        #region Fields

        private readonly VisualizerTarget visualizerTarget;

        private Func<TModel, VisualizerTarget, IViewModel<TModel>> viewModelFactory;
        private Func<Stream, TModel> deserialize;
        private Action<TModel, Stream>? serialize;
        private IViewModel<TModel>? viewModel;
        private TModel? lastAppliedModel;
        private bool suppressNextAvailable;
        private bool isMessagingAvailable;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerExtensionWpfAdapter{TModel}"/> class.
        /// </summary>
        /// <param name="visualizerTarget">The visualizer target passed to the <see cref="IDebuggerVisualizerProvider.CreateVisualizerAsync"/> method.</param>
        /// <param name="viewModelFactory">A delegate that can create a view model from a <typeparamref name="TModel"/> instance.</param>
        /// <param name="deserialize">A delegate that can deserialize a <typeparamref name="TModel"/> instance from a stream.</param>
        /// <param name="serialize">An optional delegate that can serialize a <typeparamref name="TModel"/> instance to a stream.</param>
        public VisualizerExtensionWpfAdapter(VisualizerTarget visualizerTarget, Func<TModel, VisualizerTarget, IViewModel<TModel>> viewModelFactory,
            Func<Stream, TModel> deserialize, Action<TModel, Stream>? serialize)
        {
            this.visualizerTarget = visualizerTarget;
            this.viewModelFactory = viewModelFactory;
            this.deserialize = deserialize;
            this.serialize = serialize;
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Initializes the adapter asynchronously.
        /// </summary>
        /// <param name="isToolWindowHost"><see langword="true"/>, if the visualizer is hosted in a tool window; otherwise, <see langword="false"/>.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InitializeAsync(bool isToolWindowHost)
        {
            // Attempting to initialize the view model even without an explicit Available notification.
            // This should always work for a ModalDialog visualizer, but usually works also for ToolWindows.
            // Early initialization allows creating the embedded view before the WPF window is shown.
            await VisualizerTargetOnStateChangedAsync(this, VisualizerTargetStateNotification.Available);
            if (isToolWindowHost)
            {
                // If the initialization was successful, then we can suppress the first Available notification.
                if (viewModel != null)
                    suppressNextAvailable = true;
                visualizerTarget.StateChanged += VisualizerTargetOnStateChangedAsync;
            }
            else
                Unloaded += WpfVisualizerAdapter_Unloaded;

        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            serialize = null;
            deserialize = null!;
            viewModelFactory = null!;
            visualizerTarget.StateChanged -= VisualizerTargetOnStateChangedAsync;
            if (viewModel is IViewModel<TModel> vm)
                vm.ChangesApplied -= ViewModel_ChangesApplied;

            // this disposes the view
            base.Dispose(disposing);

            // disposing the view model only after the view is disposed
            if (disposing)
            {
                viewModel?.Dispose();
                viewModel = null;
                (lastAppliedModel as IDisposable)?.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private async Task VisualizerTargetOnStateChangedAsync(object? sender, VisualizerTargetStateNotification args)
        {
            IViewModel<TModel>? vm = viewModel;
            ReadOnlySequence<byte> data;
            TModel model;

            switch (args)
            {
                case VisualizerTargetStateNotification.Available:
                    SetNotification(null);
                    isMessagingAvailable = !ReferenceEquals(sender, this);

                    // ViewModel is already initialized
                    if (vm != null)
                    {
                        // The first actual Available status after a successful initialization: we are already done.
                        if (suppressNextAvailable)
                        {
                            suppressNextAvailable = false;
                            return;
                        }

                        // Non-first available status: just updating the state
                        goto case VisualizerTargetStateNotification.ValueUpdated;
                    }

                    IView? view = null;
                    try
                    {
                        data = await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), CancellationToken.None) ?? default;
                        using (Stream stream = data.AsStream())
                            model = deserialize.Invoke(stream);

                        viewModel = viewModelFactory.Invoke(model, visualizerTarget);
                        view = ViewFactory.CreateView(viewModel);
                        host.Child = (Control)view;
                    }
                    catch (Exception e)
                    {
                        view?.Dispose();
                        viewModel?.Dispose();
                        viewModel = null;

                        // Failed to get the model data. Waiting for the next Available notification.
                        SetNotification(Res.ErrorMessageDebuggerVisualizerCannotLoad(e.Message));
                        return;
                    }

                    if (serialize != null)
                        viewModel.ChangesApplied += ViewModel_ChangesApplied;
                    break;

                case VisualizerTargetStateNotification.Unavailable:
                    isMessagingAvailable = false;
                    break;

                case VisualizerTargetStateNotification.ValueUpdated:
                    // Not initialized yet (a previous initialization might have been unsuccessful)
                    if (vm == null)
                        goto case VisualizerTargetStateNotification.Available;

                    isMessagingAvailable = true;
                    SetNotification(null);
                    try
                    {
                        data = await visualizerTarget.ObjectSource.RequestDataAsync(default(ReadOnlySequence<byte>), CancellationToken.None) ?? default;
                        using (Stream stream = data.AsStream())
                            model = deserialize.Invoke(stream);

                        // Failed to update the view model: discarding the newly deserialized model
                        if (!vm.TrySetModel(model))
                            (model as IDisposable)?.Dispose();
                    }
                    catch (Exception e)
                    {
                        SetNotification(Res.WarningMessageDebuggerVisualizerCannotUpdate(e.Message), ValidationSeverity.Warning);
                    }

                    break;
            }
        }

        #endregion

        #region Event handlers

        [SuppressMessage("ReSharper", "AsyncVoidMethod", Justification = "False alarm, event handler")]
        [SuppressMessage("VS2022", "VSTHRD100:Avoid \"async void\" methods, because any exceptions not handled by the method will crash the process", Justification = "False alarm, event handler")]
        private async void ViewModel_ChangesApplied(object sender, EventArgs e)
        {
            if (serialize == null)
                return;

            if (!isMessagingAvailable)
            {
                // If messaging is not available, then we just ignore the changes and reset the modified state of the view model.
                // This can happen when the debugged process is paused during the request.
                (viewModel as ObservableObjectBase)?.SetModified(true);
                SetNotification(Res.WarningMessageDebuggerVisualizerApplyNotAvailable, ValidationSeverity.Warning);
                return;
            }

            SetNotification(null);
            TModel editedModel = viewModel!.GetEditedModel();
            if (!ReferenceEquals(editedModel, lastAppliedModel))
                (lastAppliedModel as IDisposable)?.Dispose();

            lastAppliedModel = editedModel;

            try
            {
                using var ms = new MemoryStream();
                serialize.Invoke(editedModel, ms);
                await visualizerTarget.ObjectSource.ReplaceTargetObjectAsync(ms.AsReadOnlySequence(), CancellationToken.None);
            }
            catch (Exception ex)
            {
                // Failed to replace the target object. We just ignore this exception and trying to reset the modified state of the view model.
                (viewModel as ObservableObjectBase)?.SetModified(true);
                SetNotification(Res.ErrorMessageDebuggerVisualizerCannotApply(ex.Message));
            }
        }

        private void WpfVisualizerAdapter_Unloaded(object sender, RoutedEventArgs e)
        {
            // Required to avoid memory leaks. When the visualizer Style is not ToolWindow but ModalDialog, then VS2022 may not call Dispose
            // until VS is closed, even if the visualizer is closed.
            Unloaded -= WpfVisualizerAdapter_Unloaded;
            Dispose();
        }

        #endregion

        #endregion
    }
}
#endif