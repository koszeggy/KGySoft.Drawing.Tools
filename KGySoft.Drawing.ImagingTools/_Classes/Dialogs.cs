#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Dialogs.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Dialogs
    {
        #region Methods

        internal static void ErrorMessage(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        internal static void ErrorMessage(string format, params object[] args) => ErrorMessage(String.Format(format, args));
        internal static void InfoMessage(string message) => MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        internal static bool ConfirmMessage(string message) => MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        #endregion
    }
}
