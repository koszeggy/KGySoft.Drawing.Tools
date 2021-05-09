#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CountColorsViewModel.cs
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
using System.Drawing;
using System.Threading;

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class CountColorsViewModel : ViewModelBase, IViewModel<int?>
    {
        #region Nested classes

        #region CountTask class

        private sealed class CountTask : AsyncTaskBase
        {
            #region Fields

            internal Bitmap Bitmap = default!;

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private readonly DrawingProgressManager drawingProgressManager;
        
        private volatile CountTask? task;
        private int? colorCount;

        #endregion

        #region Properties

        internal object ProgressSyncRoot => drawingProgressManager;
        internal bool IsProcessing { get => Get<bool>(); set => Set(value); }
        internal DrawingProgress Progress { get => Get<DrawingProgress>(); set => Set(value); }
        internal string DisplayText { get => Get(() => Res.TextCountingColors); set => Set(value); }

        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));

        #endregion

        #region Constructors

        internal CountColorsViewModel(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap), PublicResources.ArgumentNull);
            drawingProgressManager = new DrawingProgressManager(p =>
            {
                lock (ProgressSyncRoot)
                    Progress = p;
            });
            BeginCountColors(bitmap);
        }

        #endregion

        #region Methods

        #region Public Methods

        public int? GetEditedModel()
        {
            task?.WaitForCompletion();
            return colorCount;
        }

        #endregion

        #region Internal Methods

        internal void CancelIfRunning()
        {
            CountTask? t = task;
            if (t == null)
                return;

            t.IsCanceled = true;
            SetModified(false);
            t.WaitForCompletion();
        }

        #endregion

        #region Protected Methods

        protected override bool AffectsModifiedState(string propertyName) => false;

        #endregion

        #region Private Methods

        private void BeginCountColors(Bitmap bitmap)
        {
            IsProcessing = true;
            task = new CountTask { Bitmap = bitmap };
            ThreadPool.QueueUserWorkItem(DoCountColors);
        }

        private void DoCountColors(object? state)
        {
            Exception? error = null;

            // We must lock on the image to avoid the possible "bitmap region is already in use" error from the Paint of main view's image viewer,
            // which also locks on the image to help avoiding this error
            lock (task!.Bitmap)
            {
                IReadableBitmapData? bitmapData = null;
                try
                {
                    bitmapData = task.Bitmap.GetReadableBitmapData();
                    IAsyncResult asyncResult = bitmapData.BeginGetColorCount(new AsyncConfig
                    {
                        IsCancelRequestedCallback = () => task.IsCanceled,
                        ThrowIfCanceled = false,
                        Progress = drawingProgressManager
                    });

                    // Waiting to be finished or canceled. As we are on a different thread blocking wait is alright
                    colorCount = asyncResult.EndGetColorCount();
                }
                catch (Exception e) when (!e.IsCriticalGdi())
                {
                    error = e;
                }
                finally
                {
                    bitmapData?.Dispose();
                    task.SetCompleted();
                }
            }

            if (task.IsCanceled)
                colorCount = null;

            SetModified(colorCount.HasValue);

            // the execution of this method will be marshaled back to the UI thread
            void Action()
            {
                DisplayText = error != null ? Res.ErrorMessage(error.Message)
                    : colorCount == null ? Res.TextOperationCanceled
                    : Res.TextColorCount(colorCount.Value);
                IsProcessing = false;
            }

            TryInvokeSync(Action);
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand()
        {
            CancelIfRunning();
            CloseViewCallback?.Invoke();
        }

        #endregion

        #endregion
    }
}
