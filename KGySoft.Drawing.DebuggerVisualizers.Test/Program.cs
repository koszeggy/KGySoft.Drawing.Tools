﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Program.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using KGySoft.Drawing.DebuggerVisualizers.Test.View;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test
{
    static class Program
    {
        #region Methods

        [STAThread]
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "Would just cause double disposing because closing will dispose the form anyway.")]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DebuggerTestForm());
        }

        #endregion
    }
}
