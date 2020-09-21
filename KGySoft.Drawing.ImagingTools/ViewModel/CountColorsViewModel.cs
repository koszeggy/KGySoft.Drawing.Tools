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

using KGySoft.ComponentModel;
using KGySoft.Drawing.Imaging;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class CountColorsViewModel : ViewModelBase, IViewModel<int?>
    {
        #region Nested classes

        #region CountTask class

        private sealed class CountTask
        {
            #region Fields

            internal IReadableBitmapData BitmapData;
            internal volatile bool IsCanceled;
            internal IAsyncResult AsyncResult;

            #endregion

            #region Methods

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
                    // must not be in a lock because then EndCountColors could possibly never end
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private CountTask task;
        private readonly DrawingProgressManager drawingProgressManager;
        private int? colorCount;

        #endregion

        #region Properties

        internal object ProgressSyncRoot => drawingProgressManager;
        internal bool IsProcessing { get => Get<bool>(); set => Set(value); }
        internal DrawingProgress Progress { get => Get<DrawingProgress>(); set => Set(value); }
        internal string DisplayText { get => Get<string>(Res.TextCountingColors); set => Set(value); }

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
            task.WaitForCompletion();
            return colorCount;
        }

        #endregion

        #region Internal Methods

        internal void CancelIfRunning()
        {
            if (task.AsyncResult.IsCompleted)
                return;
            task.IsCanceled = true;
            SetModified(false);
            task.WaitForCompletion();
        }

        #endregion

        #region Protected Methods

        protected override bool AffectsModifiedState(string propertyName) => false;

        #endregion

        #region Private Methods

        private void BeginCountColors(Bitmap bitmap)
        {
            IsProcessing = true;
            task = new CountTask { BitmapData = bitmap.GetReadableBitmapData() };
            task.AsyncResult = task.BitmapData.BeginGetColorCount(new AsyncConfig
            {
                IsCancelRequestedCallback = () => task.IsCanceled,
                ThrowIfCanceled = false,
                Progress = drawingProgressManager,
                CompletedCallback = EndCountColors
            });
        }

        private void EndCountColors(IAsyncResult asyncResult)
        {
            Exception error = null;
            try
            {
                colorCount = asyncResult.EndGetColorCount();
            }
            catch (Exception e) when (!e.IsCriticalGdi())
            {
                error = e;
            }
            finally
            {
                task.BitmapData.Dispose();
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

            SynchronizedInvokeCallback?.Invoke(Action);
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
