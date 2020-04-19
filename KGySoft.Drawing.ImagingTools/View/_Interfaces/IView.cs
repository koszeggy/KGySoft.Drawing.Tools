#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IView.cs
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
using System.Windows.Forms;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    /// <summary>
    /// Represents a view instance.
    /// A new instance can be created by the <see cref="ViewFactory"/> class.
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Shows the view as a modal dialog.
        /// </summary>
        /// <param name="owner">If not <see langword="null"/>, then the created dialog will belong to the specified owner. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        void ShowDialog(IWin32Window owner = null);

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Shows the view as a non-modal window.
        /// If the view was already shown, then makes it the active window.
        /// </summary>
        void Show();
    }
}