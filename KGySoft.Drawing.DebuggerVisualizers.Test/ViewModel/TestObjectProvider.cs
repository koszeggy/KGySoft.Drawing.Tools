#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TestObjectProvider.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using KGySoft.CoreLibraries;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Suppressions

#if NET5_0_OR_GREATER
#pragma warning disable SYSLIB0011 // Type or member is obsolete - must use BinaryFormatter to be compatible with the MS implementation
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel
{
    internal class TestObjectProvider : IVisualizerObjectProvider
    {
        #region Properties

        #region Public Properties

        public bool IsObjectReplaceable { get; set; }
        public bool IsBinaryFormatterSupported => false;

        #endregion

        #region Internal Properties

        internal object Object { get; private set; }

        internal VisualizerObjectSource Serializer { get; set; } = default!;

        internal bool ObjectReplaced { get; private set; }

        #endregion

        #endregion

        #region Constructors

        public TestObjectProvider(object obj) => Object = obj;

        #endregion

        #region Methods

        public Stream GetData()
        {
            var ms = new MemoryStream();
            Serializer.GetData(Object, ms);
            ms.Position = 0;
            return ms;
        }

        public object GetObject()
        {
            using var ms = new MemoryStream();
            Serializer.GetData(Object, ms);
            ms.Position = 0;
            return new BinaryFormatter().Deserialize(ms);
        }

        public void ReplaceObject(object newObject)
        {
            Object = newObject.DeepClone(null);
            ObjectReplaced = true;
        }

        public void ReplaceData(Stream newObjectData)
        {
            newObjectData.Position = 0L;
            Object = Serializer.CreateReplacementObject(Object, newObjectData);
            ObjectReplaced = true;
        }

        public Stream TransferData(Stream outgoingData) => throw new NotImplementedException();
        public object TransferObject(object outgoingObject) => throw new NotImplementedException();

        #endregion
    }
}
