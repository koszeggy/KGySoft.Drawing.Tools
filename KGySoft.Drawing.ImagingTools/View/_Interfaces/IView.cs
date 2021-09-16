#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: IView.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

#endregion

namespace KGySoft.Drawing.ImagingTools.View
{
    /// <summary>
    /// Represents a view instance.
    /// A new instance can be created by the <see cref="ViewFactory"/> class.
    /// </summary>
    public interface IView : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets whether this view is disposed.
        /// </summary>
        public bool IsDisposed { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the view as a modal dialog.
        /// When using this overload, do not let the handle of the owner destroyed (some operations such as changing right-to-left may cause the handle recreated).
        /// </summary>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        void ShowDialog(IntPtr ownerWindowHandle = default);

        /// <summary>
        /// Shows the view as a modal dialog.
        /// </summary>
        /// <param name="owner">If not <see langword="null"/>, then the created dialog will be owned by the specified <see cref="IView"/> instance.</param>
        void ShowDialog(IView? owner);

        /// <summary>
        /// Shows the view as a non-modal window.
        /// If the view was already shown, then makes it the active window.
        /// </summary>
        void Show();

        #endregion
    }
}