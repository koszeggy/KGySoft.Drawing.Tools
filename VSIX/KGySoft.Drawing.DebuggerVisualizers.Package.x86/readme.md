## KGy SOFT Image Debugger Visualizers

**VS 2022**: See the [64-bit installer](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers-x64) (for version 17.9 Preview 1 or higher only; offers modern, non-dialog embeddable visualizers)

**VS 2013-2019**: See the [32-bit installer](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers) (classic visualizers)

**VS 2008-2022**: See [manual installation](https://github.com/koszeggy/KGySoft.Drawing.Tools#installing-debugger-visualizers) (classic visualizers)

This package provides debugger visualizers for several  GDI+, WPF, SkiaSharp and KGy SOFT image types like `Bitmap`, `BitmapSource`, `SKBitmap`, `Metafile`, `ImageSource`, `SKImage`, `Icon`, `Graphics`, `SKSurface`, `BitmapData`, `SKPixmap`, `ColorPalette`, `BitmapPalette` and more. Possible derived types such as `DrawingImage`, `BitmapFrame`, `WriteableBitmap`, etc. are also supported). For GDI+ types it also supports multi-page, multi-resolution and animated images as well as saving them in various formats.

When a type is debugged in Visual Studio and there is a debugger visualizer installed for that type, then a magnifier icon appears that you can click to open the visualizer.

![Debugger Visualizer Usage](https://user-images.githubusercontent.com/27336165/124266849-c7ff1780-db37-11eb-9df8-f2149430da16.png)

Either click the magnifier icon or choose a debugger visualizer from the drop-down list (if more visualizers are applicable).

![Debugging Graphics](https://user-images.githubusercontent.com/27336165/124266974-f54bc580-db37-11eb-98e1-207c48590afa.png)

When debugging WPF images from the latest Visual Studio versions you need to change the default WPF Tree Visualizer to KGy SOFT ImageSource Debugger Visualizer from the drop-down menu:

![Debugger Visualizer Usage (WPF)](https://kgysoft.net/images/DebuggerVisualizerWpfUsage.png)

If an image or icon instance is debugged in a non read-only context, then it can be modified, replaced or cleared (supported for GDI+ types only).

![Changing pixel format with quantizing and dithering](https://kgysoft.net/images/Quantizing.png)

Several modifications are allowed on non-read-only images such as rotating, resizing, changing pixel format with quantizing and dithering, adjusting brightness, contrast and gamma, or even editing the palette entries of indexed bitmaps (supported for GDI+ types only, or when you execute the KGy SOFT Imaging Tools application from the Visual Studio Tools menu).

![Debugging Palette](https://kgysoft.net/images/DebugPalette.png)

## Installing Debugger Visualizers

* For Visual Studio 2013 and above you can use this VSIX package (the [32-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers) for VS2013-2019 or the [64-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers-x64) for VS2022). It will install the .NET Framework 4.6.2 build, which works also for .NET Core projects.
* For older Visual Studio versions and/or frameworks follow the [installation steps](https://github.com/koszeggy/KGySoft.Drawing.Tools#installing-debugger-visualizers) at the project site.

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt).

## FAQ

**Q:** Can I use the debugger visualizers for other Visual Studio versions?
<br/>**A:** The VSIX installer has two versions. You can use the [32-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers) for VS2013-2019 and the [64-bit version](https://marketplace.visualstudio.com/items?itemName=KGySoft.drawing-debugger-visualizers-x64) for VS2022. However, you can install the debugger visualizers manually for any version starting with Visual Studio 2008. See the [installation steps](https://github.com/koszeggy/KGySoft.Drawing.Tools#installing-debugger-visualizers) at the project site.

**Q:** Is Visual Studio Code supported?
<br/>**A:** As it has a completely different API, it is not supported yet.

**Q:** I get an error message when I click the magnifier icon.
<br/>**A:** It can have several reasons. See the [Troubleshooting](https://github.com/koszeggy/KGySoft.Drawing.Tools#troubleshooting) section at the project site.

**Q:** Wouldn't it be possible to leave the debugger visualizer open while stepping through the code?
<br/>**A:** This is supported in the 64-bit version of the debugger visualizers, if you use Visual Studio 17.9 Preview 1 or later. When using older versions this is not possible due to the limitations of the classic Visual Studio debugger API.

**Q:** Are WPF image types supported?
<br/>**A:** Yes, starting with version 3.0.0 WPF `ImageSource` (and derived types), `BitmapPalette` and `Color` types are supported as well. You might need to explicitly select the correct visualizer from the drop-down menu next to the magnifier icon.

**Q:** Are other 3rd party image types supported?
<br/>**A:** Yes, starting with version 3.1.0 SkiaSharp types `SKBitmap`, `SKImage`, `SKPixmap`, `SKSurface` and `SKColor` are also supported. The infrastructure is extensible so further frameworks can be expected in future versions.

**Q:** Where do I find the edited/downloaded resource files?
<br/>**A:** From the Language Options dialog click the _Edit Resources..._ button. Starting with version 5.0.0 the Edit Resources window has an _Open in containing folder_ button that navigates to the folder where the edited resource files are stored, selecting currently edited file.

**Q:** I have removed the debugger visualizer extension, and it is still working. How can I remove it completely?
<br/>**A:** When the extension is active it copies the visualizers into the `Documents\Visual Studio <version>\Visualizers` folder if it is not there. Unlike an MSI installer the VSIX packages do not support uninstall actions so this copied content will not be removed automatically. However, the extension creates also a _KGy SOFT Image Debugger Visualizers/Manage Installations..._ menu item under the Tools menu where you can remove the installation from the Documents folder. So the proper way of a complete uninstall:
1. Click _Tools/Image Debugger Visualizers/Manage Installations..._
2. Select the current Visual Studio version and click "Remove"
3. Now uninstall the extension from the _Tools/Extensions and Updates..._ (2019 and above: _Extensions/Manage Extensions_) menu. Without this last step the debugger visualizers will be automatically reinstalled when you restart Visual Studio.

## More Info:

Feel free to visit the [project site](https://github.com/koszeggy/KGySoft.Drawing.Tools) for more information. There you will find the binaries for other frameworks and Visual Studio versions as well as the complete source code.