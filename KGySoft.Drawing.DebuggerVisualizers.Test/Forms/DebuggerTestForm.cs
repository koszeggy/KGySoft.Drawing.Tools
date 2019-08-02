#region Used namespaces

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using KGySoft.ComponentModel;
using KGySoft.CoreLibraries;
using KGySoft.Drawing.DebuggerVisualizers.Serializers;
using KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel;
using KGySoft.Drawing.ImagingTools.Forms;
using KGySoft.Reflection;
using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test.Forms
{
    public partial class DebuggerTestForm : Form
    {
        #region Nested classes

        #region TestWindowService class

        class TestWindowService : IDialogVisualizerService
        {
            #region Methods

            public DialogResult ShowDialog(Control control)
            {
                throw new NotImplementedException();
            }

            public DialogResult ShowDialog(CommonDialog dialog)
            {
                throw new NotImplementedException();
            }

            public DialogResult ShowDialog(Form form)
            {
                return form.ShowDialog();
            }

            #endregion
        }

        #endregion

        #region TestObjectProvider class

        class TestObjectProvider : IVisualizerObjectProvider
        {
            #region Fields

            VisualizerObjectSource serializer;

            private object obj;

            #endregion

            #region Properties

            public bool IsObjectReplaceable
            {
                get { return true; }
            }

            #endregion

            #region Constructors

            public TestObjectProvider(VisualizerObjectSource objectSource, object obj)
            {
                serializer = objectSource;
                this.obj = obj;
            }

            #endregion

            #region Methods

            public Stream GetData()
            {
                MemoryStream ms = new MemoryStream();
                serializer.GetData(obj, ms);
                ms.Position = 0;
                return ms;
            }

            public object GetObject()
            {
                MemoryStream ms = new MemoryStream();
                serializer.GetData(obj, ms);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }

            public void ReplaceData(Stream newObjectData)
            {
                throw new NotImplementedException();
            }

            public void ReplaceObject(object newObject)
            {
                obj = newObject.DeepClone();
            }

            public Stream TransferData(Stream outgoingData)
            {
                throw new NotImplementedException();
            }

            public object TransferObject(object outgoingObject)
            {
                throw new NotImplementedException();
            }

            internal object GetReplacedObject()
            {
                return obj;
            }

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private readonly CommandBindingsCollection commandBindings = new CommandBindingsCollection();
        private readonly DebuggerTestFormViewModel viewModel = new DebuggerTestFormViewModel();
        private readonly Bitmap fallbackPreview = new Bitmap(1, 1);

        #endregion

        #region Constructors

        public DebuggerTestForm()
        {
            InitializeComponent();
            commandBindings.AddPropertyBinding(rbBitmap32, nameof(RadioButton.Checked), nameof(viewModel.Bmp32), viewModel);
            commandBindings.AddPropertyBinding(rbBitmap16, nameof(RadioButton.Checked), nameof(viewModel.Bmp16), viewModel);
            commandBindings.AddPropertyBinding(rbBitmap4, nameof(RadioButton.Checked), nameof(viewModel.Bmp4), viewModel);
            commandBindings.AddPropertyBinding(rbMetafile, nameof(RadioButton.Checked), nameof(viewModel.Metafile), viewModel);
            commandBindings.AddPropertyBinding(rbHIcon, nameof(RadioButton.Checked), nameof(viewModel.HIcon), viewModel);
            commandBindings.AddPropertyBinding(rbManagedIcon, nameof(RadioButton.Checked), nameof(viewModel.ManagedIcon), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsBitmap, nameof(RadioButton.Checked), nameof(viewModel.GraphicsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbGraphicsHwnd, nameof(RadioButton.Checked), nameof(viewModel.GraphicsControl), viewModel);
            commandBindings.AddPropertyBinding(rbBitmapData32, nameof(RadioButton.Checked), nameof(viewModel.BitmapData32), viewModel);
            commandBindings.AddPropertyBinding(rbBitmapData8, nameof(RadioButton.Checked), nameof(viewModel.BitmapData8), viewModel);
            commandBindings.AddPropertyBinding(rbPalette256, nameof(RadioButton.Checked), nameof(viewModel.Palette256), viewModel);
            commandBindings.AddPropertyBinding(rbPalette2, nameof(RadioButton.Checked), nameof(viewModel.Palette2), viewModel);
            commandBindings.AddPropertyBinding(rbColor, nameof(RadioButton.Checked), nameof(viewModel.Color), viewModel);
            commandBindings.AddPropertyBinding(rbFromFile, nameof(RadioButton.Checked), nameof(viewModel.ImageFromFile), viewModel);

            commandBindings.AddPropertyBinding(rbAsImage, nameof(RadioButton.Checked), nameof(viewModel.AsImage), viewModel);
            commandBindings.AddPropertyBinding(rbAsBitmap, nameof(RadioButton.Checked), nameof(viewModel.AsBitmap), viewModel);
            commandBindings.AddPropertyBinding(rbAsMetafile, nameof(RadioButton.Checked), nameof(viewModel.AsMetafile), viewModel);
            commandBindings.AddPropertyBinding(rbAsIcon, nameof(RadioButton.Checked), nameof(viewModel.AsIcon), viewModel);

            pictureBox.DataBindings.Add(nameof(pictureBox.Image), viewModel, nameof(viewModel.PreviewImage), true)
                .Format += (sender, e) => e.Value = e.Value ?? fallbackPreview;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                commandBindings.Dispose();
                viewModel.Dispose();
            }

            base.Dispose(disposing);
        }

        //private void RefreshFromFile()
        //{
        //    string fileName = tbFile.Text;
        //    if (File.Exists(fileName))
        //    {
        //        try
        //        {
        //            Image image = Image.FromFile(fileName);
        //            if (pictureBox.Image != null)
        //                pictureBox.Image.Dispose();

        //            pictureBox.Image = (Image)image.Clone();
        //            if (image.RawFormat.Guid == ImageFormat.Icon.Guid)
        //                icon = new Icon(fileName);
        //            else
        //                icon = null;

        //            image.Dispose();
        //        }
        //        catch (Exception)
        //        {
        //            icon = null;
        //            pictureBox.Image = null;
        //        }
        //    }
        //    else
        //        pictureBox.Image = null;
        //}

        #endregion

        //#region Event Handlers
        ////ReSharper disable InconsistentNaming

        //private void ucCustomSelector1_ButtonBrowseClick(object sender, EventArgs e)
        //{
        //    using (OpenFileDialog ofd = new OpenFileDialog())
        //    {
        //        if (ofd.ShowDialog() == DialogResult.OK)
        //            tbFile.Text = ofd.FileName;
        //    }
        //}

        //private void ucCustomSelector1_ValueChanged(object sender, EventArgs e)
        //{
        //    if (rbFromFile.Checked)
        //        RefreshFromFile();
        //}

        //private void rbBitmap32_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbBitmap32.Checked)
        //        return;

        //    icon = null;
        //    pictureBox.Image = Icons.Shield.ExtractBitmap(0);
        //}

        //private void rbMetafile_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbMetafile.Checked)
        //        return;

        //    icon = null;

        //    //Set up reference Graphic
        //    Graphics refGraph = this.CreateGraphics();
        //    IntPtr hdc = refGraph.GetHdc();
        //    Metafile img = new Metafile(hdc, new Rectangle(0, 0, 100, 100), MetafileFrameUnit.Pixel, EmfType.EmfOnly, "Test");

        //    //Draw some silly drawing
        //    using (var g = Graphics.FromImage(img))
        //    {
        //        var r = new Rectangle(0, 0, 100, 100);
        //        var leftEye = new Rectangle(20, 20, 20, 30);
        //        var rightEye = new Rectangle(60, 20, 20, 30);

        //        using (Pen pRed = new Pen(Color.Red, 2.5f))
        //        using (var pBlack = new Pen(Color.Black, 3))
        //        {
        //            g.FillEllipse(Brushes.Yellow, r);
        //            g.FillEllipse(Brushes.White, leftEye);
        //            g.FillEllipse(Brushes.White, rightEye);
        //            g.DrawEllipse(pBlack, leftEye);
        //            g.DrawEllipse(pBlack, rightEye);
        //            g.DrawBezier(pRed, new Point(10, 50), new Point(10, 100), new Point(90, 100), new Point(90, 50));
        //        }
        //    }

        //    refGraph.ReleaseHdc(hdc); //cleanup
        //    refGraph.Dispose();

        //    //using (Stream s = File.Create(@"d:\temp\wmf.wmf"))
        //    //{
        //    //    img.Save(s, false);
        //    //    s.Flush();
        //    //}

        //    //using (Stream s = File.Create(@"d:\temp\emf.emf"))
        //    //{
        //    //    img.Save(s, true);
        //    //    s.Flush();
        //    //}

        //    pictureBox.Image = img;
        //}

        //private void rbHIcon_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbHIcon.Checked)
        //        return;

        //    icon = SystemIcons.Information;
        //    pictureBox.Image = SystemIcons.Information.ToMultiResBitmap();
        //}

        //private void rbManagedIcon_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbManagedIcon.Checked)
        //        return;

        //    icon = Icons.Information;
        //    pictureBox.Image = Icons.Information.ToMultiResBitmap();
        //}

        //private void advancedRadioButton5_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbFromFile.Checked)
        //        return;

        //    RefreshFromFile();
        //}

        //private void advancedButton1_Click(object sender, EventArgs e)
        //{
        //    ImageDebuggerVisualizer debugger = new ImageDebuggerVisualizer();
        //    TestWindowService windowService = new TestWindowService();
        //    TestObjectProvider objectProvider = new TestObjectProvider(new ImageSerializer(),  icon != null && advancedCheckBox1.Checked ? (object)icon : pictureBox.Image);
        //    Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
        //    object result = objectProvider.GetReplacedObject();
        //}

        //private void advancedButton2_Click(object sender, EventArgs e)
        //{
        //    using (ImageDebuggerVisualizerForm frm = new ImageDebuggerVisualizerForm())
        //    {
        //        Image image = null;
        //        if (icon != null && advancedCheckBox1.Checked)
        //        {
        //            Icon ico = (Icon)icon.Clone();
        //            frm.Icon = ico;
        //            image = frm.Image;
        //        }
        //        else
        //        {
        //            if (pictureBox.Image != null)
        //                image = (Image)pictureBox.Image.Clone();
        //            frm.Image = image;
        //        }

        //        frm.ShowDialog(this);
        //        if (frm.Image != image)
        //            pictureBox.Image = frm.Image == null ? null : (Image)frm.Image.Clone();
        //    }
        //}

        //private void rbBitmap16_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbBitmap16.Checked)
        //        return;

        //    icon = null;
        //    pictureBox.Image = Icons.Shield.ToAlphaBitmap().ConvertPixelFormat(PixelFormat.Format16bppRgb555, null);
        //    //using (Graphics g = Graphics.FromHwnd(ucCustomSelector1.Handle))
        //    ////using (Graphics g = Graphics.FromImage(Images.ErrorLarge))
        //    //{
        //    //    //g.SetClip(new Rectangle(10, 10, 10, 10));
        //    //    pictureBox.Image = g.ToBitmap(false);
        //    //}
        //}

        //private void rbBitmap4_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (!rbBitmap4.Checked)
        //        return;

        //    icon = null;
        //    pictureBox.Image = Icons.Shield.ToAlphaBitmap().ConvertPixelFormat(PixelFormat.Format4bppIndexed, null);
        //    //pictureBox.Image = Images.ShieldLarge.ConvertPixelFormat(PixelFormat.Format8bppIndexed, Images.ShieldLarge.GetColors(Int32.MaxValue));
        //}

        //private void advancedButton3_Click(object sender, EventArgs e)
        //{
        //    using (Graphics g = Graphics.FromHwnd(Handle))
        //    {
        //        //Matrix m = g.Transform;
        //        //m.Translate(ucCustomSelector1.Left, ucCustomSelector1.Top);
        //        //g.Transform = m;
        //        //g.SetClip(new Rectangle(Point.Empty, ucCustomSelector1.Size));
        //        g.SetClip(tbFile.Bounds, CombineMode.Intersect);

        //        GraphicsDebuggerVisualizer debugger = new GraphicsDebuggerVisualizer();
        //        TestWindowService windowService = new TestWindowService();
        //        TestObjectProvider objectProvider = new TestObjectProvider(new GraphicsSerializer(), g);
        //        Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
        //    }
        //}

        //private void advancedButton4_Click(object sender, EventArgs e)
        //{
        //    if (pictureBox.Image == null || pictureBox.Image is Metafile || Image.GetPixelFormatSize(pictureBox.Image.PixelFormat) <= 8)
        //        return;

        //    using (Graphics g = Graphics.FromImage(pictureBox.Image))
        //    {
        //        g.SetClip(new Rectangle(0, 0, 25, 25));
        //        GraphicsDebuggerVisualizer debugger = new GraphicsDebuggerVisualizer();
        //        TestWindowService windowService = new TestWindowService();
        //        TestObjectProvider objectProvider = new TestObjectProvider(new GraphicsSerializer(), g);
        //        Reflector.InvokeMethod(debugger, "Show", windowService, objectProvider);
        //    }
        //}

        ////ReSharper restore InconsistentNaming
        //#endregion

        //private void rbGraphicsBitmap_CheckedChanged(object sender, EventArgs e)
        //{

        //}
    }
}
