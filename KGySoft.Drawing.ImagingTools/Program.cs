#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Program.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
#if NET471_OR_GREATER || NETCOREAPP
using System.Runtime.InteropServices;
#endif
using System.Runtime.Versioning;
using System.Windows.Forms;

using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;
using KGySoft.Reflection;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// When executed as a standalone application, this is the entry point.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ThemeColors.SetBaseTheme(DefaultTheme.System);
            DumpDebugInfo();

            using IViewModel viewModel = ViewModelFactory.FromCommandLineArguments(args);
            using IView view = ViewFactory.CreateView(viewModel);
            Application.Run(ViewFactory.TryGetForm(view)!);
        }

        [Conditional("DEBUG")]
        private static void DumpDebugInfo()
        {
#if NET35
            const string frameworkName = ".NET Framework 3.5";
#else
            TargetFrameworkAttribute attr = (TargetFrameworkAttribute)Attribute.GetCustomAttribute(typeof(Program).Assembly, typeof(TargetFrameworkAttribute))!;
            string frameworkName = attr.FrameworkDisplayName is { Length: > 0 } name ? name : attr.FrameworkName;
#endif
            Console.WriteLine(frameworkName);
            Console.WriteLine($"IsWindows: {OSHelper.IsWindows} {OSHelper.GetWindowsVersion()}");
            Console.WriteLine($"IsMono: {OSHelper.IsMono} {(OSHelper.IsMono ? Reflector.InvokeMethod(Type.GetType("Mono.Runtime")!, "GetDisplayName") : null)}");
            Console.WriteLine($"IsWine: {OSHelper.IsWine} {Environment.GetEnvironmentVariable("WINEPREFIX")}");
            Console.WriteLine($"Environment.OSVersion.Platform: {Environment.OSVersion.Platform}");
#if NET471_OR_GREATER || NETCOREAPP
            Console.WriteLine($"RuntimeInformation.FrameworkDescription: {RuntimeInformation.FrameworkDescription}");
#endif  
            Console.WriteLine($"System scale: {ScaleHelper.SystemScale}");
            Console.WriteLine($"Per-monitor DPI awareness version: {ScaleHelper.PerMonitorDpiAwarenessVersion}");
        }

        #endregion
    }
}
