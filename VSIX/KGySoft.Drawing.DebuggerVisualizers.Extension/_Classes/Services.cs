using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;


namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    /// <summary>
    /// Contains the services used by the package. Mind the initializations from 3 different places!
    /// </summary>
    internal static class Services
    {
        #region Properties

        internal static IServiceProvider ServiceProvider { get; set; } = default!;
        internal static IAsyncServiceProvider? AsyncServiceProvider { get; set; }
        internal static IVsShell? ShellService { get; set; }
        //internal static IMenuCommandService? MenuCommandService { get; set; }
        internal static IVsInfoBarUIFactory? InfoBarUIFactory { get; set; }
        internal static DTE? DTE { get; set; }

        #endregion
    }
}
