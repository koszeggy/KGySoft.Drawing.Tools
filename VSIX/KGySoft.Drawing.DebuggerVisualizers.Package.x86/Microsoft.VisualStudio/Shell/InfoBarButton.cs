#region Usings

using System;

#endregion

// ReSharper disable once CheckNamespace
namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// Similar to InfoBarButton in Microsoft.VisualStudio.Shell.15.0 but referencing that NuGet for the x86 version
    /// would make the package incompatible with older VS versions.
    /// </summary>
    internal class InfoBarButton : InfoBarActionItem
    {
        #region Constructors

        internal InfoBarButton(string text, Action actionContext)
            : base(text, actionContext)
        {
        }

        #endregion

        #region Properties
        
        public override bool IsButton => true;

        #endregion
    }
} 
