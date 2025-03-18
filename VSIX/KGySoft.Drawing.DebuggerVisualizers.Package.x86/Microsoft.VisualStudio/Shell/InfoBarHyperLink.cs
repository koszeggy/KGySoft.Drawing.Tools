#region Usings

using System;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarHyperlink in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal class InfoBarHyperlink : InfoBarActionItem
    {
        #region Properties

        public override bool IsButton => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new hyperlink with optional formatting options.
        /// </summary>
        /// <param name="text">The text for the span.</param>
        /// <param name="actionContext">An optional tag which identifies this action item.</param>
        internal InfoBarHyperlink(string text, Action actionContext)
            : base(text, actionContext)
        {
        }

        #endregion
    }
}
