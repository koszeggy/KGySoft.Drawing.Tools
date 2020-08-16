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
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    internal class CustomPropertiesObject : PersistableObjectBase, ICustomPropertiesProvider
    {
        #region Fields

        private readonly PropertyDescriptorCollection propertyDescriptors;

        private bool isSuspended;

        #endregion

        #region Constructors

        internal CustomPropertiesObject(IEnumerable<CustomPropertyDescriptor> descriptors)
        {
            propertyDescriptors = new PropertyDescriptorCollection(descriptors.Cast<PropertyDescriptor>().ToArray());
        }

        internal CustomPropertiesObject(CustomPropertiesObject other, IEnumerable<CustomPropertyDescriptor> descriptors)
            : this(descriptors)
        {
            other.Suspend();
            Suspend();
            try
            {
                // allowing to set any properties from other so old values are not lost when changing instances even they are not among currently available properties
                ((IPersistableObject)this).SetProperties(((IPersistableObject)other).GetProperties());
            }
            finally
            {
                other.Resume();
                Resume();
            }
        }

        #endregion

        #region Methods

        #region Protected Methods

        // if the object is suspended we allow to get/set any properties so the copy constructor can populate any data
        protected override bool CanGetProperty(string propertyName) => isSuspended || propertyDescriptors.Find(propertyName, false) != null;

        protected override bool CanSetProperty(string propertyName, object value) => isSuspended
            || propertyDescriptors.Find(propertyName, false) is CustomPropertyDescriptor descriptor
                && descriptor.PropertyType.CanAcceptValue(value) && (descriptor.AllowedValues == null || value.In(descriptor.AllowedValues));

        #endregion

        #region Private Methods

        private void Suspend()
        {
            if (isSuspended)
                return;
            isSuspended = true;
            SuspendChangedEvent();
        }

        private void Resume()
        {
            if (!isSuspended)
                return;
            isSuspended = false;
            ResumeChangedEvent();
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => TypeDescriptor.GetAttributes(this, true);
        string ICustomTypeDescriptor.GetClassName() => TypeDescriptor.GetClassName(this, true);
        string ICustomTypeDescriptor.GetComponentName() => TypeDescriptor.GetComponentName(this, true);
        TypeConverter ICustomTypeDescriptor.GetConverter() => TypeDescriptor.GetConverter(this, true);
        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => TypeDescriptor.GetDefaultEvent(this, true);
        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this, true);
        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => TypeDescriptor.GetEvents(this, true);
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => TypeDescriptor.GetEvents(this, attributes, true);
        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => this;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => propertyDescriptors;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            => new PropertyDescriptorCollection(propertyDescriptors.Cast<PropertyDescriptor>().Where(p => attributes == null || attributes.Any(a => p.Attributes.Contains(a))).ToArray());

        void ICustomPropertiesProvider.ResetValue(CustomPropertyDescriptor descriptor) => ResetProperty(descriptor.Name);

        void ICustomPropertiesProvider.SetValue(CustomPropertyDescriptor descriptor, object value)
            // AllowedValues is checked in CanSetProperty
            => Set(value, true, descriptor.Name);

        object ICustomPropertiesProvider.GetValue(CustomPropertyDescriptor descriptor)
        {
            object result = Get(descriptor.DefaultValue, descriptor.Name);
            if (descriptor.AllowedValues != null && !result.In(descriptor.AllowedValues))
                result = descriptor.AllowedValues.Length > 0 ? descriptor.AllowedValues[0] : descriptor.DefaultValue;
            return result;
        }

        #endregion

        #endregion
    }
}
