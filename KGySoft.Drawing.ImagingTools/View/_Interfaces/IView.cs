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
    internal interface IView : IDisposable
    {
        void ShowDialog(IWin32Window owner = null);
        bool IsDisposed { get; }
        void Show();
    }
}