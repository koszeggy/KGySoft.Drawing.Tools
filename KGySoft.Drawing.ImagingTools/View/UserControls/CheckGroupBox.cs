#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CheckGroupBox.cs
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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.UserControls
{
    [Designer(typeof(CheckGroupBoxDesigner))]
    internal partial class CheckGroupBox : BaseUserControl
    {
        #region Nested classes

        #region CheckGroupBoxDesigner class

        private sealed class CheckGroupBoxDesigner : ParentControlDesigner
        {
            #region Fields

            IDesignerHost designerHost;

            #endregion

            #region Properties

            public override System.Collections.ICollection AssociatedComponents => ((CheckGroupBox)Control).groupBox.Controls.Cast<Control>().ToList();

            #endregion

            #region Methods

            #region Public Methods

            public override void Initialize(IComponent component)
            {
                base.Initialize(component);
                AutoResizeHandles = true;
                EnableDesignMode(((CheckGroupBox)component).groupBox, nameof(CheckGroupBox.GroupBox));
                designerHost = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
            }

            public override bool CanParent(Control control) => false;
            public override int NumberOfInternalControlDesigners() => 1;

            public override ControlDesigner InternalControlDesigner(int internalControlIndex)
            {
                Control panel = ((CheckGroupBox)Control).groupBox;
                switch (internalControlIndex)
                {
                    case 0:
                        return this.designerHost.GetDesigner(panel) as ControlDesigner;
                    default:
                        return null;
                }
            }

            #endregion

            #region Protected Methods

            protected override Control GetParentForComponent(IComponent component) => ((CheckGroupBox)Control).groupBox;

            protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
            {
                ParentControlDesigner panelDesigner = designerHost.GetDesigner(((CheckGroupBox)Control).groupBox) as ParentControlDesigner;
                InvokeCreateTool(panelDesigner, tool);
                return null;
            }

            #endregion

            #endregion
        }

        #endregion

        #region ContentGroupBox class

        /// <summary>
        /// A GroupBox whose default properties are not serialized
        /// </summary>
        [ToolboxItem(false)]
        [Designer("System.Windows.Forms.Design.GroupBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        private sealed class ContentGroupBox : GroupBox
        {
            #region Properties

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
            public override DockStyle Dock
            {
                // it is not enough just returning DockStyle.Fill because base must be explicitly set
                get => base.Dock;
                set => base.Dock = value;
            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
            public new Point Location => Point.Empty;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
            public new Padding Padding => base.Padding;

            [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
            public new string Name => base.Name;

            [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
            public new Size Size
            {
                get => base.Size;
                set => base.Size = value;
            }

            [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
            public new int TabIndex => base.TabIndex;

            [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
            public new bool TabStop => base.TabStop;

            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
            public new bool Visible => base.Visible;

            #endregion

            #region Constructors

            public ContentGroupBox()
            {
                base.Dock = DockStyle.Fill;
                base.Padding = new Padding(3, 4, 3, 3);
            }

            #endregion
        }

        #endregion

        #endregion

        #region Events

        internal event EventHandler CheckedChanged
        {
            add => Events.AddHandler(nameof(CheckedChanged), value);
            remove => Events.RemoveHandler(nameof(CheckedChanged), value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the content panel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GroupBox GroupBox => groupBox;

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => checkBox.Text;
            set => checkBox.Text = value;
        }

        [DefaultValue(true)]
        public bool Checked
        {
            get => checkBox.Checked;
            set => checkBox.Checked = value;
        }

        #endregion

        #region Constructors

        public CheckGroupBox()
        {
            // constructor must be public so the designer can create an instance
            InitializeComponent();
            checkBox.CheckedChanged += CheckBox_CheckedChanged;
        }

        #endregion

        #region Methods

        #region Protected Methods

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            checkBox.CheckedChanged -= CheckBox_CheckedChanged;

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void OnCheckedChanged(EventArgs e) => (Events[nameof(CheckedChanged)] as EventHandler)?.Invoke(this, e);

        #endregion

        #region Event handlers

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            groupBox.Enabled = checkBox.Checked;
            OnCheckedChanged(e);
        }

        #endregion

        #endregion
    }
}
