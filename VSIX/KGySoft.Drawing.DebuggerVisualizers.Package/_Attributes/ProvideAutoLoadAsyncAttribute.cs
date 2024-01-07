#if !VS2022_OR_GREATER
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ProvideAutoLoadAsyncAttribute.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2024 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

#endregion

#nullable enable

namespace KGySoft.Drawing.DebuggerVisualizers.Package
{
    /// <summary>
    /// Same as <c>[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]</c>
    /// in VS2015 and above. Required to support BackgroundLoad in VS2013.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class ProvideAutoLoadAsyncAttribute : RegistrationAttribute
    {
        #region Constants

        private const int backgroundLoad = 0x2;

        #endregion

        #region Properties

        private static string RegKeyName => $"AutoLoadPackages\\{new Guid(VSConstants.UICONTEXT.NoSolution_string):B}";

        #endregion

        #region Methods

        public override void Register(RegistrationContext context)
        {
            using Key childKey = context.CreateKey(RegKeyName);
            childKey.SetValue(context.ComponentType.GUID.ToString("B"), backgroundLoad);
        }

        public override void Unregister(RegistrationContext context) => context.RemoveValue(RegKeyName, context.ComponentType.GUID.ToString("B"));

        #endregion
    }
}
#endif