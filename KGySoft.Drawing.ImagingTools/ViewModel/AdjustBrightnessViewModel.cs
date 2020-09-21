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
using System.Drawing.Imaging;
using System.Linq;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class AdjustBrightnessViewModel : ViewModelBase, IViewModel<Bitmap>
    {
        #region Nested Classes

        private sealed class GenerateTask : IDisposable
        {
            #region Fields

            internal readonly IReadWriteBitmapData BitmapData;

            internal Bitmap Result;
            internal volatile bool IsCanceled;
            internal IAsyncResult AsyncResult;

            #endregion

            #region Constructors

            internal GenerateTask(Bitmap bitmap)
            {
                Result = bitmap.Clone(new Rectangle(Point.Empty, bitmap.Size), bitmap.PixelFormat);
                BitmapData = Result.GetReadWriteBitmapData();
            }

            #endregion

            #region Methods

            #region Public Methods

            public void Dispose()
            {
                BitmapData.Dispose();
                Result?.Dispose();
            }

            #endregion

            #region Internal Methods

            internal void WaitForCompletion()
            {
                if (AsyncResult.IsCompleted)
                    return;

                try
                {
                    AsyncResult.AsyncWaitHandle.WaitOne();
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

        private bool initializing = true;
        private bool keepResult;
        private volatile GenerateTask activeTask;
        private DrawingProgressManager drawingProgressManager;

        #endregion

        #region Properties

        internal ColorChannels ColorChannels { get => Get(ColorChannels.Rgb); set => Set(value); }
        internal float Value { get => Get<float>(); set => Set(value); }
        internal PreviewImageViewModel PreviewImageViewModel => Get(() => new PreviewImageViewModel());

        internal ICommand ApplyCommand => Get(() => new SimpleCommand(OnApplyCommand));
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommandState ApplyCommandState => Get(() => new CommandState { Enabled = false });

        internal bool IsGenerating { get => Get<bool>(); set => Set(value); }
        internal DrawingProgress Progress { get => Get<DrawingProgress>(); set => Set(value); }

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
                    BeginGeneratePreview();
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
            IsGenerating = true;

            // canceling any current generating in progress (not nullifying ActiveTask because that could enable the Apply command,
            // but it will done in EndGeneratePreview if no new task is added in this method)
            GenerateTask canceledTask = CancelGeneratePreview();

            // checking whether a new generate should be added
            float value = Value;
            ColorChannels channels = ColorChannels;

            // original image
            if (value.Equals(0f) || channels == ColorChannels.None)
            {
                SetPreview(originalImage);
                IsGenerating = false;
                return;
            }

            // waiting for the cancellation end to prevent the possible "The image is locked elsewhere" error
            canceledTask?.WaitForCompletion();

            // generating a new image
            lock (syncRoot)
            {
                // using Begin/EndConvertPixelFormat instead of await ConvertPixelFormatAsync so it is compatible even with .NET 3.5
                var newTask = new GenerateTask(originalImage);
                newTask.AsyncResult = newTask.BitmapData.BeginAdjustBrightness(value, channels: channels,
                    asyncConfig: new AsyncConfig
                    {
                        IsCancelRequestedCallback = () => newTask.IsCanceled,
                        ThrowIfCanceled = false,
                        State = newTask,
                        CompletedCallback = EndGeneratePreview,
                        Progress = drawingProgressManager
                    });

                activeTask = newTask;
            }
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure",
            Justification = "False alarm, SynchronizedInvokeCallback waits for the execution to be finished")]
        private void EndGeneratePreview(IAsyncResult asyncResult)
        {
            Exception error = null;
            var task = (GenerateTask)asyncResult.AsyncState;

            try
            {
                asyncResult.EndAdjustBrightness();
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                error = e;
            }

            lock (syncRoot)
            {
                try
                {
                    if (task.IsCanceled)
                        return;

                    // the execution of this method will be marshaled back to the UI thread
                    void Action()
                    {
                        if (error == null)
                        {
                            SetPreview(task.Result);
                            task.Result = null; // to prevent disposing
                        }
                        else
                            ShowError(Res.ErrorMessage(error.Message));
                        IsGenerating = false;
                    }

                    SynchronizedInvokeCallback?.Invoke(Action);
                }
                finally
                {
                    task.Dispose();
                    activeTask = null;
                }
            }
        }

        private GenerateTask CancelGeneratePreview()
        {
            GenerateTask runningTask;
            //lock (syncRoot)
                runningTask = activeTask;
            if (runningTask != null)
                runningTask.IsCanceled = true;
            return runningTask;
        }

        private void SetPreview(Bitmap image)
        {
            PreviewImageViewModel preview = PreviewImageViewModel;
            Image toDispose = preview.Image;
            preview.Image = image;
            if (toDispose != null && toDispose != originalImage)
                toDispose.Dispose();
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
            CancelGeneratePreview()?.WaitForCompletion();
            SetModified(false);
            CloseViewCallback?.Invoke();
        }

        private void OnApplyCommand()
        {
            Debug.Assert(!IsGenerating);
            keepResult = true;
            CloseViewCallback?.Invoke();
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        Bitmap IViewModel<Bitmap>.GetEditedModel() => PreviewImageViewModel.Image as Bitmap;

        #endregion

        #endregion
    }
}
