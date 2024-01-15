#if !VS2022_OR_GREATER

#region Usings

using System;

#endregion

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarButton in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal class InfoBarButton : InfoBarActionItem
    {
        internal InfoBarButton(string text, Action actionContext)
            : base(text, actionContext)
        {
        }

        public override bool IsButton => true;
    }
} 
#endif