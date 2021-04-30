#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TestObjectProvider.cs
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using KGySoft.CoreLibraries;

using Microsoft.VisualStudio.DebuggerVisualizers;

#endregion

#region Suppressions

#if NET5_0_OR_GREATER
#pragma warning disable SYSLIB0011 // Type or member is obsolete - must use BinaryFormatter to be compatible with the MS implementation
#pragma warning disable IDE0079 // Remove unnecessary suppression - must use BinaryFormatter to be compatible with the MS implementation
#pragma warning disable CS0618 // Use of obsolete symbol - as above  
#endif

#endregion

namespace KGySoft.Drawing.DebuggerVisualizers.Test.ViewModel
{
    internal class TestObjectProvider : IVisualizerObjectProvider
    {
        #region Properties

        #region Public Properties

        public bool IsObjectReplaceable { get; set; }

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
            Object = newObject.DeepClone();
            ObjectReplaced = true;
        }

        public void ReplaceData(Stream newObjectData) => throw new NotImplementedException();
        public Stream TransferData(Stream outgoingData) => throw new NotImplementedException();
        public object TransferObject(object outgoingObject) => throw new NotImplementedException();

        #endregion
    }
}
