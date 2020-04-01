﻿#region Copyright

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

namespace KGySoft.Drawing.ImagingTools.View
{
    public static class Dialogs
    {
        #region Methods

        public static void ErrorMessage(string message) => MessageBox.Show(message, Res.TitleError, MessageBoxButtons.OK, MessageBoxIcon.Error);
        public static void InfoMessage(string message) => MessageBox.Show(message, Res.TitleInformation, MessageBoxButtons.OK, MessageBoxIcon.Information);
        public static void WarningMessage(string message) => MessageBox.Show(message, Res.TitleWarning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        public static bool ConfirmMessage(string message) => MessageBox.Show(message, Res.TitleConfirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        #endregion
    }
}
