using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using Microsoft.VisualStudio.Extensibility.UI;

//using KGySoft.ComponentModel;

namespace KGySoft.Drawing.DebuggerVisualizers.Extension
{
    //[DataContract]
    //internal class ImageData : ObservableObjectBase
    //{
    //    [DataMember]public string? Info { get => Get<string>(); set => Set(value); }
    //    [DataMember]public BitmapSource? Image { get => Get<BitmapSource>(); set => Set(value); }
    //}
    [DataContract]
    internal class ImageData : NotifyPropertyChangedObject
    {
        private string? info;
        private BitmapSource? image;


        [DataMember]
        public string? Info
        {
            get => info;
            set => SetProperty(ref info, value);
        }

        [DataMember]
        public BitmapSource? Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }
    }
}
