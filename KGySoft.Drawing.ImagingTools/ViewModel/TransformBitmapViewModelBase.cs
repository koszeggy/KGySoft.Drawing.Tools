﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TransformBitmapViewModelBase.cs
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
using System.ComponentModel;
using System.Drawing;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Threading;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal abstract class TransformBitmapViewModelBase : ViewModelBase<Bitmap?>, IValidatingObject
    {
        #region Nested Classes

        protected abstract class GenerateTaskBase : AsyncTaskBase
        {
            #region Methods

            internal abstract void Initialize(Bitmap source, bool isInUse);
            internal abstract IAsyncResult BeginGenerate(AsyncConfig asyncConfig);
            internal abstract Bitmap? EndGenerate(IAsyncResult asyncResult);

            #endregion
        }

        #endregion

        #region Fields

        private readonly Bitmap originalImage;
        private readonly object syncRoot = new object();

        private volatile GenerateTaskBase? activeTask;
        private bool keepResult;
        private DrawingProgressManager? drawingProgressManager;
        private EventHandler? validationResultsChangedHandler;

        #endregion

        #region Events

        internal event EventHandler? ValidationResultsChanged
        {
            add => value.AddSafe(ref validationResultsChangedHandler);
            remove => value.RemoveSafe(ref validationResultsChangedHandler);
        }

        #endregion

        #region Properties

        #region Public Properties

        public bool IsValid { get => Get<bool>(); private set => Set(value); }
        public ValidationResultsCollection ValidationResults { get => Get(DoValidation); private set => Set(value); }

        #endregion

        #region Internal Properties

        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommand ResetCommand => Get(() => new SimpleCommand(OnResetCommand));
        internal ICommandState ApplyCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState ResetCommandState => Get(() => new CommandState { Enabled = false });

        internal bool IsGenerating { get => Get<bool>(); set => Set(value); }
        internal AsyncProgress<DrawingOperation> Progress { get => Get<AsyncProgress<DrawingOperation>>(); set => Set(value); }

        #endregion

        #region Protected Properties

        protected abstract bool AreSettingsChanged { get; }

        #endregion

        #region Private Properties

        private Exception? GeneratePreviewError { get => Get<Exception?>(); set => Set(value); }

        #endregion

        #endregion

        #region Constructors

        protected TransformBitmapViewModelBase(Bitmap image)
        {
            originalImage = image ?? throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            PreviewImageViewModel previewImage = PreviewImageViewModel;
            previewImage.PropertyChanged += PreviewImage_PropertyChanged;
            previewImage.PreviewImage = previewImage.OriginalImage = image;
        }

        #endregion

        #region Methods

        #region Public Methods

        public override Bitmap? GetEditedModel() => PreviewImageViewModel.PreviewImage as Bitmap;

        #endregion

        #region Internal Methods

        internal override void ViewLoaded()
        {
            // could be in constructor but we only need it when there is a view
            drawingProgressManager = new DrawingProgressManager(TrySetProgress);
            base.ViewLoaded();
        }

        #endregion

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(ValidationResults):
                    var validationResults = (ValidationResultsCollection)e.NewValue!;
                    IsValid = !validationResults.HasErrors;
                    validationResultsChangedHandler?.Invoke(this, EventArgs.Empty);
                    return;

                case nameof(GeneratePreviewError):
                    Validate();
                    return;

                case nameof(IsModified):
                case nameof(IsGenerating):
                    ApplyCommandState.Enabled = IsModified && !IsGenerating;
                    if (e.PropertyName == nameof(IsGenerating))
                        PreviewImageViewModel.ShowOriginalEnabled = e.NewValue is false;
                    return;

                default:
                    if (!IsViewLoaded || !AffectsPreview(e.PropertyName!))
                        return;
                    Validate();
                    ResetCommandState.Enabled = AreSettingsChanged;
                    BeginGeneratePreview();
                    return;
            }
        }

        protected void Validate()
        {
            if (!IsViewLoaded)
                return;
            ValidationResults = DoValidation();
        }

        protected virtual ValidationResultsCollection DoValidation()
        {
            Exception? error = GeneratePreviewError;
            var result = new ValidationResultsCollection();

            // errors
            if (error != null)
                result.AddError(nameof(PreviewImageViewModel.PreviewImage), Res.ErrorMessageFailedToGeneratePreview(error.Message));

            return result;
        }

        // IsModified is set explicitly from PreviewImage_PropertyChanged
        protected override bool AffectsModifiedState(string propertyName) => false;

        protected abstract bool AffectsPreview(string propertyName);

        protected void BeginGeneratePreview()
        {
            // sending cancel request to pending generate but the completion is awaited on a pool thread to prevent the UI from lagging
            CancelRunningGenerate();
            GeneratePreviewError = null;

            // error - null
            if (!IsValid)
            {
                SetPreview(null);
                IsGenerating = false;
                return;
            }

            // Not awaiting the canceled task here to prevent the UI from lagging.
            IsGenerating = true;
            ThreadPool.QueueUserWorkItem(DoGenerate, CreateGenerateTask());
        }

        protected abstract GenerateTaskBase CreateGenerateTask();
        protected abstract bool MatchesSettings(GenerateTaskBase task);
        protected abstract bool MatchesOriginal(GenerateTaskBase task);
        protected virtual void ResetParameters() { }

        protected override void ApplyDisplayLanguage() => Validate();

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                if (activeTask != null)
                {
                    CancelRunningGenerate();
                    WaitForPendingGenerate();
                }

                Debug.Assert(activeTask == null);
                Image? preview = PreviewImageViewModel.PreviewImage;
                PreviewImageViewModel.Dispose();

                if (!ReferenceEquals(originalImage, preview) && !keepResult)
                    preview?.Dispose();

                drawingProgressManager = null;
                validationResultsChangedHandler = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void DoGenerate(object? state)
        {
            var task = (GenerateTaskBase)state!;

            // This is a fairly large lock ensuring that only one generate task is running at once.
            // Instead of this we could await the canceled task before queuing a new one but then the UI can freeze for some moments.
            // (It wouldn't cause deadlocks because here every TryInvokeSync is after completing the task.)
            // But many threads can be queued, which all stop here before acquiring the lock. To prevent spawning too many threads we
            // don't use a regular lock here but a bit active spinning that can exit without taking the lock if the task gets outdated.
            while (!Monitor.TryEnter(syncRoot, 1))
            {

                if (!IsDisposed && MatchesSettings(task))
                    continue;
                task.Dispose();
                return;
            }

            try
            {
                // lost race
                if (IsDisposed || !MatchesSettings(task))
                {
                    task.Dispose();
                    return;
                }

                // Awaiting the previous unfinished task. This could be also in BeginGeneratePreview but that may freeze the UI for some time.
                Debug.Assert(activeTask?.IsCanceled != false);
                WaitForPendingGenerate();
                Debug.Assert(activeTask == null);

                // resetting possible previous progress
                drawingProgressManager?.New(DrawingOperation.UndefinedProcessing);

                // from now on the task can be canceled
                activeTask = task;
                try
                {
                    // original image
                    if (MatchesOriginal(task))
                    {
                        task.SetCompleted();
                        TryInvokeSync(() =>
                        {
                            SetPreview(originalImage);
                            IsGenerating = false;
                        });
                        return;
                    }

                    // preparing generate (allocations, sync operations, etc.)
                    try
                    {
                        task.Initialize(originalImage, PreviewImageViewModel.PreviewImage == originalImage);
                    }
                    catch (Exception e)
                    {
                        task.SetCompleted();
                        TryInvokeSync(() =>
                        {
                            GeneratePreviewError = e;
                            SetPreview(null);
                            IsGenerating = false;
                        });
                        return;
                    }

                    Exception? error = null;
                    Bitmap? result = null;
                    try
                    {
                        // starting generate: using Begin.../End... methods instead of await ...Async so it is compatible even with .NET 3.5
                        IAsyncResult asyncResult = task.BeginGenerate(new AsyncConfig
                        {
                            // ReSharper disable once AccessToDisposedClosure - false alarm, newTask.Dispose() is called only on error
                            IsCancelRequestedCallback = () => task.IsCanceled,
                            ThrowIfCanceled = false,
                            Progress = drawingProgressManager,
                        });

                        // Waiting to be finished or canceled. As we are on a different thread blocking wait is alright
                        result = task.EndGenerate(asyncResult);
                    }
                    catch (Exception e)
                    {
                        error = e;
                    }
                    finally
                    {
                        task.SetCompleted();
                    }

                    if (task.IsCanceled)
                    {
                        result?.Dispose();
                        return;
                    }

                    // applying result (or error)
                    TryInvokeSync(() =>
                    {
                        GeneratePreviewError = error;
                        SetPreview(result);
                        IsGenerating = false;
                    });
                }
                finally
                {
                    task.Dispose();
                    activeTask = null;
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        private void TrySetProgress(AsyncProgress<DrawingOperation> progress)
        {
            if (IsDisposed)
                return;
            try
            {
                Progress = progress;
            }
            catch (ObjectDisposedException)
            {
                // lost race - just ignoring it
            }
        }

        private void CancelRunningGenerate()
        {
            GenerateTaskBase? runningTask = activeTask;
            if (runningTask == null)
                return;
            runningTask.IsCanceled = true;
        }

        private void WaitForPendingGenerate()
        {
            // In a non-UI thread it should be in a lock
            GenerateTaskBase? runningTask = activeTask;
            if (runningTask == null)
                return;
            runningTask.WaitForCompletion();
            runningTask.Dispose();
            activeTask = null;
        }

        private void SetPreview(Bitmap? image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image? toDispose = preview.PreviewImage;
            preview.PreviewImage = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
        }

        #endregion

        #region Event Handlers

        private void PreviewImage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            var vm = (PreviewImageViewModel)sender!;

            // preview image has been changed: updating IsModified accordingly
            if (e.PropertyName == nameof(vm.PreviewImage))
            {
                Image? image = vm.PreviewImage;
                SetModified(image != null && originalImage != image);
            }
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand()
        {
            // canceling any pending generate and waiting for finishing so no "image is locked elsewhere" will come from the main form for the original image
            CancelRunningGenerate();
            WaitForPendingGenerate();
            SetModified(false);
            CloseViewCallback?.Invoke();
        }

        private void OnApplyCommand()
        {
            Debug.Assert(!IsGenerating);
            keepResult = true;
            CloseViewCallback?.Invoke();
        }

        private void OnResetCommand() => ResetParameters();

        #endregion

        #endregion
    }
}
