#if !VS2022_OR_GREATER
#region Usings

using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarActionItem in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal abstract class InfoBarActionItem : InfoBarTextSpan, IVsInfoBarActionItem
    {
        #region Properties

        public abstract bool IsButton { get; }
        public object? ActionContext { get; }

        #endregion

        #region Constructors

        protected InfoBarActionItem(string text, object? actionContext = null)
            : base(text)
        {
            ActionContext = actionContext;
        }

        #endregion
    }
}
#endif