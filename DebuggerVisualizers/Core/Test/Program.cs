#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Program.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
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

using KGySoft.Drawing.DebuggerVisualizers.Core.Test.View;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Core.Test
{
    static class Program
    {
        #region Methods

        [STAThread]
        static void Main()
        {
            DrawingModule.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DebuggerTestForm());
        }

        #endregion
    }
}
