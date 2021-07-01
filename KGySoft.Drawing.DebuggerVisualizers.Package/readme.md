## KGy SOFT Drawing Debugger Visualizers for Visual Studio 2008-2019

This package provides debugger visualizers for several `System.Drawing` types such as `Image`, `Bitmap`, `Metafile`, `Icon`, `BitmapData`, `Graphics`, `ColorPalette`, `Color`. It supports multi-page, multi-resolution and animated images as well as saving them in various formats. 

When a type is debugged in Visual Studio and there is a debugger visualizer installed for that type, then a magnifier icon appears that you can click to open the visualizer.

![Debugger Visualizer Usage](https://kgysoft.net/images/DebuggerVisualizerUsage.png)

Either click the magnifier icon or choose a debugger visualizer from the drop down list (if more visualizers are applicable).

![Debugging Graphics](https://kgysoft.net/images/DebugGraphics.png)

If an image or icon instance is debugged in a non read-only context, then it can be modified, replaced or cleared.

![Changing pixel format with quantizing and dithering](https://kgysoft.net/images/Quantizing.png)

Several modifications are allowed on non-read-only images such as rotating, resizing, changing pixel format with quantizing and dithering, adjusting brightness, contrast and gamma, or even editing the palette entries of indexed bitmaps.

![Debugging Palette](https://kgysoft.net/images/DebugPalette.png)

## Installing Debugger Visualizers

* For Visual Studio 2013 and above you can use this VSIX package (tested with versions up to Visual Studio 2019). It will install the .NET 4.5 version.
* For older Visual Studio versions and/or frameworks follow the [installation steps](https://github.com/koszeggy/KGySoft.Drawing.Tools#installing-debugger-visualizers) at the project site.

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Drawing.Tools/blob/master/changelog.txt).

## FAQ

**Q:** Can I use the debugger visualizers for other Visual Studio versions?
<br/>**A:** The VSIX installer supports Visual Studio 2013 and newer versions (tested until 2019). However, you can install the debugger visualizers manually for any version starting with Visual Studio 2008. See the [installation steps](https://github.com/koszeggy/KGySoft.Drawing.Tools#installing-debugger-visualizers) at the project site.

**Q:** Is Visual Studio Code supported?
<br/>**A:** As it has a completely different API, it is not supported yet.

**Q:** I get an error message when I click the magnifier icon.
<br/>**A:** It can have several reasons. See the [Troubleshooting](https://github.com/koszeggy/KGySoft.Drawing.Tools#troubleshooting) section at the project site.

**Q:** Are WPF image types supported?
<br/>**A:** No, these visualizers are for `System.Drawing` types. But the built-in Dependency Object visualizer is able to display image sources anyway.

**Q:** Where do I find the edited/downloaded resource files? Even my previously edited/downloaded resources have been disappeared.
<br/>**A:** The __Visual Studio/Tools/KGy SOFT Drawing Debugger Visualizers__ and clicking the magnifier icon executes the Imaging Tools from different locations. If you edit the language resources at one place they will not be automatically applied at the other place. Therefore, the saved resources might be at different possible locations:
* If you execute a manually deployed version the resources will be in a `Resources` subfolder in the folder you executed the ImagingTools from
* During debugging the tool is executed from the debugger visualizers folder: `Documents\Visual Studio <version>\Visualizers`
* If you launch the tool from the Visual Studio Tools menu, then it is located under `ProgramData\Microsoft\VisualStudio\Packages\...`

**Q:** I have removed the debugger visualizer extension, and it is still working. How can I remove it completely?
<br/>**A:** When the extension is active it copies the visualizers into the `Documents\Visual Studio <version>\Visualizers` folder if it is not there. Unlike an MSI installer the VSIX packages do not support uninstall actions so this copied content will not be removed automatically. However, the extension creates also a _KGy SOFT  Drawing Debugger Visualizers/Manage Installations..._ menu item under the Tools menu where you can remove the installation from the Documents folder. So the proper way of a complete uninstall:
1. Click _Tools/Drawing Debugger Visualizers/Manage Installations..._
2. Select the current Visual Studio version and click "Remove"
3. Now uninstall the extension from the _Tools/Extensions and Updates..._ (2019: _Extensions/Manage Extensions_) menu. Without this last step the debugger visualizers will be automatically reinstalled when you restart Visual Studio.

## More Info:

Feel free to visit the [project site](https://github.com/koszeggy/KGySoft.Drawing.Tools) for more information. There you will find the binaries for other frameworks and Visual Studio versions as well as the complete source code.