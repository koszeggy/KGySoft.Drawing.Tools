using System.Windows.Forms;
using KGySoft.Drawing.ImagingTools.Controls;

namespace KGySoft.Drawing.ImagingTools.Forms
{
    partial class ImageDebuggerVisualizerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.tsMenu = new ScalingToolStrip();
            this.btnAutoZoom = new System.Windows.Forms.ToolStripButton();
            this.btnColorSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.miBackColor = new System.Windows.Forms.ToolStripMenuItem();
            this.miDeafult = new System.Windows.Forms.ToolStripMenuItem();
            this.miWhite = new System.Windows.Forms.ToolStripMenuItem();
            this.miBlack = new System.Windows.Forms.ToolStripMenuItem();
            this.miShowPalette = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.sepFrames = new System.Windows.Forms.ToolStripSeparator();
            this.btnCompound = new System.Windows.Forms.ToolStripButton();
            this.btnPrev = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnConfiguration = new System.Windows.Forms.ToolStripButton();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.splitter = new System.Windows.Forms.Splitter();
            this.timerPlayer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblNotification = new KGySoft.Drawing.ImagingTools.Controls.NotificationLabel();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.tsMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtInfo.Location = new System.Drawing.Point(0, 193);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtInfo.Size = new System.Drawing.Size(334, 123);
            this.txtInfo.TabIndex = 0;
            this.txtInfo.WordWrap = false;
            this.txtInfo.TextChanged += new System.EventHandler(this.txtInfo_TextChanged);
            this.txtInfo.Enter += new System.EventHandler(this.txtInfo_Enter);
            // 
            // tsMenu
            // 
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAutoZoom,
            this.btnColorSettings,
            this.toolStripSeparator1,
            this.btnSave,
            this.btnOpen,
            this.btnClear,
            this.sepFrames,
            this.btnCompound,
            this.btnPrev,
            this.btnNext,
            this.toolStripSeparator2,
            this.btnConfiguration});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(334, 25);
            this.tsMenu.TabIndex = 2;
            // 
            // btnAutoZoom
            // 
            this.btnAutoZoom.CheckOnClick = true;
            this.btnAutoZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAutoZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoZoom.Name = "btnAutoZoom";
            this.btnAutoZoom.Size = new System.Drawing.Size(23, 22);
            this.btnAutoZoom.Text = "Auto Zoom";
            this.btnAutoZoom.CheckedChanged += new System.EventHandler(this.btnAutoZoom_CheckedChanged);
            // 
            // btnColorSettings
            // 
            this.btnColorSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnColorSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miBackColor,
            this.miShowPalette});
            this.btnColorSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnColorSettings.Name = "btnColorSettings";
            this.btnColorSettings.Size = new System.Drawing.Size(13, 22);
            this.btnColorSettings.Text = "Color Settings";
            // 
            // miBackColor
            // 
            this.miBackColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDeafult,
            this.miWhite,
            this.miBlack});
            this.miBackColor.Name = "miBackColor";
            this.miBackColor.Size = new System.Drawing.Size(142, 22);
            this.miBackColor.Text = "Back Color";
            // 
            // miDeafult
            // 
            this.miDeafult.Checked = true;
            this.miDeafult.CheckState = System.Windows.Forms.CheckState.Checked;
            this.miDeafult.Name = "miDeafult";
            this.miDeafult.Size = new System.Drawing.Size(112, 22);
            this.miDeafult.Text = "Default";
            this.miDeafult.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.miDeafult.Click += new System.EventHandler(this.miBackColor_Click);
            // 
            // miWhite
            // 
            this.miWhite.BackColor = System.Drawing.Color.White;
            this.miWhite.ForeColor = System.Drawing.Color.Black;
            this.miWhite.Name = "miWhite";
            this.miWhite.Size = new System.Drawing.Size(112, 22);
            this.miWhite.Text = "White";
            this.miWhite.Click += new System.EventHandler(this.miBackColor_Click);
            // 
            // miBlack
            // 
            this.miBlack.BackColor = System.Drawing.Color.Black;
            this.miBlack.ForeColor = System.Drawing.Color.White;
            this.miBlack.Name = "miBlack";
            this.miBlack.Size = new System.Drawing.Size(112, 22);
            this.miBlack.Text = "Black";
            this.miBlack.Click += new System.EventHandler(this.miBackColor_Click);
            // 
            // miShowPalette
            // 
            this.miShowPalette.Name = "miShowPalette";
            this.miShowPalette.Size = new System.Drawing.Size(142, 22);
            this.miShowPalette.Text = "Show Palette";
            this.miShowPalette.Click += new System.EventHandler(this.miShowPalette_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save As";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(23, 22);
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // sepFrames
            // 
            this.sepFrames.Name = "sepFrames";
            this.sepFrames.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCompound
            // 
            this.btnCompound.Checked = true;
            this.btnCompound.CheckOnClick = true;
            this.btnCompound.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnCompound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompound.Name = "btnCompound";
            this.btnCompound.Size = new System.Drawing.Size(23, 22);
            this.btnCompound.Text = "Toggle as a single image";
            this.btnCompound.Click += new System.EventHandler(this.btnCompound_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(23, 22);
            this.btnPrev.Text = "Previous Image";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            this.btnNext.Text = "Next Image";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnConfiguration
            // 
            this.btnConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfiguration.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Size = new System.Drawing.Size(23, 22);
            this.btnConfiguration.Text = "Manage Debugger Visualizer Installations...";
            this.btnConfiguration.Click += new System.EventHandler(this.btnConfiguration_Click);
            // 
            // dlgOpen
            // 
            this.dlgOpen.Title = "Open Image";
            // 
            // dlgSave
            // 
            this.dlgSave.Title = "Save Image As...";
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 190);
            this.splitter.MinExtra = 16;
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(334, 3);
            this.splitter.TabIndex = 3;
            this.splitter.TabStop = false;
            // 
            // timerPlayer
            // 
            this.timerPlayer.Interval = 10;
            this.timerPlayer.Tick += new System.EventHandler(this.timerPlayer_Tick);
            // 
            // lblNotification
            // 
            this.lblNotification.AutoSize = true;
            this.lblNotification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblNotification.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNotification.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNotification.ForeColor = System.Drawing.Color.Black;
            this.lblNotification.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNotification.Location = new System.Drawing.Point(0, 25);
            this.lblNotification.Name = "lblNotification";
            this.lblNotification.Padding = new System.Windows.Forms.Padding(3, 3, 20, 3);
            this.lblNotification.Size = new System.Drawing.Size(334, 24);
            this.lblNotification.TabIndex = 4;
            this.lblNotification.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.lblNotification, "Click to hide");
            // 
            // pbImage
            // 
            this.pbImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImage.ImageLocation = "";
            this.pbImage.Location = new System.Drawing.Point(0, 49);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(334, 141);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbImage.TabIndex = 1;
            this.pbImage.TabStop = false;
            this.pbImage.SizeChanged += new System.EventHandler(this.pbImage_SizeChanged);
            // 
            // ImageDebuggerVisualizerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 316);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.lblNotification);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.txtInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "ImageDebuggerVisualizerForm";
            this.Text = "ImageDebuggerVisualizerForm";
            this.Load += new System.EventHandler(this.ImageDebuggerVisualizerForm_Load);
            this.Resize += new System.EventHandler(this.ImageDebuggerVisualizerForm_Resize);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.ToolStripButton btnAutoZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator sepFrames;
        private System.Windows.Forms.ToolStripButton btnPrev;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnCompound;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.SaveFileDialog dlgSave;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.Timer timerPlayer;
        private System.Windows.Forms.ToolStripDropDownButton btnColorSettings;
        private System.Windows.Forms.ToolStripMenuItem miBackColor;
        private System.Windows.Forms.ToolStripMenuItem miDeafult;
        private System.Windows.Forms.ToolStripMenuItem miWhite;
        private System.Windows.Forms.ToolStripMenuItem miBlack;
        private System.Windows.Forms.ToolStripMenuItem miShowPalette;
        protected ScalingToolStrip tsMenu;
        protected System.Windows.Forms.TextBox txtInfo;
        private NotificationLabel lblNotification;
        private ToolTip toolTip;
        private ToolStripSeparator toolStripSeparator2;
        protected ToolStripButton btnConfiguration;
    }
}