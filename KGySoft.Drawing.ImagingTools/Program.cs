using System;
using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.Forms;

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Program
    {
        /// <summary>
        /// When executed as a standalone application, this is the entry point.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppMainForm());
        }
    }
}
