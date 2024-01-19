#if !VS2022_OR_GREATER
#region Usings

using System;

using Microsoft.VisualStudio.Imaging.Interop;

#endregion

namespace Microsoft.VisualStudio.Imaging
{
    /// <summary>
    /// Normally this class is available from the Microsoft.VisualStudio.ImageCatalog package.
    /// The problem is that back then the ImageMoniker struct was also in this package, but then it has been
    /// moved to Microsoft.VisualStudio.Imaging.Interop.14.0.DesignTime and this causes a conflict if we
    /// reference and old version of it for the x86 package to be compatible with VS2013.
    /// </summary>
    internal static class KnownMonikers
    {
        #region Constants

        private const int statusError = 2926;
        private const int statusWarning = 2956;
        private const int statusInformation = 2933;

        #endregion

        #region Fields

        private static readonly Guid imageCatalogGuid = new Guid("{ae27a6b0-e345-4288-96df-5eaf394ee369}");

        #endregion

        #region Properties

        internal static ImageMoniker StatusError => new ImageMoniker { Guid = imageCatalogGuid, Id = statusError };
        internal static ImageMoniker StatusWarning => new ImageMoniker { Guid = imageCatalogGuid, Id = statusWarning };
        internal static ImageMoniker StatusInformation => new ImageMoniker { Guid = imageCatalogGuid, Id = statusInformation };

        #endregion
    }
}
#endif