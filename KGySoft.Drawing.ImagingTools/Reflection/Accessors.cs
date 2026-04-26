#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Accessors.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2026 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NETFRAMEWORK
using System.Runtime.Serialization;
#endif
using System.Threading;
using System.Windows.Forms;

using KGySoft.Collections;
#if NETFRAMEWORK
using KGySoft.CoreLibraries;
#endif
using KGySoft.Reflection;
using KGySoft.WinForms;

#endregion

namespace KGySoft.Drawing.ImagingTools.Reflection
{
    // ReSharper disable InconsistentNaming
    internal static class Accessors
    {
        #region Fields

        // Property keys and lookup callbacks. Public flags are added to support possible future compatibility for originally non-visible properties.
        private static readonly object propDataGridView_MouseDownCellAddress = new();
        private static readonly object propDataGridView_CellMouseDownInContentBounds = new();
        private static readonly object propToolStrip_ToolTip = new();
        private static readonly Dictionary<object, Func<PropertyInfo?>> propertyLookup = new(3)
        {
            [propDataGridView_MouseDownCellAddress] = () => typeof(DataGridView).GetProperty("MouseDownCellAddress", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
            [propDataGridView_CellMouseDownInContentBounds] = () => typeof(DataGridView).GetProperty("CellMouseDownInContentBounds", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
            [propToolStrip_ToolTip] = () => typeof(ToolStrip).GetProperty("ToolTip", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
        };

        // Field keys and lookup callbacks. The DeclaredOnly flag is implicitly added by FindField.
        private static readonly LockFreeCacheOptions cacheOptions = new() { ThresholdCapacity = 16, HashingStrategy = HashingStrategy.And, MergeInterval = TimeSpan.FromMilliseconds(100) };
        private static readonly object fieldColorPalette_flags = new();
        private static readonly object fieldColorPalette_entries = new();
        private static readonly object fieldDataGridView_tooltipWindow = new();
        private static readonly object fieldDataGridView_tooltipControl = new();
        private static readonly object fieldDataGridView_tooltipControl_toolTip = new();
        private static readonly object fieldToolStrip_tooltipWindow = new();
        private static readonly Dictionary<object, Func<FieldInfo?>> fieldLookup = new(6)
        {
            [fieldColorPalette_flags] = () => FindField(typeof(ColorPalette), "flags", typeof(int), BindingFlags.Instance | BindingFlags.NonPublic),
            [fieldColorPalette_entries] = () => FindField(typeof(ColorPalette), "entries", typeof(Color[]), BindingFlags.Instance | BindingFlags.NonPublic),
            [fieldDataGridView_tooltipWindow] = () => FindField(typeof(DataGridView), "tooltip_window", typeof(ToolTip), BindingFlags.Instance | BindingFlags.NonPublic),
            [fieldDataGridView_tooltipControl] = () => FindField(typeof(DataGridView), "toolTipControl", null, BindingFlags.Instance | BindingFlags.NonPublic),
            [fieldDataGridView_tooltipControl_toolTip] = () => FindField(typeof(DataGridView).GetNestedType("DataGridViewToolTip", BindingFlags.Instance | BindingFlags.NonPublic), "toolTip", typeof(ToolTip), BindingFlags.Instance | BindingFlags.NonPublic),
            [fieldToolStrip_tooltipWindow] = () => FindField(typeof(ToolStrip), "tooltip_window", typeof(ToolTip), BindingFlags.Instance | BindingFlags.NonPublic),
        };

        private static IThreadSafeCacheAccessor<object, PropertyAccessor?>? properties;
        private static IThreadSafeCacheAccessor<object, FieldAccessor?>? fields;

        #endregion

        #region Methods

        #region Internal Methods

        #region ColorPalette

        internal static ColorPalette CreateColorPalette(Color[] entries, PaletteFlags flags)
        {
            // ColorPalette has an internal (int count) ctor in .NET Framework-.NET 8.0, which is missing in .NET 9+, where a public Color[] ctor exists in return
#if NET9_0_OR_GREATER
            var result = new ColorPalette(entries);
#else
            // Not using the (int count) ctor, because it may not exist in Mono, or when a consumer project references a different version of the System.Drawing.Common package.
#if NETFRAMEWORK
            var result = (ColorPalette)FormatterServices.GetUninitializedObject(typeof(ColorPalette));
#else
            var result = (ColorPalette)RuntimeHelpers.GetUninitializedObject(typeof(ColorPalette));
#endif
            TryGetField(fieldColorPalette_entries)?.SetInstanceValue(result, entries);
#endif

            TryGetField(fieldColorPalette_flags)?.SetInstanceValue(result, (int)flags);
            return result;
        }

        #endregion

        #region DataGridView

        internal static void TrySetToolTip(this DataGridView grid, Func<ToolTip> toolTipFactory)
        {
            // Framework Mono has a ToolTip tooltip_window field
            if (OSHelper.IsFrameworkMono)
            {
                if ((TryGetField(fieldDataGridView_tooltipWindow)) is not FieldAccessor field)
                    return;

                field.SetInstanceValue(grid, toolTipFactory.Invoke());
                return;
            }

            // Assuming Windows implementation here (including Wine Mono): DataGridView.*toolTipControl*.*toolTip*
            if (TryGetField(fieldDataGridView_tooltipControl) is not FieldAccessor fieldOuter || TryGetField(fieldDataGridView_tooltipControl_toolTip) is not FieldAccessor fieldInner)
                return;

            fieldInner.Set(fieldOuter.Get(grid), toolTipFactory.Invoke());
        }

        internal static ToolTip? TryGetToolTip(this DataGridView grid)
        {
            // Framework Mono has a ToolTip tooltip_window field
            if (OSHelper.IsFrameworkMono)
                return TryGetField(fieldDataGridView_tooltipWindow)?.GetInstanceValue<DataGridView, ToolTip>(grid);

            // Assuming Windows implementation here (including Wine Mono): DataGridView.*toolTipControl*.*toolTip*
            if (TryGetField(fieldDataGridView_tooltipControl) is not FieldAccessor fieldOuter || TryGetField(fieldDataGridView_tooltipControl_toolTip) is not FieldAccessor fieldInner)
                return null;

            return fieldInner.Get(fieldOuter.Get(grid)) as ToolTip;
        }

        internal static Point? TryGetMouseDownCellAddress(this DataGridView grid)
            => TryGetProperty(propDataGridView_MouseDownCellAddress)?.GetInstanceValue<DataGridView, Point>(grid);

        internal static bool? TryGetCellMouseDownInContentBounds(this DataGridView grid)
            => TryGetProperty(propDataGridView_CellMouseDownInContentBounds)?.GetInstanceValue<DataGridView, bool>(grid);


        #endregion

        #region ToolStrip

        internal static ToolTip? TryGetToolTip(this ToolStrip toolStrip) => OSHelper.IsFrameworkMono
            ? TryGetField(fieldToolStrip_tooltipWindow)?.GetInstanceValue<ToolStrip, ToolTip>(toolStrip)
            : TryGetProperty(propToolStrip_ToolTip)?.GetInstanceValue<ToolStrip, ToolTip>(toolStrip);

        #endregion

        #endregion

        #region Private Methods

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private static PropertyAccessor? TryGetProperty(object key)
        {
            #region Local Methods

            [MethodImpl(MethodImplOptions.NoInlining)]
            static PropertyAccessor? GetPropertyAccessor(object key)
            {
                if (!propertyLookup.TryGetValue(key, out var func))
                    throw new InvalidOperationException(Res.InternalError("GetPropertyAccessor: Property key found"));
                PropertyInfo? result = func.Invoke();
                return result is null ? null : PropertyAccessor.GetAccessor(result);
            }

            #endregion

            if (properties == null)
                Interlocked.CompareExchange(ref properties, ThreadSafeCacheFactory.Create<object, PropertyAccessor?>(GetPropertyAccessor, cacheOptions), null);
            return properties[key];
        }

        private static FieldInfo? FindField(Type? declaringType, string? namePattern, Type? fieldType, BindingFlags bindingFlags)
        {
            if (declaringType == null)
                return null;
            FieldInfo[] candidates = declaringType.GetFields(bindingFlags | BindingFlags.DeclaredOnly);
            return candidates.FirstOrDefault(f => (fieldType == null || f.FieldType == fieldType) && f.Name == namePattern) // exact name first
                ?? candidates.FirstOrDefault(f => (fieldType == null || f.FieldType == fieldType)
                        && (namePattern == null || f.Name.Contains(namePattern, StringComparison.OrdinalIgnoreCase)));
        }

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private static FieldAccessor? TryGetField(object key)
        {
            #region Local Methods

            [MethodImpl(MethodImplOptions.NoInlining)]
            static FieldAccessor? GetFieldAccessor(object key)
            {
                if (!fieldLookup.TryGetValue(key, out var func))
                    throw new InvalidOperationException(Res.InternalError("GetFieldAccessor: Field key found"));
                FieldInfo? result = func.Invoke();
                return result is null ? null : FieldAccessor.GetAccessor(result);
            }

            #endregion

            if (fields == null)
                Interlocked.CompareExchange(ref fields, ThreadSafeCacheFactory.Create<object, FieldAccessor?>(GetFieldAccessor, null, cacheOptions), null);
            return fields[key];
        }

        #endregion

        #endregion
    }
}
