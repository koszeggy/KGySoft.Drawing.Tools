#region Usings

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarModel in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal class InfoBarModel : IVsInfoBar
    {
        #region Nested classes

        #region TextSpanCollection class

        private class TextSpanCollection : IVsInfoBarTextSpanCollection
        {
            #region Fields

            private readonly IReadOnlyList<IVsInfoBarTextSpan> textSpans;

            #endregion

            #region Properties

            public int Count => textSpans.Count;

            #endregion

            #region Constructors

            internal TextSpanCollection(IEnumerable<IVsInfoBarTextSpan> textSpans)
                => this.textSpans = (textSpans as IReadOnlyList<IVsInfoBarTextSpan>) ?? textSpans.ToArray();

            #endregion

            #region Methods

            public IVsInfoBarTextSpan GetSpan(int index) => textSpans[index];

            #endregion
        }

        #endregion

        #region ActionItemCollection class

        private sealed class ActionItemCollection : IVsInfoBarActionItemCollection
        {
            #region Fields

            private readonly IReadOnlyList<IVsInfoBarActionItem> actionItems;

            #endregion

            #region Properties

            public int Count => actionItems.Count;

            #endregion

            #region Constructors

            internal ActionItemCollection(IEnumerable<IVsInfoBarActionItem> actionItems)
                => this.actionItems = (actionItems as IReadOnlyList<IVsInfoBarActionItem>) ?? actionItems.ToArray();

            #endregion

            #region Methods

            public IVsInfoBarActionItem GetItem(int index) => actionItems[index];

            #endregion
        }

        #endregion

        #endregion

        #region Properties

        public IVsInfoBarActionItemCollection ActionItems { get; }
        public bool IsCloseButtonVisible => true;
        public IVsInfoBarTextSpanCollection TextSpans { get; }
        public ImageMoniker Image { get; }

        #endregion

        #region Constructors

        internal InfoBarModel(IEnumerable<IVsInfoBarTextSpan> textSpans, IEnumerable<IVsInfoBarActionItem> actionItems, ImageMoniker image)
        {
            Image = image;
            TextSpans = new TextSpanCollection(textSpans);
            ActionItems = new ActionItemCollection(actionItems);
        }

        #endregion
    }
}
