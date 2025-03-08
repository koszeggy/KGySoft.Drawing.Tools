//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//using Microsoft.VisualStudio;
//using Microsoft.VisualStudio.Extensibility;
//using Microsoft.VisualStudio.Extensibility.Shell;
//using Microsoft.VisualStudio.Shell;

//namespace KGySoft.Drawing.DebuggerVisualizers.Extension
//{
//    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
//    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
//    [InstalledProductRegistration("My Asynchronous Package", "Loads asynchronously", "1.0")]
//    [Guid("d71fec50-1ce3-40d5-9e4e-3f5d3ed397b0")]
//    [ProvideBindingPath]
//    public class TestPackage : AsyncPackage
//    {
//        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
//        {
//            VisualStudioExtensibility extensibility = await this.GetServiceAsync<VisualStudioExtensibility, VisualStudioExtensibility>();
//            await extensibility.Shell().ShowPromptAsync("Hello from in-proc", PromptOptions.OK, cancellationToken);
//            await base.InitializeAsync(cancellationToken, progress);
//        }
//    }
//}
