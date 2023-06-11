#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: QuantizerThresholdEditor.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2023 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Windows.Forms.Design;

using KGySoft.Drawing.ImagingTools.View.UserControls;

#endregion

namespace KGySoft.Drawing.ImagingTools.View.Design
{
    [SuppressMessage("Microsoft.Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by ComponentModel via attribute")]
    internal class QuantizerThresholdEditor : UITypeEditor
    {
        #region Properties

        public override bool IsDropDownResizable => true;

        #endregion

        #region Methods

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

        public override object? EditValue(ITypeDescriptorContext context, IServiceProvider? provider, object? value)
        {
            if (provider == null || value == null)
                return value;
            var editorService = (IWindowsFormsEditorService?)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null)
                return value;

            var originalValue = (byte)value;
            using var editorControl = new QuantizerThresholdEditorControl(editorService, originalValue);
            editorService.DropDownControl(editorControl);

            // if value didn't change returning the original boxed reference so no update will occur
            return originalValue == editorControl.Value ? value : editorControl.Value;
        }

        #endregion
    }
}