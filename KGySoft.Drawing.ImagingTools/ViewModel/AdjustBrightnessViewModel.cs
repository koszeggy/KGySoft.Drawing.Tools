#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: AdjustBrightnessViewModel.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class AdjustBrightnessViewModel : ViewModelBase, IViewModel<Bitmap>, IValidatingObject
    {
        #region Nested Classes

        private sealed class GenerateTask : IDisposable
        {
            #region Fields

            #region Private Fields
            
            private Bitmap result;
            private ManualResetEventSlim completedEvent;
            private volatile bool isDisposed;

            #endregion

            #region Internal Fields
            
            internal volatile bool IsCanceled;

            #endregion

            #endregion

            #region Properties

            internal float Value { get; }
            internal ColorChannels ColorChannels { get; }
            internal IReadWriteBitmapData BitmapData { get; private set; }


            #endregion

            #region Constructors

            internal GenerateTask(float value, ColorChannels colorChannels)
            {
                Value = value;
                ColorChannels = colorChannels;
            }

            #endregion

            #region Methods

            #region Public Methods

            public void Dispose()
            {
                if (isDisposed)
                    return;
                BitmapData?.Dispose();
                result?.Dispose();
                completedEvent?.Set();
                completedEvent?.Dispose();
                isDisposed = true;
            }

            #endregion

            #region Internal Methods

            internal void Initialize(Bitmap bitmap)
            {
                result = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                BitmapData = result.GetReadWriteBitmapData();

                // Cloning the content by CopyTo, which is much faster than Bitmap.Clone.
                // Not using async for this one because it is quite fast and we don't want double progress
                using (var source = bitmap.GetReadableBitmapData())
                    source.CopyTo(BitmapData);

                completedEvent = new ManualResetEventSlim(false);
            }

            internal void ReleaseBitmapData()
            {
                BitmapData.Dispose();
                BitmapData = null;
            }

            internal Bitmap GetResult()
            {
                // Once result is obtained it is cleared to prevent dispose
                Bitmap bmp = result;
                result = null;
                return bmp;
            }

            internal void SetCompleted() => completedEvent.Set();

            internal void WaitForCompletion()
            {
                if (isDisposed)
                    return;

                try
                {
                    completedEvent.Wait();
                }
                catch (ObjectDisposedException)
                {
                    // it can happen that the task has just been completed after querying IsCompleted but this part
                    // must not be in a lock because then EndGeneratePreview could possibly never end
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #region Fields

        private readonly Bitmap originalImage;
        private readonly object syncRoot = new object();

        private volatile GenerateTask activeTask;
        private bool initializing = true;
        private bool keepResult;
        private DrawingProgressManager drawingProgressManager;

        #endregion

        #region Events

        internal event EventHandler<EventArgs<ValidationResultsCollection>> ValidationResultsChanged
        {
            add => ValidationResultsChangedHandler += value;
            remove => ValidationResultsChangedHandler -= value;
        }

        #endregion

        #region Properties

        #region Public Properties

        public bool IsValid { get => Get<bool>(); set => Set(value); }
        public ValidationResultsCollection ValidationResults { get => Get(DoValidation); set => Set(value); }

        #endregion

        #region Internal Properties

        internal ColorChannels ColorChannels { get => Get(ColorChannels.Rgb); set => Set(value); }
        internal float Value { get => Get<float>(); set => Set(value); }
        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommand ResetCommand => Get(() => new SimpleCommand(OnResetCommand));
        internal ICommandState ApplyCommandState => Get(() => new CommandState { Enabled = false });
        internal ICommandState ResetCommandState => Get(() => new CommandState { Enabled = false });

        internal bool IsGenerating { get => Get<bool>(); set => Set(value); }
        internal DrawingProgress Progress { get => Get<DrawingProgress>(); set => Set(value); }

        #endregion

        #region Private Properties

        private Exception GeneratePreviewError { get => Get<Exception>(); set => Set(value); }
        private EventHandler<EventArgs<ValidationResultsCollection>> ValidationResultsChangedHandler { get => Get<EventHandler<EventArgs<ValidationResultsCollection>>>(); set => Set(value); }

        #endregion

        #endregion

        #region Constructors

        internal AdjustBrightnessViewModel(Bitmap image)
        {
            originalImage = image ?? throw new ArgumentNullException(nameof(image), PublicResources.ArgumentNull);
            PreviewImageViewModel previewImage = PreviewImageViewModel;
            previewImage.PropertyChanged += PreviewImage_PropertyChanged;
            previewImage.Image = image;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnPropertyChanged(PropertyChangedExtendedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(ColorChannels):
                case nameof(Value):
                    if (initializing)
                        return;
                    Validate();
                    ResetCommandState.Enabled = !Value.Equals(0f) || ColorChannels != ColorChannels.Rgb;
                    BeginGeneratePreview();
                    return;

                case nameof(ValidationResults):
                    var validationResults = (ValidationResultsCollection)e.NewValue;
                    IsValid = !validationResults.HasErrors;
                    ValidationResultsChangedHandler?.Invoke(this, new EventArgs<ValidationResultsCollection>(validationResults));
                    return;

                case nameof(GeneratePreviewError):
                    Validate();
                    return;

                case nameof(IsModified):
                case nameof(IsGenerating):
                    ApplyCommandState.Enabled = IsModified && !IsGenerating;
                    return;
            }
        }

        // IsModified is set explicitly from PreviewImage_PropertyChanged
        protected override bool AffectsModifiedState(string propertyName) => false;

        internal override void ViewLoaded()
        {
            // could be in constructor but we only need it when there is a view
            drawingProgressManager = new DrawingProgressManager(p => Progress = p);
            initializing = false;
            base.ViewLoaded();
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            if (disposing)
            {
                Image preview = PreviewImageViewModel.Image;
                PreviewImageViewModel?.Dispose();

                // ReSharper disable once InconsistentlySynchronizedField - locking is only needed for cloning
                if (!ReferenceEquals(originalImage, preview) && !keepResult)
                    preview?.Dispose();

                drawingProgressManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void BeginGeneratePreview()
        {
            // sending cancel request to pending generate but the completion is awaited on a pool thread to prevent deadlocks
            CancelRunningGenerate();

            GeneratePreviewError = null;
            IsGenerating = true;
            ThreadPool.QueueUserWorkItem(DoGenerate, new GenerateTask(Value, ColorChannels));
        }

        [SuppressMessage("Reliability", "CA2002:Do not lock on objects with weak identity", Justification = "False alarm, originalImage is not a remote object")]
        private void DoGenerate(object state)
        {
            var task = (GenerateTask)state;

            // this is a fairly large lock ensuring that only one generate task is running at once
            lock (syncRoot)
            {
                Debug.Assert(activeTask?.IsCanceled != false);
                WaitForPendingGenerate();
                Debug.Assert(activeTask == null);

                // lost race
                if (!task.Value.Equals(Value) || task.ColorChannels != ColorChannels)
                {
                    task.Dispose();
                    return;
                }

                // original image
                if (task.Value.Equals(0f) || task.ColorChannels == ColorChannels.None)
                {
                    SynchronizedInvokeCallback?.Invoke(() =>
                    {
                        SetPreview(originalImage);
                        IsGenerating = false;
                    });
                    return;
                }

                // from now on the task can be canceled
                activeTask = task;
                try
                {
                    // Locking on source image to avoid "bitmap region is already locked" if the UI is painting the image when we clone it
                    // This works this way because UI can repaint the image any time and is also locks the image for that period.
                    // Another solution could be if we used a clone of the original image but it is better to avoid using multiple clones.
                    lock (originalImage)
                    {
                        // preparing for the generate
                        try
                        {
                            task.Initialize(originalImage);
                        }
                        catch (Exception e) when (!e.IsCriticalGdi())
                        {
                            SynchronizedInvokeCallback?.Invoke(() =>
                            {
                                GeneratePreviewError = e;
                                SetPreview(null);
                                IsGenerating = false;
                            });
                            return;
                        }
                    }

                    // starting generate: using Begin.../End... methods instead of await ...Async so it is compatible even with .NET 3.5
                    IAsyncResult asyncResult = task.BitmapData.BeginAdjustBrightness(task.Value, channels: task.ColorChannels, asyncConfig: new AsyncConfig
                    {
                        // ReSharper disable once AccessToDisposedClosure - false alarm, newTask.Dispose() is called only on error
                        IsCancelRequestedCallback = () => task.IsCanceled,
                        ThrowIfCanceled = false,
                        State = task,
                        Progress = drawingProgressManager
                    });

                    // Waiting to be finished or canceled. As we are on a different thread blocking wait is alright
                    Exception error = null;
                    try
                    {
                        asyncResult.EndAdjustBrightness();
                    }
                    catch (Exception e) when (!e.IsCriticalGdi())
                    {
                        error = e;
                    }
                    finally
                    {
                        task.ReleaseBitmapData();
                        task.SetCompleted();
                    }

                    if (task.IsCanceled)
                        return;

                    // applying result (or error)
                    Bitmap result = error == null ? task.GetResult() : null;
                    SynchronizedInvokeCallback?.Invoke(() =>
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
        }

        private void CancelRunningGenerate()
        {
            GenerateTask runningTask = activeTask;
            if (runningTask == null)
                return;
            runningTask.IsCanceled = true;
        }

        private void WaitForPendingGenerate()
        {
            // In a non-UI thread it should be in a lock
            GenerateTask runningTask = activeTask;
            if (runningTask == null)
                return;
            runningTask.WaitForCompletion();
            runningTask.Dispose();
            activeTask = null;
        }

        private void SetPreview(Bitmap image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image toDispose = preview.Image;
            preview.Image = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
        }

        private void Validate()
        {
            if (initializing)
                return;
            ValidationResults = DoValidation();
        }

        private ValidationResultsCollection DoValidation()
        {
            Exception error = GeneratePreviewError;
            var result = new ValidationResultsCollection();

            // errors
            if (error != null)
                result.AddError(nameof(PreviewImageViewModel.Image), Res.ErrorMessageFailedToGeneratePreview(error.Message));

            return result;
        }

        #endregion

        #region Event Handlers

        private void PreviewImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = (PreviewImageViewModel)sender;

            // preview image has been changed: updating IsModified accordingly
            if (e.PropertyName == nameof(vm.Image))
            {
                Image image = vm.Image;
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

        private void OnResetCommand()
        {
            Value = 0f;
            ColorChannels = ColorChannels.Rgb;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Bitmap IViewModel<Bitmap>.GetEditedModel() => PreviewImageViewModel.Image as Bitmap;

        #endregion

        #endregion
    }
}
