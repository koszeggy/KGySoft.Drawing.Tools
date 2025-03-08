#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Notifications.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Diagnostics;

using EnvDTE;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    /// <summary>
    /// A service for notifications. Tries to display messages as info bars in the first place,
    /// falls back to dialogs if services for info bar are not available (VS2013).
    /// </summary>
    internal static class Notifications
    {
        #region Nested Classes

        private sealed class InfoBar : InfoBarModel, IVsInfoBarUIEvents
        {
            #region Fields

            private uint cookie;

            #endregion

            #region Constructors

            internal InfoBar(ImageMoniker icon, IEnumerable<IVsInfoBarTextSpan> textSpans, IEnumerable<IVsInfoBarActionItem> actionItems)
                : base(textSpans, actionItems, icon)
            {
            }

            #endregion

            #region Methods

            #region Public Methods

            public void OnActionItemClicked(IVsInfoBarUIElement uiElement, IVsInfoBarActionItem actionItem) => (actionItem.ActionContext as Action)?.Invoke();
            public void OnClosed(IVsInfoBarUIElement uiElement) => uiElement.Unadvise(cookie);

            #endregion

            #region Internal Methods

            internal void Assign(IVsInfoBarUIElement uiElement) => uiElement.Advise(this, out cookie);

            #endregion

            #endregion
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal static void Error(string message)
        {
            if (!TryShowInfoBar(KnownMonikers.StatusError, message))
                ShowMessageBox(OLEMSGICON.OLEMSGICON_CRITICAL, message);
        }

        internal static void Warning(string message)
        {
            if (!TryShowInfoBar(KnownMonikers.StatusWarning, message))
                ShowMessageBox(OLEMSGICON.OLEMSGICON_WARNING, message);
        }

        internal static void Info(string message)
        {
            if (!TryShowInfoBar(KnownMonikers.StatusInformation, message))
                ShowMessageBox(OLEMSGICON.OLEMSGICON_INFO, message);
        }

        /// <summary>
        /// The link and button click handlers should be defined as <see cref="Action"/>s in <see cref="IVsInfoBarActionItem.ActionContext"/>.
        /// <paramref name="additionalSpans"/> and <paramref name="actionItems"/> may not be displayed if info bars are not supported (VS2013)
        /// </summary>
        internal static void Info(string mainMessage, IEnumerable<IVsInfoBarTextSpan>? additionalSpans, IEnumerable<IVsInfoBarActionItem>? actionItems = null)
        {
            IEnumerable<IVsInfoBarTextSpan> textSpans = [new InfoBarTextSpan($"{Res.TitleMessageDialog}: {mainMessage}")];
            if (additionalSpans != null)
                textSpans = textSpans.Concat(additionalSpans);
            if (!TryShowInfoBar(KnownMonikers.StatusInformation, textSpans, actionItems ?? Array.Empty<IVsInfoBarActionItem>()))
                ShowMessageBox(OLEMSGICON.OLEMSGICON_INFO, mainMessage);
        }

        #endregion

        #region Private Methods

        private static bool TryShowInfoBar(ImageMoniker icon, string message)
            => TryShowInfoBar(icon, [new InfoBarTextSpan($"{Res.TitleMessageDialog}: {message}")], Array.Empty<IVsInfoBarActionItem>());

        private static bool TryShowInfoBar(ImageMoniker icon, IEnumerable<IVsInfoBarTextSpan> textSpans, IEnumerable<IVsInfoBarActionItem> actionItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (Services.InfoBarUIFactory == null || Services.ShellService == null)
                return false;

            IVsInfoBarHost? host = GetMainWindowInfoBarHost();
            if (host == null && Services.DTE == null)
                return false;

            var infoBar = new InfoBar(icon, textSpans, actionItems);
            IVsInfoBarUIElement? uiElement = Services.InfoBarUIFactory.CreateInfoBar(infoBar);
            if (uiElement == null)
                return false;

            infoBar.Assign(uiElement);

            // Main window host was null, maybe it just didn't appear yet (splash screen or quick start dialog is visible).
            // In this case we defer displaying the info bar until it appears
            if (host == null)
            {
                Debug.Assert(Services.DTE != null);
                _dispWindowEvents_WindowActivatedEventHandler? handler = null;
                handler = (_, _) =>
                {
                    host = GetMainWindowInfoBarHost();
                    if (host == null)
                        return;

                    Services.DTE!.Events.WindowEvents.WindowActivated -= handler;
                    host.AddInfoBar(uiElement);
                };

                Services.DTE!.Events.WindowEvents.WindowActivated += handler;
                return true;
            }

            host.AddInfoBar(uiElement);
            return true;
        }

        private static void ShowMessageBox(OLEMSGICON icon, string message)
            => VsShellUtilities.ShowMessageBox(Services.ServiceProvider, message, Res.TitleMessageDialog, icon, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

        private static IVsInfoBarHost? GetMainWindowInfoBarHost()
        {
            if (Services.ShellService == null)
                return null;
            Services.ShellService.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var propObj);
            return propObj as IVsInfoBarHost;
        }

        #endregion

        #endregion
    }
}
