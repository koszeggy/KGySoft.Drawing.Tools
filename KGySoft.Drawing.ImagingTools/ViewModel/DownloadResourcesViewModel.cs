#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadResourcesViewModel.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Serialization.Xml;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DownloadResourcesViewModel : ViewModelBase, IViewModel<ICollection<LocalizationInfo>>
    {
        #region Nested classes

        #region DownloadManifestTask class

        private class DownloadTask : AsyncTaskBase
        {
            #region Fields

            internal Uri Uri = default!;

            #endregion
        }

        #endregion

        #region DownloadResourcesTask class

        private sealed class DownloadResourcesTask : DownloadTask
        {
            #region Fields

            internal List<DownloadInfo> Files = default!;
            internal bool Overwrite;

            #endregion
        }

        #endregion

        #region DownloadInfo class

        private sealed class DownloadInfo
        {
            #region Fields

            private readonly string remotePath;

            #endregion

            #region Properties

            internal LocalizationInfo Info { get; }
            internal string FileName { get; }
            internal string RemoteUri => $"{remotePath}/{FileName}";
            internal string LocalPath => Path.Combine(Res.ResourcesDir, FileName);

            #endregion

            #region Constructors

            public DownloadInfo(LocalizationInfo info, LocalizableLibraries library)
            {
                Info = info;
                FileName = info.CultureName == Res.DefaultLanguage.Name
                    ? ResHelper.GetBaseName(library) + ".resx"
                    : $"{ResHelper.GetBaseName(library)}.{info.CultureName}.resx";
                remotePath = $"{info.CultureName}_{info.Author}_{info.ImagingToolsVersion}";
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private volatile AsyncTaskBase? activeTask;
        private volatile HashSet<LocalizationInfo> downloadedCultures = new HashSet<LocalizationInfo>();

        #endregion

        #region Properties

        internal DownloadableResourceItemCollection? Items { get => Get<DownloadableResourceItemCollection?>(); set => Set(value); }
        internal bool IsProcessing { get => Get<bool>(); set => Set(value); }
        internal (int MaximumValue, int CurrentValue) Progress { get => Get<(int, int)>(); set => Set(value); }
        
        internal ICommand CancelCommand => Get(() => new SimpleCommand(OnCancelCommand));
        internal ICommand DownloadCommand => Get(() => new SimpleCommand(OnDownloadCommand));

        internal ICommandState DownloadCommandState => Get(() => new CommandState { Enabled = false });

        #endregion

        #region Methods

        #region Internal Methods

        internal void CancelIfRunning()
        {
            AsyncTaskBase? t = activeTask;
            if (t == null)
                return;

            t.IsCanceled = true;
            SetModified(false);
            t.WaitForCompletion();
        }

        #endregion

        #region Protected Methods

        protected override bool AffectsModifiedState(string propertyName) => false;

        internal override void ViewLoaded()
        {
            base.ViewLoaded();
            try
            {
                BeginDownloadManifest();
            }
            catch (Exception e) when (!e.IsCritical())
            {
                ShowError(Res.ErrorMessageCouldNotAccessOnlineResources(e.Message));
                CloseViewCallback?.Invoke();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
                Items?.Dispose();

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void BeginDownloadManifest()
        {
            IsProcessing = true;
            activeTask = new DownloadTask { Uri = new Uri(Configuration.BaseUri, "manifest.xml") };
            ThreadPool.QueueUserWorkItem(DoDownloadManifest, activeTask);
        }

        private void DoDownloadManifest(object state)
        {
            var task = (DownloadTask)state!;
            try
            {
                if (task.IsCanceled)
                    return;

                Progress = (2, 0);
                byte[]? data = Download(task);
                if (data == null)
                    return;

                using var reader = XmlReader.Create(new StreamReader(new MemoryStream(data), Encoding.UTF8));
                reader.ReadStartElement("manifest");
                var itemsList = new List<LocalizationInfo>();
                XmlSerializer.DeserializeContent(reader, itemsList);

                TryInvokeSync(() =>
                {
                    var items = new DownloadableResourceItemCollection(itemsList);
                    items.ListChanged += Items_ListChanged;
                    Items = items;
                    IsProcessing = false;
                });
            }
            catch (Exception e) when (!e.IsCritical())
            {
                TryInvokeSync(() =>
                {
                    ShowError(Res.ErrorMessageCouldNotAccessOnlineResources(e.Message));
                    CloseViewCallback?.Invoke();
                });
            }
            finally
            {
                task.Dispose();
                activeTask = null;
            }
        }

        private void BeginDownloadResources(List<DownloadInfo> toDownload, bool overwrite)
        {
            DownloadCommandState.Enabled = false;
            IsProcessing = true;
            activeTask = new DownloadResourcesTask { Files = toDownload, Overwrite = overwrite };
            ThreadPool.QueueUserWorkItem(DoDownloadResources, activeTask);
        }

        private void DoDownloadResources(object state)
        {
            var task = (DownloadResourcesTask)state!;
            string current = null!;
            try
            {
                if (task.IsCanceled)
                    return;

                // x3: 2 for download (retrieving response + downloading content), 1 for saving the file
                Progress = (task.Files.Count * 3, 0);
                int downloaded = 0;
                foreach (DownloadInfo downloadInfo in task.Files)
                {
                    current = downloadInfo.FileName;
                    if (task.IsCanceled)
                        return;

                    if (!task.Overwrite && File.Exists(downloadInfo.LocalPath))
                    {
                        IncrementProgress(3);
                        continue;
                    }

                    // downloading in memory first
                    task.Uri = new Uri(Configuration.BaseUri, downloadInfo.RemoteUri);
                    byte[]? data = Download(task);
                    if (data == null)
                        return;

                    // if there was no issue with downloading, then saving the file
                    File.WriteAllBytes(downloadInfo.LocalPath, data);
                    downloadedCultures.Add(downloadInfo.Info);
                    IncrementProgress();
                    downloaded += 1;
                    SetModified(true);
                }

                TryInvokeSync(() =>
                {
                    IsProcessing = false;
                    if (downloadedCultures.All(i => ResHelper.TryGetCulture(i.CultureName, out var _)))
                        ShowInfo(Res.InfoMessageDownloadCompleted(downloaded));
                    else
                        ShowWarning(Res.WarningMessageDownloadCompletedWithUnsupportedCultures(downloaded));
                    CloseViewCallback?.Invoke();
                });
            }
            catch (Exception e) when (!e.IsCritical())
            {
                // not clearing the downloadedCultures because those files are removed
                TryInvokeSync(() =>
                {
                    IsProcessing = false;
                    DownloadCommandState.Enabled = true;
                });
                ShowError(Res.ErrorMessageFailedToDownloadResource(current, e.Message));
            }
            finally
            {
                task.Dispose();
                activeTask = null;
            }
        }

        /// <summary>
        /// Returns the downloaded content, or null if task was canceled.
        /// Always increments 2 in progress: 1 for response, 1 for the downloaded content.
        /// </summary>
        private byte[]? Download(DownloadTask task)
        {
            // Not using WebClient and its async methods because we are already on a separate thread
            // and this way we can use the same thread for multiple files, too.
            var request = WebRequest.Create(task.Uri);
            using WebResponse response = request.GetResponse();
            if (task.IsCanceled)
                return null;

            // We do not use the file size in the progress because
            // 1.) We work with small files
            // 2.) When we download more files we can't set the maximum value for all of the files
            IncrementProgress();
            using Stream? src = response.GetResponseStream();
            if (src == null)
                return null;

            int len = (int)response.ContentLength;
            var result = new byte[len];
            int offset = 0;
            int count;
            while ((count = src.Read(result, offset, result.Length - offset)) > 0)
            {
                if (task.IsCanceled)
                    return null;
                offset += count;
            }

            IncrementProgress();
            return result;
        }

        private void IncrementProgress(int value = 1)
        {
            var current = Progress;
            Progress = (current.MaximumValue, current.CurrentValue + value);
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        ICollection<LocalizationInfo> IViewModel<ICollection<LocalizationInfo>>.GetEditedModel() => downloadedCultures;

        #endregion

        #region Event Handlers

        private void Items_ListChanged(object sender, ListChangedEventArgs e)
        {
            var items = (DownloadableResourceItemCollection)sender;
            if (e.ListChangedType != ListChangedType.ItemChanged || e.PropertyDescriptor?.Name != nameof(DownloadableResourceItem.Selected))
                return;

            DownloadCommandState.Enabled = items[e.NewIndex].Selected || items.Any(i => i.Selected);
        }

        #endregion

        #region Command Handlers

        private void OnCancelCommand()
        {
            CancelIfRunning();
            CloseViewCallback?.Invoke();
        }

        private void OnDownloadCommand()
        {
            var toDownload = new List<DownloadInfo>();
            var existingFiles = new List<string>();
            bool ignoreVersionMismatch = false;
            Version selfVersion = InstallationManager.ImagingToolsVersion;

            foreach (DownloadableResourceItem item in Items!)
            {
                if (!item.Selected)
                    continue;

                if (!ignoreVersionMismatch && !selfVersion.NormalizedEquals(item.Info.ImagingToolsVersion))
                {
                    if (Confirm(Res.ConfirmMessageResourceVersionMismatch, false))
                        ignoreVersionMismatch = true;
                    else
                        return;
                }

                LocalizationInfo info = item.Info;
                foreach (LocalizableLibraries lib in info.ResourceSets.GetFlags(false))
                {
                    var file = new DownloadInfo(info, lib);
                    toDownload.Add(file);
                    if (File.Exists(file.LocalPath))
                        existingFiles.Add(file.FileName);
                }
            }

            bool overwrite = false;
            if (existingFiles.Count > 0)
            {
                bool? confirmResult = CancellableConfirm(Res.ConfirmMessageOverwriteResources(existingFiles.Join(Environment.NewLine)), 1);
                if (confirmResult == null)
                    return;

                overwrite = confirmResult.Value;
            }

            BeginDownloadResources(toDownload, overwrite);
        }

        #endregion

        #endregion
    }
}