#region Usings

using Microsoft.VisualStudio.Shell.Interop;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarTextSpan in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal class InfoBarTextSpan : IVsInfoBarTextSpan
    {
        #region Properties

        public string Text { get; }
        public bool Bold => false;
        public bool Italic => false;
        public bool Underline => false;

        #endregion

        #region Constructors

        internal InfoBarTextSpan(string text) => Text = text;

        #endregion
    }
}
