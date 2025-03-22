#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerVisualizerHelper.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2025 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

#if NET472_OR_GREATER
using System;
#endif
using System.Diagnostics;
#if NET472_OR_GREATER
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
#endif

using KGySoft.Reflection;

using Microsoft.VisualStudio.DebuggerVisualizers;
#if NET472_OR_GREATER
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.DebuggerVisualizers;
using Microsoft.VisualStudio.RpcContracts.RemoteUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test
{
    internal static class DebuggerVisualizerHelper
    {
        #region Fields

#if NET472_OR_GREATER
        private static bool isThreadHelperInitialized;
#endif

        #endregion

        #region Methods

        #region Internal Methods

        internal static bool ShowClassicVisualizer(DebuggerVisualizerAttribute attr, object targetObject, bool isReplaceable, out object? replacementObject)
        {
            var windowService = new TestWindowService();
            var objectProvider = new TestObjectProvider(targetObject) { IsObjectReplaceable = isReplaceable };
            DialogDebuggerVisualizer debugger = (DialogDebuggerVisualizer)Reflector.CreateInstance(Reflector.ResolveType(attr.VisualizerTypeName)!);
            objectProvider.Serializer = (VisualizerObjectSource)Reflector.CreateInstance(Reflector.ResolveType(attr.VisualizerObjectSourceTypeName!)!);
            Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
            replacementObject = objectProvider.ObjectReplaced ? objectProvider.Object : null;
            return objectProvider.ObjectReplaced;
        }

#if NET472_OR_GREATER
        internal static void ShowExtensionVisualizer(IDebuggerVisualizerProvider provider, object targetObject, bool isReplaceable, Action<object?> applyReplacedObject)
        {
            EnsureThreadHelperInitialized();
            Type targetType = targetObject.GetType();
            DebuggerVisualizerProviderConfiguration cfg = provider.DebuggerVisualizerProviderConfiguration;
            VisualizerObjectSource serializer = cfg.VisualizerObjectSourceType is null
                ? new VisualizerObjectSource()
                : (VisualizerObjectSource)Reflector.CreateInstance(Reflector.ResolveType(cfg.VisualizerObjectSourceType.Type)!);

            var testVisualizerTarget = new TestVisualizerTarget(targetObject, serializer, applyReplacedObject);
            Type visualizerTargetImplementationType = Reflector.ResolveType(typeof(VisualizerTarget).Assembly, "Microsoft.VisualStudio.Extensibility.DebuggerVisualizers.VisualizerTargetImplementation")!;
            var visualizerTarget = (VisualizerTarget)Reflector.CreateInstance(visualizerTargetImplementationType, testVisualizerTarget,
                new VisualizerTargetData(targetType.FullName!, targetType.Module.Name, targetType.Assembly.GetName().Version) { IsTargetReplaceable = isReplaceable });

            using IRemoteUserControl visualizer = provider.CreateVisualizerAsync(visualizerTarget, CancellationToken.None).Result;
            if (visualizer is not ILocalControlWrapper localControlWrapper)
                throw new InvalidOperationException("Only local control wrappers are supported by this test.");

            Reflector.InvokeMethod(visualizerTarget, "RaiseStateChangedAsync", VisualizerTargetStateNotification.Available);
            var handle = GCHandle.FromIntPtr((IntPtr)localControlWrapper.GetGCHandleAsync(CancellationToken.None).Result);
            new Window { Title = cfg.Targets.FirstOrDefault(t => Reflector.ResolveType(t.TargetType)?.IsInstanceOfType(targetObject) == true)?.VisualizerDisplayName, Content = handle.Target }.ShowDialog();
        }
#endif

        #endregion

        #region Private Methods

#if NET472_OR_GREATER
        private static void EnsureThreadHelperInitialized()
        {
            if (isThreadHelperInitialized)
                return;

            // This prevents NullReferenceException in ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync
            Reflector.SetField(typeof(ThreadHelper), "_joinableTaskContextCache", new JoinableTaskContext());

            isThreadHelperInitialized = true;
        }
#endif

        #endregion

        #endregion
    }
}
