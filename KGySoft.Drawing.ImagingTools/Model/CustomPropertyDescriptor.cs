#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: CustomPropertyDescriptor.cs
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
using System.Drawing.Design;
using System.Globalization;
using System.Linq;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Drawing.ImagingTools.Model
{
    /// <summary>
    /// Represents a property stored in an <see cref="ICustomPropertiesProvider"/> implementation.
    /// </summary>
    internal class CustomPropertyDescriptor : PropertyDescriptor
    {
        #region Nested classes

        #region PickValueConverter class

        private class PickValueConverter : TypeConverter
        {
            #region Fields

            private readonly TypeConverter wrappedConverter;
            private readonly object?[] allowedValues;

            #endregion

            #region Constructors

            internal PickValueConverter(TypeConverter converter, object?[] allowedValues)
            {
                wrappedConverter = converter;
                this.allowedValues = allowedValues;
            }

            #endregion

            #region Methods

            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => wrappedConverter.CanConvertFrom(context, sourceType);
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => wrappedConverter.CanConvertTo(context, destinationType);
            public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) => wrappedConverter.ConvertFrom(context, culture, value);
            public override object? ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) => wrappedConverter.ConvertTo(context, culture, value, destinationType);
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => true;
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) => new StandardValuesCollection(allowedValues);

            #endregion
        }

        #endregion

        #endregion

        #region Fields

        private readonly HashSet<Attribute> attributes;

        private AttributeCollection? cachedAttributes;
        private PickValueConverter? converter;
        private Type? editor;

        #endregion

        #region Properties

        #region Public Properties

        public override AttributeCollection Attributes
        {
            get
            {
                if (cachedAttributes != null)
                    return cachedAttributes;
                var result = new HashSet<Attribute>(attributes);
                return cachedAttributes = new AttributeCollection(result.ToArray());
            }
        }

        public override TypeConverter? Converter => AllowedValues == null || base.Converter == null ? base.Converter : converter ??= new PickValueConverter(base.Converter, AllowedValues);
        public override Type ComponentType => typeof(ICustomPropertiesProvider);
        public override bool IsReadOnly => false;
        public override Type PropertyType { get; }

        #endregion

        #region Internal Properties

        internal new string? Category
        {
            get => base.Category;
            set
            {
                cachedAttributes = null;
                if (value != null)
                    attributes.Add(new CategoryAttribute(value));
            }
        }

        internal new string? Description
        {
            get => base.Description;
            set
            {
                cachedAttributes = null;
                if (value != null)
                    attributes.Add(new DescriptionAttribute(value));
            }
        }

        internal Type? UITypeEditor
        {
            get => editor ??= GetEditor(typeof(UITypeEditor))?.GetType();
            set
            {
                if (value == editor)
                    return;
                cachedAttributes = null;
                editor = value;
                if (value != null)
                    attributes.Add(new EditorAttribute(value, typeof(UITypeEditor)));
            }
        }

        internal object? DefaultValue { get; set; }
        internal object?[]? AllowedValues { get; set; }
        internal Func<object?, object?>? AdjustValue { get; set; }

        #endregion

        #endregion

        #region Constructors

        public CustomPropertyDescriptor(string name, Type type)
            : base(name, null)
        {
            PropertyType = type;
            attributes = new HashSet<Attribute>(TypeDescriptor.GetAttributes(type).Cast<Attribute>());
        }

        public CustomPropertyDescriptor(CustomPropertyDescriptor other) : base(other)
        {
            PropertyType = other.PropertyType;
            attributes = other.attributes;
        }

        #endregion

        #region Methods

        #region Public Methods
        
        public override bool ShouldSerializeValue(object component) => !Equals(GetValue(component), DefaultValue);
        public override bool CanResetValue(object component) => ShouldSerializeValue(component);
        public override void ResetValue(object component) => ((ICustomPropertiesProvider)component).ResetValue(Name);
        public override void SetValue(object component, object value) => ((ICustomPropertiesProvider)component).SetValue(Name, DoAdjustValue(value));
        public override object? GetValue(object component) => DoAdjustValue(((ICustomPropertiesProvider)component).GetValue(Name, DefaultValue));
        public override string ToString() => $"{Name}: {PropertyType}";

        #endregion

        #region Private Methods

        private object? DoAdjustValue(object? value)
            => AdjustValue != null ? AdjustValue.Invoke(value)
                : !AllowedValues.IsNullOrEmpty() && !value.In(AllowedValues) ? AllowedValues![0]
                : value == null && PropertyType.IsValueType ? DefaultValue ?? Activator.CreateInstance(PropertyType)
                : value;

        #endregion

        #endregion
    }
}
