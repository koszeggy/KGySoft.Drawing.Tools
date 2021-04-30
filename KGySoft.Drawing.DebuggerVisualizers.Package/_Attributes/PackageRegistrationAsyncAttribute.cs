#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PackageRegistrationAsyncAttribute.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
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

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    /// <summary>
    /// Same as <c>[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]</c>
    /// in VS2015 and above. Required to support AllowsBackgroundLoading in VS2013.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class PackageRegistrationAsyncAttribute : RegistrationAttribute
    {
        #region Methods

        #region Static Methods

        private static string RegKeyName(RegistrationContext context) => $"Packages\\{context.ComponentType.GUID:B}";

        #endregion

        #region Instance Methods

        public override void Register(RegistrationContext context)
        {
            Type t = context.ComponentType;
            Key? packageKey = null;
            try
            {
                packageKey = context.CreateKey(RegKeyName(context));

                //use a friendly description if it exists.
                if (TypeDescriptor.GetAttributes(t)[typeof(DescriptionAttribute)] is DescriptionAttribute attr && !String.IsNullOrEmpty(attr.Description))
                    packageKey.SetValue(String.Empty, attr.Description);
                else
                    packageKey.SetValue(String.Empty, t.Name);

                packageKey.SetValue("InprocServer32", context.InprocServerPath);
                packageKey.SetValue("Class", t.FullName);
                packageKey.SetValue("Assembly", t.Assembly.FullName);
                packageKey.SetValue("AllowsBackgroundLoad", true);

                if (typeof(IVsPackageDynamicToolOwner).IsAssignableFrom(context.ComponentType) ||
                    typeof(IVsPackageDynamicToolOwnerEx).IsAssignableFrom(context.ComponentType))
                {
                    packageKey.SetValue("SupportsDynamicToolOwner", Microsoft.VisualStudio.PlatformUI.Boxes.BooleanTrue);
                }
            }
            finally
            {
                packageKey?.Close();
            }
        }

        public override void Unregister(RegistrationContext context) => context.RemoveKey(RegKeyName(context));

        #endregion

        #endregion
    }
}
