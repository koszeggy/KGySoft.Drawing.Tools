#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPropertiesObject.cs
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using KGySoft.ComponentModel;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal class CustomPropertiesObject : PersistableObjectBase, ICustomPropertiesProvider
    {
        #region Fields

        private readonly PropertyDescriptorCollection propertyDescriptors;

        #endregion

        #region Constructors

        internal CustomPropertiesObject(IEnumerable<CustomPropertyDescriptor> descriptors)
        {
            propertyDescriptors = new PropertyDescriptorCollection(descriptors.Cast<PropertyDescriptor>().ToArray());
        }

        internal CustomPropertiesObject(CustomPropertiesObject other, IEnumerable<CustomPropertyDescriptor> descriptors)
            : this(descriptors)
        {
            // allowing to set any properties from other so old values are not lost when changing instances even they are not among currently available properties
            ((IPersistableObject)this).SetProperties(((IPersistableObject)other).GetProperties());
        }

        #endregion

        #region Methods

        #region Protected Methods

        // Base allows to set reflected properties only. We could check propertyDescriptors here but ctor needs to set any property
        // and validation is performed by CustomPropertyDescriptor
        protected override bool CanGetProperty(string propertyName) => true;
        protected override bool CanSetProperty(string propertyName, object? value) => true;

        #endregion

        #region Explicitly Implemented Interface Methods

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(this, true);
        string ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(this, true);
        string? ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(this, true);
        TypeConverter ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(this, true);
        EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);
        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);
        object? ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(this, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => this;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => propertyDescriptors;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes)
            => new PropertyDescriptorCollection(propertyDescriptors.Cast<PropertyDescriptor>().Where(p => attributes == null || attributes.Any(a => p.Attributes.Contains(a))).ToArray());

        void ICustomPropertiesProvider.ResetValue(string propertyName) => ResetProperty(propertyName);
        void ICustomPropertiesProvider.SetValue(string propertyName, object? value) => Set(value, true, propertyName);
        object? ICustomPropertiesProvider.GetValue(string propertyName, object? defaultValue) => Get(defaultValue, propertyName);

        #endregion

        #endregion
    }
}
