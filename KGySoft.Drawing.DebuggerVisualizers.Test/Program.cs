using System;
using System.Windows.Forms;
using KGySoft.Drawing.DebuggerVisualizers.Test.Forms;

namespace KGySoft.Drawing.DebuggerVisualizers.Test
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DebuggerTestForm());
        }
    }
}
