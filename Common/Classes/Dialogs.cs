using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KGySoft.DebuggerVisualizers.Common
{
    internal static class Dialogs
    {
        internal static void ErrorMessage(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        internal static void ErrorMessage(string format, params object[] args) => ErrorMessage(String.Format(format, args));

        internal static void InfoMessage(string message) => MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
