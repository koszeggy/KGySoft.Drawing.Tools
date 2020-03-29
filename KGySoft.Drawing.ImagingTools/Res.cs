#region Copyright

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

using KGySoft.Collections;
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

        // ReSharper disable once CollectionNeverUpdated.Local
        private static readonly Cache<Type, PropertyInfo[]> localizablePropertiesCache = new Cache<Type, PropertyInfo[]>(GetLocalizableProperties);

        #endregion

        #region Properties

        #region Title Captions

        /// <summary>KGy SOFT Imaging Tools</summary>
        internal static string TitleAppName => Get("Title_AppName");

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

        /// <summary>files</summary>
        internal static string TextFiles => Get("Text_Files");

        /// <summary>All files</summary>
        internal static string TextAllFiles => Get("Text_AllFiles");

        /// <summary>Metafiles</summary>
        internal static string TextMetafiles => Get("Text_Metafiles");

        /// <summary>Icon</summary>
        internal static string TextIcon => Get("Text_Icon");

        /// <summary>Images</summary>
        internal static string TextImages => Get("Text_Images");

        /// <summary>File Format</summary>
        internal static string TextFileFormat => Get("Text_FileFormat");

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

        #endregion

        #region Notifications

        /// <summary>As a standalone application, KGy SOFT Imaging Tools can be used to load images, save them in various formats, extract frames or pages, examine or change palette entries of indexed images, etc.
        /// 
        /// But it can be used also as a debugger visualizer for Image, Bitmap, Metafile, BitmapData, Graphics, ColorPalette and Color types.
        /// See the Configuration button.</summary>
        internal static string NotificationWelcome => Get("Notification_Welcome");

        /// <summary>The loaded metafile has been converted to Bitmap. To load it as a Metafile, choose the Image Debugger Visualizer instead.</summary>
        internal static string NotificationMetafileAsBitmap => Get("Notification_MetafileAsBitmap");

        /// <summary>The loaded image has been converted to Icon</summary>
        internal static string NotificationImageAsIcon => Get("Notification_ImageAsIcon");

        /// <summary>The palette of an indexed BitmapData cannot be reconstructed, therefore a default palette is used. You can change palette colors in the menu.</summary>
        internal static string NotificationPaletteCannotBeRestored => Get("Notification_PaletteCannotBeRestored");

        #endregion

        #region Messages

        /// <summary>Saving modifications as animated GIF is not supported</summary>
        internal static string ErrorMessageAnimGifNotSupported => Get("ErrorMessage_AnimGifNotSupported");

        /// <summary>Are you sure you want to overwrite this installation?</summary>
        internal static string ConfirmMessageOverwriteInstallation => Get("ConfirmMessage_OverwriteInstallation");

        /// <summary>Are you sure you want to remove this installation?</summary>
        internal static string ConfirmMessageRemoveInstallation => Get("ConfirmMessage_RemoveInstallation");

        #endregion

        #region Installations

        /// <summary>&lt;Custom Path...&gt;</summary>
        internal static string InstallationsCustomDir => Get("Installations_CustomDir");

        /// <summary>Not Installed</summary>
        internal static string InstallationsStatusNotInstalled => Get("Installations_StatusNotInstalled");

        /// <summary>Unknown version (incompatible runtime?)</summary>
        internal static string InstallationsStatusUnknown => Get("Installations_StatusUnknown");

        #endregion

        #endregion

        #region Methods

        #region Internal Methods

        #region General

        internal static string Get(string id) => resourceManager.GetString(id, LanguageSettings.DisplayLanguage) ?? String.Format(CultureInfo.InvariantCulture, unavailableResource, id);

        internal static void ApplyResources(object target, string name)
        {
            // Unlike ComponentResourceManager we don't go by ResourceSet because that would kill resource fallback traversal
            // so we go by localizable properties
            PropertyInfo[] properties = localizablePropertiesCache[target.GetType()];
            if (properties == null)
                return;

            foreach (PropertyInfo property in properties)
            {
                string value = resourceManager.GetString(name + "." + property.Name, LanguageSettings.DisplayLanguage);
                if (value == null)
                    continue;
                Reflector.SetProperty(target, property, value);
            }
        }

        /// <summary>Internal Error: {0}</summary>
        /// <remarks>Use this method to avoid CA1303 for using string literals in internal errors that never supposed to occur.</remarks>
        internal static string InternalError(string msg) => Get("General_InternalErrorFormat", msg);

        #endregion

        #region Title Captions

        /// <summary>Type: {0}</summary>
        public static string TitleType(string type) => Get("Title_TypeFormat", type);

        /// <summary>; Size: {0}</summary>
        public static string TitleSize(string size) => Get("Title_SizeFormat", size);

        /// <summary>Palette Count: {0}</summary>
        public static string TitlePaletteCount(int count) => Get("Title_PaletteCountFormat", count);

        /// <summary>Visible Clip Bounds: {0}</summary>
        public static string TitleVisibleClip(Rectangle rect) => Get("Title_VisibleClipFormat", rect);

        /// <summary>Untransformed Visible Clip Bounds: {0}</summary>
        public static string TitleUntransformedVisibleClip(Rectangle rect) => Get("Title_UntransformedVisibleClipFormat", rect);

        /// <summary>Color: {0}</summary>
        public static string TitleColor(Color color) => Get("Title_ColorFormat", color.Name);

        #endregion

        #region Texts

        /// <summary>A: {0}</summary>
        public static string TextAlphaValue(byte a) => Get("Text_AlphaValueFormat", a);

        /// <summary>R: {0}</summary>
        public static string TextRedValue(byte r) => Get("Text_RedValueFormat", r);

        /// <summary>G: {0}</summary>
        public static string TextGreenValue(byte g) => Get("Text_GreenValueFormat", g);

        /// <summary>B: {0}</summary>
        public static string TextBlueValue(byte a) => Get("Text_BlueValueFormat", a);

        #endregion

        #region Info Texts

        /// <summary>Type: {0}
        /// Size: {1}
        /// {6}Pixel Format: {2}
        /// Raw format: {3}
        /// Resolution: {4} x {5} dpi</summary>
        public static string InfoImage(string type, string size, PixelFormat pixelFormat, string rawFormat, float hres, float vres, string frameInfo)
            => Get("InfoText_ImageFormat", type, size, pixelFormat, rawFormat, hres, vres, frameInfo);

        /// <summary>Unknown format: {0}</summary>
        public static string InfoUnknownFormat(Guid format) => Get("InfoText_UnknownFormat", format);

        /// <summary>Palette count: {0}</summary>
        public static string InfoPalette(int count) => Get("InfoText_PaletteFormat", count);

        /// <summary>Images: {0}</summary>
        public static string InfoFramesCount(int count) => Get("InfoText_FramesCountFormat", count);

        /// <summary>Current Image: {0}/{1}</summary>
        public static string InfoCurrentFrame(int current, int count) => Get("InfoText_CurrentFrameFormat", current, count);

        /// <summary>Selected index: {0}</summary>
        public static string InfoSelectedIndex(int index) => Get("InfoText_SelectedIndexFormat", index);

        /// <summary>ARGB value: {0:X8} ({0})
        /// Equivalent known color(s): {1}
        /// Equivalent System color(s): {2}
        /// Hue: {3:F0}°
        /// Saturation: {4:F0}%
        /// Brightness: {5:F0}%</summary>
        public static string InfoColor(int argb, string knownColors, string systemColors, float hue, float saturation, float brightness)
            => Get("InfoText_ColorFormat", argb, knownColors, systemColors, hue, saturation, brightness);

        #endregion

        #region Messages

        /// <summary>Could not load file due to an error: {0}</summary>
        public static string ErrorMessageFailedToLoadFile(string error) => Get("ErrorMessage_FailedToLoadFileFormat", error);

        /// <summary>Could not save image due to an error: {0}</summary>
        public static string ErrorMessageFailedToSaveImage(string error) => Get("ErrorMessage_FailedToSaveImageFormat", error);

        /// <summary>File does not exist: {0}</summary>
        public static string ErrorMessageFileDoesNotExist(string file) => Get("ErrorMessage_FileDoesNotExistFormat", file);

        /// <summary>Installation failed: {0}</summary>
        internal static string ErrorMessageInstallationFailed(string error) => Get("ErrorMessage_InstallationFailedFormat", error);

        /// <summary>Removing failed: {0}</summary>
        internal static string ErrorMessageRemoveInstallationFailed(string error) => Get("ErrorMessage_RemoveInstallationFailedFormat", error);

        /// <summary>Could not open the stream as an Image: {0}</summary>
        internal static string ErrorMessageNotAnImageStream(string message) => Get("ErrorMessage_NotAnImageStreamFormat", message);

        /// <summary>Could not open the stream as a Metafile: {0}</summary>
        internal static string ErrorMessageNotAMetafileStream(string message) => Get("ErrorMessage_NotAMetafileStreamFormat", message);

        /// <summary>Could not open the stream as a Bitmap: {0}</summary>
        internal static string ErrorMessageNotABitmapStream(string message) => Get("ErrorMessage_NotABitmapStreamFormat", message);

        /// <summary>Could not open the stream as an Icon: {0}</summary>
        internal static string ErrorMessageNotAnIconStream(string message) => Get("ErrorMessage_NotAnIconStreamFormat", message);


        #endregion

        #region Installations

        /// <summary>Installed: {0}</summary>
        public static string InstallationsStatusInstalled(Version version) => Get("Installations_StatusInstalledFormat", version);

        /// <summary>Installed: {0} - Runtime: {1}</summary>
        public static string InstallationsStatusInstalledWithRuntime(Version version, string runtimeVersion) => Get("Installations_StatusInstalledWithRuntimeFormat", version, runtimeVersion);

        #endregion

        #endregion

        #region Private Methods

        private static string Get(string id, params object[] args)
        {
            string format = Get(id);
            return args == null ? format : SafeFormat(format, args);
        }

        private static string SafeFormat(string format, object[] args)
        {
            try
            {
                int i = Array.IndexOf(args, null);
                if (i >= 0)
                {
                    string nullRef = PublicResources.Null;
                    for (; i < args.Length; i++)
                    {
                        if (args[i] == null)
                            args[i] = nullRef;
                    }
                }

                return String.Format(LanguageSettings.FormattingLanguage, format, args);
            }
            catch (FormatException)
            {
                return String.Format(CultureInfo.InvariantCulture, invalidResource, args.Length, format);
            }
        }

        private static PropertyInfo[] GetLocalizableProperties(Type type)
        {
            // Getting string properties only. The resource manager in this class works in safe mode anyway.
            var result = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(string)
                            && Attribute.GetCustomAttribute(p, typeof(LocalizableAttribute)) is LocalizableAttribute la && la.IsLocalizable).ToArray();
            return result.Length == 0 ? null : result;
        }

        #endregion

        #endregion
    }
}
