#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TestWindowService.cs
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
using System.Windows.Forms;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.GdiPlus.Test
{
    internal class TestWindowService : IDialogVisualizerService
    {
        #region Methods

        public DialogResult ShowDialog(Form form) => form.ShowDialog();
        public DialogResult ShowDialog(Control control) => throw new NotImplementedException();
        public DialogResult ShowDialog(CommonDialog dialog) => throw new NotImplementedException();

        #endregion
    }
}
