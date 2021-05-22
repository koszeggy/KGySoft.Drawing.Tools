﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Res.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2020 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

using KGySoft.Collections;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.ImagingTools.Properties;
using KGySoft.Reflection;
using KGySoft.Resources;

#endregion

namespace KGySoft.Drawing.ImagingTools
{
    internal static class Res
    {
        #region Constants

        private const string unavailableResource = "Resource ID not found: {0}";
        private const string invalidResource = "Resource text is not valid for {0} arguments: {1}";

        #endregion

        #region Fields

        private static readonly DynamicResourceManager resourceManager = new DynamicResourceManager("KGySoft.Drawing.ImagingTools.Messages", typeof(Res).Assembly)
        {
            SafeMode = true,
            UseLanguageSettings = true,
        };

        // Note: No need to use ThreadSafeCacheFactory here because used only from the UI thread when applying resources
        // ReSharper disable once CollectionNeverUpdated.Local
        private static readonly Cache<Type, PropertyInfo[]?> localizableStringPropertiesCache = new Cache<Type, PropertyInfo[]?>(GetLocalizableStringProperties);

        #endregion

        #region Properties

        #region General

        internal static CultureInfo OSLanguage { get; }
        internal static CultureInfo DefaultLanguage { get; }
        internal static string ResourcesDir => resourceManager.ResXResourcesDir;

        #endregion

        #region Title Captions

        /// <summary>No Image</summary>
        internal static string TitleNoImage => Get("Title_NoImage");

        /// <summary>Error</summary>
        internal static string TitleError => Get("Title_Error");

        /// <summary>Information</summary>
        internal static string TitleInformation => Get("Title_Information");

        /// <summary>Warning</summary>
        internal static string TitleWarning => Get("Title_Warning");

        /// <summary>Confirmation</summary>
        internal static string TitleConfirmation => Get("Title_Confirmation");

        #endregion

        #region Texts

        /// <summary>; </summary>
        internal static string TextSeparator => Get("Text_Separator");

        /// <summary>files</summary>
        internal static string TextFiles => Get("Text_Files");

        /// <summary>All files</summary>
        internal static string TextAllFiles => Get("Text_AllFiles");

        /// <summary>Metafiles</summary>
        internal static string TextMetafiles => Get("Text_Metafiles");

        /// <summary>Icon</summary>
        internal static string TextIcon => Get("Text_Icon");

        /// <summary>Raw Bitmap Data</summary>
        internal static string TextRaw => Get("Text_Raw");

        /// <summary>Images</summary>
        internal static string TextImages => Get("Text_Images");

        /// <summary>File Format</summary>
        internal static string TextFileFormat => Get("Text_FileFormat");

        /// <summary>Unnamed</summary>
        internal static string TextUnnamed => Get("Text_Unnamed");

        /// <summary>Toggles whether the animation is handled as a single image.
        /// • When checked, animation will play and saving as GIF saves the whole animation.
        /// • When not checked, frame navigation will be enabled and saving saves only the selected frame.</summary>
        internal static string TooltipTextCompoundAnimation => Get("TooltipText_CompoundAnimation");

        /// <summary>Toggles whether the icon is handled as a multi-resolution image.
        /// • When checked, always the best fitting image is displayed and saving as Icon saves every image.
        /// • When not checked, icon images can be explored by navigation and saving saves the selected image only.</summary>
        internal static string TooltipTextCompoundMultiSize => Get("TooltipText_CompoundMultiSize");

        /// <summary>Toggles whether the pages are handled as a compound image.
        /// • When checked, saving as TIFF saves every page.
        /// • When not checked, saving saves always the selected page only.</summary>
        internal static string TooltipTextCompoundMultiPage => Get("TooltipText_CompoundMultiPage");

        /// <summary>Smoothing Edges (Alt+S)</summary>
        internal static string TooltipTextSmoothMetafile => Get("TooltipText_SmoothMetafile");

        /// <summary>Smooth Zooming (Alt+S)</summary>
        internal static string TooltipTextSmoothBitmap => Get("TooltipText_SmoothBitmap");

        /// <summary>Auto</summary>
        internal static string TextAuto => Get("Text_Auto");

        /// <summary>Counting colors...</summary>
        internal static string TextCountingColorsId => "Text_CountingColors";

        /// <summary>Operation has been canceled.</summary>
        internal static string TextOperationCanceledId => "Text_OperationCanceled";

        /// <summary>Color Count: {0}</summary>
        internal static string TextColorCountId => "Text_ColorCountFormat";

        #endregion

        #region Info Texts

        /// <summary>World transformation: </summary>
        internal static string InfoWorldTransformation => Get("InfoText_WorldTransformation");

        /// <summary>None (Identity Matrix)</summary>
        internal static string InfoNoTransformation => Get("InfoText_NoTransformation");

        #endregion

        #region Notifications

        /// <summary>The loaded metafile has been converted to Bitmap. To load it as a Metafile, choose the Image Debugger Visualizer instead.</summary>
        internal static string NotificationMetafileAsBitmapId => "Notification_MetafileAsBitmap";

        /// <summary>The loaded image has been converted to Icon</summary>
        internal static string NotificationImageAsIconId => "Notification_ImageAsIcon";

        /// <summary>The palette of an indexed BitmapData cannot be reconstructed, therefore a default palette is used. You can change palette colors in the menu.</summary>
        internal static string NotificationPaletteCannotBeRestoredId => "Notification_PaletteCannotBeRestored";

        #endregion

        #region Messages

        /// <summary>Error: {0}</summary>
        internal static string ErrorMessageId => "ErrorMessageFormat";

        /// <summary>Saving modifications as animated GIF is not supported</summary>
        internal static string ErrorMessageAnimGifNotSupported => Get("ErrorMessage_AnimGifNotSupported");

        /// <summary>The current installation is being executed, which cannot be overwritten</summary>
        internal static string ErrorMessageInstallationCannotBeOverwritten => Get("ErrorMessage_InstallationCannotBeOverwritten");

        /// <summary>The current installation is being executed, which cannot be removed</summary>
        internal static string ErrorMessageInstallationCannotBeRemoved => Get("ErrorMessage_InstallationCannotBeRemoved");

        /// <summary>The selected quantizer supports partial transparency, which is not supported by ditherers,
        /// so partial transparent pixels will be blended with back color.</summary>
        internal static string WarningMessageDithererNoAlphaGradient => Get("WarningMessage_DithererNoAlphaGradient");

        /// <summary>Are you sure you want to overwrite this installation?</summary>
        internal static string ConfirmMessageOverwriteInstallation => Get("ConfirmMessage_OverwriteInstallation");

        /// <summary>Are you sure you want to remove this installation?</summary>
        internal static string ConfirmMessageRemoveInstallation => Get("ConfirmMessage_RemoveInstallation");

        /// <summary>You are about to install the .NET Core version, which might not be supported by Visual Studio as a debugger visualizer. Are you sure?</summary>
        internal static string ConfirmMessageNetCoreVersion => Get("ConfirmMessage_NetCoreVersion");

        /// <summary>There are unsaved modifications. Are sure to discard the changes?</summary>
        internal static string ConfirmMessageDiscardChanges => Get("ConfirmMessage_DiscardChanges");

        /// <summary>The palette contains no colors. Click OK to exit.</summary>
        internal static string InfoMessagePaletteEmpty => Get("InfoMessage_PaletteEmpty");

        /// <summary>The selected quantizer uses more colors than the original image.
        /// It is possible that is has no effect.</summary>
        internal static string InfoMessageQuantizerMayHaveNoEffect => Get("InfoMessage_QuantizerMayHaveNoEffect");

        /// <summary>The selected quantizer can only remove partial transparency but it has practically no effect as source has no alpha.</summary>
        internal static string InfoMessageArgbQuantizerHasNoEffect => Get("InfoMessage_ArgbQuantizerHasNoEffect");

        /// <summary>The selected pixel format is the same as the original one.</summary>
        internal static string InfoMessageSamePixelFormat => Get("InfoMessage_SamePixelFormat");

        /// <summary>Without selecting a quantizer possible alpha pixels of the source image are blended with black.
        /// By selecting a quantizer you can specify a different back color.</summary>
        internal static string InfoMessageAlphaTurnsBlack => Get("InfoMessage_AlphaTurnsBlack");

        #endregion

        #region Installations

        /// <summary>N/A (KGySoft.Drawing.DebuggerVisualizers.dll is missing)</summary>
        internal static string InstallationNotAvailable => Get("Installations_NotAvailable");

        /// <summary>&lt;Custom Path...&gt;</summary>
        internal static string InstallationsCustomDir => Get("Installations_CustomDir");

        /// <summary>Not Installed</summary>
        internal static string InstallationsStatusNotInstalled => Get("Installations_StatusNotInstalled");

        /// <summary>Unknown version (incompatible runtime?)</summary>
        internal static string InstallationsStatusUnknown => Get("Installations_StatusUnknown");

        #endregion

        #endregion

        #region Constructors

        static Res()
        {
            OSLanguage = LanguageSettings.DisplayLanguage.GetClosestNeutralCulture();
            DefaultLanguage = (Attribute.GetCustomAttribute(typeof(Res).Assembly, typeof(NeutralResourcesLanguageAttribute)) is NeutralResourcesLanguageAttribute attr
                ? CultureInfo.GetCultureInfo(attr.CultureName)
                : CultureInfo.InvariantCulture).GetClosestNeutralCulture();
            DrawingModule.Initialize();

            bool allowResXResources = Settings.Default.AllowResXResources;
            LanguageSettings.DisplayLanguage = allowResXResources
                ? Settings.Default.UseOSLanguage ? OSLanguage : Settings.Default.DisplayLanguage.GetClosestNeutralCulture()
                : DefaultLanguage;
            LanguageSettings.DynamicResourceManagersSource = allowResXResources ? ResourceManagerSources.CompiledAndResX : ResourceManagerSources.CompiledOnly;
        }

        #endregion

        #region Methods

        #region Internal Methods

        #region General

        /// <summary>
        /// Just an empty method to be able to trigger the static constructor without running any code other than field initializations.
        /// </summary>
        internal static void EnsureInitialized()
        {
        }

        internal static string? GetStringOrNull(string id) => resourceManager.GetString(id, LanguageSettings.DisplayLanguage);

        internal static string Get(string id) => GetStringOrNull(id) ?? String.Format(CultureInfo.InvariantCulture, unavailableResource, id);

        internal static string Get(string id, params object?[]? args)
        {
            string format = Get(id);
            return args == null ? format : SafeFormat(format, args);
        }

        internal static string Get<TEnum>(TEnum value) where TEnum : struct, Enum => Get($"{value.GetType().Name}.{Enum<TEnum>.ToString(value)}");

        internal static void ApplyStringResources(object target, string name)
        {
            // Unlike ComponentResourceManager we don't go by ResourceSet because that would kill resource fallback traversal
            // so we go by localizable properties
            PropertyInfo[]? properties = localizableStringPropertiesCache[target.GetType()];
            if (properties == null)
                return;

            foreach (PropertyInfo property in properties)
            {
                string? value = GetStringOrNull(name + "." + property.Name);
                if (value == null)
                    continue;
                Reflector.SetProperty(target, property, value);
            }
        }

        /// <summary>Internal Error: {0}</summary>
        internal static string InternalError(string msg) => Get("General_InternalErrorFormat", msg);

        #endregion

        #region Title Captions

        /// <summary>KGy SOFT Imaging Tools v{0}</summary>
        internal static string TitleAppNameAndVersion(Version version) => Get("Title_AppNameAndVersionFormat", version);

        /// <summary>Type: {0}</summary>
        internal static string TitleType(string type) => Get("Title_TypeFormat", type);

        /// <summary>Size: {0}x{1}</summary>
        internal static string TitleSize(Size size) => Get("Title_SizeFormat", size.Width, size.Height);

        /// <summary>Palette Count: {0}</summary>
        internal static string TitlePaletteCount(int count) => Get("Title_PaletteCountFormat", count);

        /// <summary>Visible Clip Bounds: {{X = {0}, Y = {1}, Size = {2}x{3}}}</summary>
        internal static string TitleVisibleClip(Rectangle rect) => Get("Title_VisibleClipFormat", rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>Original Visible Clip Bounds: {{X = {0}, Y = {1}, Size = {2}x{3}}}</summary>
        internal static string TitleOriginalVisibleClip(Rectangle rect) => Get("Title_OriginalVisibleClipFormat", rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>Color: {0}</summary>
        internal static string TitleColor(Color color) => Get("Title_ColorFormat", color.Name);

        /// <summary>Edit Resources - {0}</summary>
        internal static string TitleEditResources(string langName) => Get("Title_EditResourcesFormat", langName);

        #endregion

        #region Texts

        /// <summary>A: {0}</summary>
        internal static string TextAlphaValue(byte a) => Get("Text_AlphaValueFormat", a);

        /// <summary>R: {0}</summary>
        internal static string TextRedValue(byte r) => Get("Text_RedValueFormat", r);

        /// <summary>G: {0}</summary>
        internal static string TextGreenValue(byte g) => Get("Text_GreenValueFormat", g);

        /// <summary>B: {0}</summary>
        internal static string TextBlueValue(byte a) => Get("Text_BlueValueFormat", a);

        #endregion

        #region Info Texts

        /// <summary>Type: {0}
        /// Size: {1}x{2} pixels
        /// {7}Pixel Format: {3}
        /// Raw format: {4}
        /// Resolution: {5}x{6} DPI</summary>
        internal static string InfoImage(string type, Size size, PixelFormat pixelFormat, string rawFormat, float hres, float vres, string frameInfo)
            => Get("InfoText_ImageFormat", type, size.Width, size.Height, pixelFormat, rawFormat, hres, vres, frameInfo);

        /// <summary>Size: {0}x{1} pixels
        /// Stride: {2} bytes
        /// Pixel Format: {3}</summary>
        internal static string InfoBitmapData(Size size, int stride, PixelFormat pixelFormat)
            => Get("InfoText_BitmapDataFormat", size.Width, size.Height, stride, pixelFormat);

        /// <summary>Unknown format: {0}</summary>
        internal static string InfoUnknownFormat(Guid format) => Get("InfoText_UnknownFormat", format);

        /// <summary>Palette count: {0}</summary>
        internal static string InfoPalette(int count) => Get("InfoText_PaletteFormat", count);

        /// <summary>Images: {0}</summary>
        internal static string InfoFramesCount(int count) => Get("InfoText_FramesCountFormat", count);

        /// <summary>Current Image: {0}/{1}</summary>
        internal static string InfoCurrentFrame(int current, int count) => Get("InfoText_CurrentFrameFormat", current, count);

        /// <summary>Selected index: {0}</summary>
        internal static string InfoSelectedIndex(int index) => Get("InfoText_SelectedIndexFormat", index);

        /// <summary>ARGB value: {0:X8} ({0})
        /// Equivalent known color(s): {1}
        /// Equivalent System color(s): {2}
        /// Hue: {3:F0}°
        /// Saturation: {4:F0}%
        /// Brightness: {5:F0}%</summary>
        internal static string InfoColor(int argb, string knownColors, string systemColors, float hue, float saturation, float brightness)
            => Get("InfoText_ColorFormat", argb, knownColors, systemColors, hue, saturation, brightness);

        /// <summary>Offset: {{X={0}, Y={1}}}</summary>
        internal static string InfoTransformOffset(PointF offset) => Get("InfoText_TransformOffsetFormat", offset.X, offset.Y);

        /// <summary>Rotation and zoom matrix: [{0}; {1}] [{2}; {3}]</summary>
        internal static string InfoRotationZoom(float m0, float m1, float m2, float m3) => Get("InfoText_RotationZoomFormat", m0, m1, m2, m3);

        /// <summary>Zoom: {0}</summary>
        internal static string InfoZoom(float zoom) => Get("InfoText_ZoomFormat", zoom);

        /// <summary>Horizontal zoom: {0}</summary>
        internal static string InfoHorizontalZoom(float zoom) => Get("InfoText_HorizontalZoomFormat", zoom);

        /// <summary>Vertical zoom: {0}</summary>
        internal static string InfoVerticalZoom(float zoom) => Get("InfoText_VerticalZoomFormat", zoom);

        /// <summary>Visible Clip Bounds: {{X = {0}, Y = {1}, Size = {2}x{3} pixels}}</summary>
        internal static string InfoVisibleClip(Rectangle rect) => Get("InfoText_VisibleClipFormat", rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>Original Visible Clip Bounds: {{X = {0}, Y = {1}, Size = {2}x{3} pixels}}</summary>
        internal static string InfoOriginalVisibleClip(Rectangle rect) => Get("InfoText_OriginalVisibleClipFormat", rect.X, rect.Y, rect.Width, rect.Height);

        /// <summary>Transformed Visible Clip Bounds: {{X = {0}, Y = {1}, Size = {2}x{3} (Page Unit: {4})}}</summary>
        internal static string InfoTransformedVisibleClip(RectangleF rect, GraphicsUnit unit) => Get("InfoText_TransformedVisibleClipFormat", rect.X, rect.Y, rect.Width, rect.Height, unit);

        /// <summary>Resolution: {0}x{1} DPI</summary>
        internal static string InfoResolution(PointF dpi) => Get("InfoText_ResolutionFormat", dpi.X, dpi.Y);

        #endregion

        #region Messages

        /// <summary>Could not load file due to an error: {0}</summary>
        internal static string ErrorMessageFailedToLoadFile(string error) => Get("ErrorMessage_FailedToLoadFileFormat", error);

        /// <summary>Could not save image due to an error: {0}</summary>
        internal static string ErrorMessageFailedToSaveImage(string error) => Get("ErrorMessage_FailedToSaveImageFormat", error);

        /// <summary>File does not exist: {0}</summary>
        internal static string ErrorMessageFileDoesNotExist(string file) => Get("ErrorMessage_FileDoesNotExistFormat", file);

        /// <summary>Could not open the file as an Image: {0}</summary>
        internal static string ErrorMessageNotAnImageFile(string message) => Get("ErrorMessage_NotAnImageFileFormat", message);

        /// <summary>Could not open the file as a Metafile: {0}</summary>
        internal static string ErrorMessageNotAMetafile(string message) => Get("ErrorMessage_NotAMetafileFormat", message);

        /// <summary>Could not open the file as a Bitmap: {0}</summary>
        internal static string ErrorMessageNotABitmapFile(string message) => Get("ErrorMessage_NotABitmapFileFormat", message);

        /// <summary>Could not open the file as an Icon: {0}</summary>
        internal static string ErrorMessageNotAnIconFile(string message) => Get("ErrorMessage_NotAnIconFileFormat", message);

        /// <summary>Installation failed: {0}</summary>
        internal static string ErrorMessageInstallationFailed(string error) => Get("ErrorMessage_InstallationFailedFormat", error);

        /// <summary>Removing failed: {0}</summary>
        internal static string ErrorMessageRemoveInstallationFailed(string error) => Get("ErrorMessage_RemoveInstallationFailedFormat", error);

        /// <summary>Could not create directory {0}: {1}</summary>
        internal static string ErrorMessageCouldNotCreateDirectory(string path, string message) => Get("ErrorMessage_CouldNotCreateDirectoryFormat", path, message);

        /// <summary>Could not copy file {0}: {1}</summary>
        internal static string ErrorMessageCouldNotCopyFile(string path, string message) => Get("ErrorMessage_CouldNotCopyFileFormat", path, message);

        /// <summary>Could not delete file {0}: {1}</summary>
        internal static string ErrorMessageCouldNotDeleteFile(string path, string message) => Get("ErrorMessage_CouldNotDeleteFileFormat", path, message);

        /// <summary>Pixel format '{0}' is not supported on current platform.</summary>
        internal static string ErrorMessagePixelFormatNotSupported(PixelFormat pixelFormat) => Get("ErrorMessage_PixelFormatNotSupportedFormat", pixelFormat);

        /// <summary>The selected quantizer returns a too large palette for pixel format '{0}'.
        /// Either select at least '{1}' or reduce the number of colors to {2}.</summary>
        internal static string ErrorMessageQuantizerPaletteTooLarge(PixelFormat selectedPixelFormat, PixelFormat pixelFormatHint, int maxColors) => Get("ErrorMessage_QuantizerPaletteTooLargeFormat", selectedPixelFormat, pixelFormatHint, maxColors);

        /// <summary>Failed to initialize quantizer: {0}</summary>
        internal static string ErrorMessageFailedToInitializeQuantizer(string message) => Get("ErrorMessage_FailedToInitializeQuantizerFormat", message);

        /// <summary>Failed to initialize ditherer: {0}</summary>
        internal static string ErrorMessageFailedToInitializeDitherer(string message) => Get("ErrorMessage_FailedToInitializeDithererFormat", message);

        /// <summary>Failed to generate preview: {0}</summary>
        internal static string ErrorMessageFailedToGeneratePreview(string message) => Get("ErrorMessage_FailedToGeneratePreviewFormat", message);

        /// <summary>Value must be between {0} and {1}</summary>
        internal static string ErrorMessageValueMustBeBetween<T>(T low, T high) where T : struct => Get("ErrorMessage_ValueMustBeBetweenFormat", low, high);

        /// <summary>Value must be greater than {0}</summary>
        internal static string ErrorMessageValueMustBeGreaterThan<T>(T value) where T : struct => Get("ErrorMessage_ValueMustBeGreaterThanFormat", value);

        /// <summary>Failed to save settings: {0}</summary>
        internal static string ErrorMessageFailedToSaveSettings(string message) => Get("ErrorMessage_FailedToSaveSettingsFormat", message);

        /// <summary>Failed to regenerate resource file {0}: {1}</summary>
        internal static string ErrorMessageFailedToRegenerateResource(string fileName, string message) => Get("ErrorMessage_FailedToRegenerateResourceFormat", fileName, message);

        /// <summary>Failed to save resource file {0}: {1}</summary>
        internal static string ErrorMessageFailedToSaveResource(string fileName, string message) => Get("ErrorMessage_FailedToSaveResourceFormat", fileName, message);

        /// <summary>Could not create directory {0}: {1}
        ///
        /// The debugger visualizer may will not work for .NET Core projects.</summary>
        internal static string WarningMessageCouldNotCreateNetCoreDirectory(string path, string message) => Get("WarningMessage_CouldNotCreateNetCoreDirectoryFormat", path, message);

        /// <summary>Could not create a link for file {0}: {1}
        ///
        /// The debugger visualizer may will not work for .NET Core projects.</summary>
        internal static string WarningMessageCouldNotCreateNetCoreLink(string path, string message) => Get("WarningMessage_CouldNotCreateNetCoreLinkFormat", path, message);

        /// <summary>Could not copy file {0}: {1}
        ///
        /// The debugger visualizer may will not work for .NET Core projects.</summary>
        internal static string WarningMessageCouldNotCopyFileNetCore(string path, string message) => Get("WarningMessage_CouldNotCopyFileNetCoreFormat", path, message);

        /// <summary>The installation finished with a warning: {0}</summary>
        internal static string WarningMessageInstallationWarning(string warning) => Get("WarningMessage_InstallationWarningFormat", warning);

        /// <summary>When converting an image with wide pixel format '{0}' to another wide format,
        /// then colors will be quantized to the 32 bit ARGB color space during the conversion even if there is no quantizer used.</summary>
        internal static string WarningMessageWideConversionLoss(PixelFormat pixelFormat) => Get("WarningMessage_WideConversionLossFormat", pixelFormat);

        /// <summary>The selected quantizer uses more colors than the selected pixel format '{0}' supports.
        /// Either select at least {1} pixel format or use another quantizer that uses no more colors than '{0}' can represent;
        /// otherwise, the result might not be optimal even with dithering.</summary>
        internal static string WarningMessageQuantizerTooWide(PixelFormat selectedPixelFormat, PixelFormat pixelFormatHint) => Get("WarningMessage_QuantizerTooWideFormat", selectedPixelFormat, pixelFormatHint);

        /// <summary>The extension of the provided filename '{0}' does not match to the selected format ({1}).
        /// 
        /// Are you sure you want to save the file with the provided extension?</summary>
        internal static string ConfirmMessageSaveFileExtension(string fileName, string format) => Get("ConfirmMessage_SaveFileExtensionFormat", fileName, format);

        /// <summary>Failed to read resource file {0}: {1}
        /// 
        /// Do you want to try to regenerate it? The current file will be deleted.</summary>
        internal static string ConfirmMessageTryRegenerateResource(string fileName, string message) => Get("ConfirmMessage_TryRegenerateResourceFormat", fileName, message);

        /// <summary>{0} is the lowest compatible pixel format, which still supports the selected quantizer.</summary>
        internal static string InfoMessagePixelFormatUnnecessarilyWide(PixelFormat pixelFormat) => Get("InfoMessage_PixelFormatUnnecessarilyWideFormat", pixelFormat);

        /// <summary>The selected quantizer represents a narrower set of colors than the original '{0}'.
        /// Dithering may help to preserve more details.</summary>
        internal static string InfoMessageQuantizerCanBeDithered(PixelFormat pixelFormat) => Get("InfoMessage_QuantizerCanBeDitheredFormat", pixelFormat);

        /// <summary>The selected pixel format represents a narrower set of colors than the original '{0}'.
        /// Dithering may help to preserve more details.</summary>
        internal static string InfoMessagePixelFormatCanBeDithered(PixelFormat pixelFormat) => Get("InfoMessage_PixelFormatCanBeDitheredFormat", pixelFormat);

        /// <summary>A quantizer has been auto selected for pixel format '{0}' using default settings.
        /// Use a specific instance to adjust parameters.</summary>
        internal static string InfoMessageQuantizerAutoSelected(PixelFormat pixelFormat) => Get("InfoMessage_QuantizerAutoSelectedFormat", pixelFormat);

        /// <summary>A system default palette of {0} colors has been auto selected for pixel format '{1}'.
        /// Select a quantizer to use specific colors and settings.</summary>
        internal static string InfoMessagePaletteAutoSelected(int colors, PixelFormat pixelFormat) => Get("InfoMessage_PaletteAutoSelectedFormat", colors, pixelFormat);

        /// <summary>The ditherer is ignored for pixel format '{0}' if there is no quantizer specified.</summary>
        internal static string InfoMessageDithererIgnored(PixelFormat pixelFormat) => Get("InfoMessage_DithererIgnoredFormat", pixelFormat);

        #endregion

        #region Installations

        /// <summary>Debugger version: {0}</summary>
        internal static string InstallationAvailable(Version version) => Get("Installation_AvailableFormat", version);

        /// <summary>Debugger version: {0} - Runtime: {1}</summary>
        internal static string InstallationsAvailableWithRuntime(Version version, string runtimeVersion) => Get("Installation_AvailableWithRuntimeFormat", version, runtimeVersion);

        /// <summary>Debugger version: {0} - Target: {1}</summary>
        internal static string InstallationsAvailableWithTargetFramework(Version version, string targetFramework) => Get("Installation_AvailableWithTargetFrameworkFormat", version, targetFramework);

        /// <summary>Installed: {0}</summary>
        internal static string InstallationsStatusInstalled(Version version) => Get("Installations_StatusInstalledFormat", version);

        /// <summary>Installed: {0} - Runtime: {1}</summary>
        internal static string InstallationsStatusInstalledWithRuntime(Version version, string runtimeVersion) => Get("Installations_StatusInstalledWithRuntimeFormat", version, runtimeVersion);

        /// <summary>Installed: {0} - Target: {1}</summary>
        internal static string InstallationsStatusInstalledWithTargetFramework(Version version, string targetFramework) => Get("Installations_StatusInstalledWithTargetFrameworkFormat", version, targetFramework);

        #endregion

        #endregion

        #region Private Methods

        private static string SafeFormat(string format, object?[] args)
        {
            try
            {
                int i = Array.IndexOf(args, null);
                if (i >= 0)
                {
                    string nullRef = PublicResources.Null;
                    for (; i < args.Length; i++)
                        args[i] ??= nullRef;
                }

                return String.Format(LanguageSettings.FormattingLanguage, format, args);
            }
            catch (FormatException)
            {
                return String.Format(CultureInfo.InvariantCulture, invalidResource, args.Length, format);
            }
        }

        private static PropertyInfo[]? GetLocalizableStringProperties(Type type)
        {
            // Getting string properties only. The resource manager in this class works in safe mode anyway.
            var result = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string)
                    && Attribute.GetCustomAttribute(p, typeof(LocalizableAttribute)) is LocalizableAttribute la && la.IsLocalizable).ToArray();
            return result.Length == 0 ? null : result;
        }

        private static CultureInfo GetClosestNeutralCulture(this CultureInfo culture)
        {
            if (CultureInfo.InvariantCulture.Equals(culture))
                return culture;

            while (!culture.IsNeutralCulture)
                culture = culture.Parent;

            return culture;
        }

        #endregion

        #endregion
    }
}
