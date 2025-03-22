#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DebuggerHelper.cs
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;

using KGySoft.Drawing.ImagingTools.Model;
using KGySoft.Drawing.ImagingTools.View;
using KGySoft.Drawing.ImagingTools.ViewModel;

#if NET472_OR_GREATER
using Microsoft.VisualStudio.Extensibility.DebuggerVisualizers;
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers
{
    /// <summary>
    /// Provides debugger methods for debugger visualizers
    /// </summary>
    public static class DebuggerHelper
    {
        #region Methods

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="image"/>.
        /// </summary>
        /// <param name="image">The image to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>An <see cref="Image"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="image"/>.</returns>
        public static Image? DebugImage(Image? image, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using IViewModel<Image?> vm = ViewModelFactory.FromImage(image, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            return vm.IsModified ? vm.GetEditedModel() : image;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="bitmap"/>.
        /// </summary>
        /// <param name="bitmap">The bitmap to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A <see cref="Bitmap"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="bitmap"/>.</returns>
        public static Bitmap? DebugBitmap(Bitmap? bitmap, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using IViewModel<Bitmap?> vm = ViewModelFactory.FromBitmap(bitmap, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            return vm.IsModified ? vm.GetEditedModel() : bitmap;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="metafile"/>.
        /// </summary>
        /// <param name="metafile">The metafile to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A <see cref="Metafile"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="metafile"/>.</returns>
        public static Metafile? DebugMetafile(Metafile? metafile, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using IViewModel<Metafile?> vm = ViewModelFactory.FromMetafile(metafile, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            return vm.IsModified ? vm.GetEditedModel() : metafile;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="icon"/>.
        /// </summary>
        /// <param name="icon">The icon to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>An <see cref="Icon"/> that is returned by the debugger. If <paramref name="isReplaceable"/> is <see langword="false"/>, then this will be always the original <paramref name="icon"/>.</returns>
        public static Icon? DebugIcon(Icon? icon, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using IViewModel<Icon?> vm = ViewModelFactory.FromIcon(icon, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            return vm.IsModified ? vm.GetEditedModel() : icon;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="bitmapData"/>.
        /// </summary>
        /// <param name="bitmapData">The bitmap data to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugBitmapData(BitmapData bitmapData, IntPtr ownerWindowHandle = default)
        {
            if (bitmapData == null)
                throw new ArgumentNullException(nameof(bitmapData), PublicResources.ArgumentNull);
            using IViewModel vm = ViewModelFactory.FromBitmapData(bitmapData);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="customBitmapInfo"/>.
        /// </summary>
        /// <param name="customBitmapInfo">The custom bitmap info debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugCustomBitmap(CustomBitmapInfo customBitmapInfo, IntPtr ownerWindowHandle = default)
        {
            if (customBitmapInfo == null)
                throw new ArgumentNullException(nameof(customBitmapInfo), PublicResources.ArgumentNull);
            using IViewModel vm = ViewModelFactory.FromCustomBitmap(customBitmapInfo);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="graphics"/>.
        /// </summary>
        /// <param name="graphics">The <see cref="Graphics"/> instance to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugGraphics(Graphics graphics, IntPtr ownerWindowHandle = default)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics), PublicResources.ArgumentNull);
            using IViewModel vm = ViewModelFactory.FromGraphics(graphics);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="ColorPalette"/> instance.
        /// </summary>
        /// <param name="palette">The palette object to debug.</param>
        /// <param name="isReplaceable">Indicates whether the palette is replaceable.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the palette has been edited; otherwise, <see langword="null"/>.</returns>
        public static ColorPalette? DebugPalette(ColorPalette palette, bool isReplaceable, IntPtr ownerWindowHandle = default)
        {
            if (palette == null)
                throw new ArgumentNullException(nameof(palette), PublicResources.ArgumentNull);
            using IViewModel<ColorPalette> vm = ViewModelFactory.FromPalette(palette, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            if (isReplaceable && vm.IsModified)
                return vm.GetEditedModel();

            return null;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="customPalette"/>.
        /// </summary>
        /// <param name="customPalette">The custom palette info to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugCustomPalette(CustomPaletteInfo customPalette, IntPtr ownerWindowHandle = default)
        {
            if (customPalette == null)
                throw new ArgumentNullException(nameof(customPalette), PublicResources.ArgumentNull);
            using IViewModel vm = ViewModelFactory.FromCustomPalette(customPalette);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Shows the debugger for a <see cref="Color"/> instance.
        /// </summary>
        /// <param name="color">The color object to debug.</param>
        /// <param name="isReplaceable"><see langword="true"/>, if the debugged instance can be replaced or edited; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        /// <returns>A non-<see langword="null"/>&#160;instance, when the color has been edited and should be serialized back; otherwise, <see langword="null"/>.</returns>
        public static Color? DebugColor(Color color, bool isReplaceable = true, IntPtr ownerWindowHandle = default)
        {
            using IViewModel<Color> vm = ViewModelFactory.FromColor(color, !isReplaceable);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
            if (isReplaceable && vm.IsModified)
                return vm.GetEditedModel();

            return null;
        }

        /// <summary>
        /// Shows a debugger dialog for the specified <paramref name="customColor"/>.
        /// </summary>
        /// <param name="customColor">The custom color info to debug.</param>
        /// <param name="ownerWindowHandle">If specified, then the created dialog will be owned by the window that has specified handle. This parameter is optional.
        /// <br/>Default value: <see cref="IntPtr.Zero">IntPtr.Zero</see>.</param>
        public static void DebugCustomColor(CustomColorInfo customColor, IntPtr ownerWindowHandle = default)
        {
            if (customColor == null)
                throw new ArgumentNullException(nameof(customColor), PublicResources.ArgumentNull);
            using IViewModel vm = ViewModelFactory.FromCustomColor(customColor);
            ViewFactory.ShowDialog(vm, ownerWindowHandle);
        }

        /// <summary>
        /// Gets the debugger visualizers of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to get the debugger visualizers for.</param>
        /// <returns>The debugger visualizers of the specified assembly.</returns>
        public static Dictionary<Type, DebuggerVisualizerAttribute> GetDebuggerVisualizers(Assembly assembly)
            => Attribute.GetCustomAttributes(assembly ?? throw new ArgumentNullException(nameof(assembly), PublicResources.ArgumentNull), typeof(DebuggerVisualizerAttribute))
                .Cast<DebuggerVisualizerAttribute>().ToDictionary(a => a.Target!);

#if NET472_OR_GREATER
        /// <summary>
        /// Gets the debugger visualizer providers of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to get the debugger visualizer providers for.</param>
        /// <returns>The debugger visualizer providers of the specified assembly.</returns>
        public static Dictionary<Type, IDebuggerVisualizerProvider> GetDebuggerVisualizerProviders(Assembly assembly)
        {
            var result = new Dictionary<Type, IDebuggerVisualizerProvider>();
            Type?[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            foreach (Type? type in types)
            {
                if (type is null || type.IsInterface || type.IsAbstract || !typeof(IDebuggerVisualizerProvider).IsAssignableFrom(type))
                    continue;

                var provider = (IDebuggerVisualizerProvider)Activator.CreateInstance(type);
                DebuggerVisualizerProviderConfiguration cfg = provider.DebuggerVisualizerProviderConfiguration;
                foreach (VisualizerTargetType target in cfg.Targets)
                    result[Type.GetType(target.TargetType)] = provider;
            }

            return result;
        }
#endif

        #endregion
    }
}
