#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DownloadableResourceItemCollection.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System.Collections.Generic;
using System.ComponentModel;

using KGySoft.Collections;
using KGySoft.ComponentModel;
using KGySoft.Drawing.ImagingTools.Model;

#endregion

namespace KGySoft.Drawing.ImagingTools.ViewModel
{
    internal class DownloadableResourceItemCollection : SortableBindingList<DownloadableResourceItem>
    {
        #region Fields

        private readonly StringKeyedDictionary<List<DownloadableResourceItem>> langGroups;

        #endregion

        #region Constructors

        internal DownloadableResourceItemCollection(ICollection<LocalizationInfo> collection) : base(new List<DownloadableResourceItem>(collection.Count))
        {
            langGroups = new StringKeyedDictionary<List<DownloadableResourceItem>>();
            foreach (LocalizationInfo info in collection)
            {
                string langKey = info.Language.Name;
                var item = new DownloadableResourceItem(info);
                item.PropertyChanged += Item_PropertyChanged;

                if (langGroups.TryGetValue(langKey, out List<DownloadableResourceItem>? group))
                    group.Add(item);
                else
                    langGroups[langKey] = new List<DownloadableResourceItem>(1) { item };

                Add(item);
            }

            ApplySort(nameof(DownloadableResourceItem.Language), ListSortDirection.Ascending);
        }

        #endregion

        #region Methods

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var changedItem = (DownloadableResourceItem)sender;
            if (e.PropertyName != nameof(DownloadableResourceItem.Selected) || !changedItem.Selected)
                return;

            //  deselecting other items of the same language as if they belonged to the same radio group
            foreach (DownloadableResourceItem item in langGroups[((DownloadableResourceItem)sender).CultureName])
            {
                if (item != sender && item.Selected)
                    item.Selected = false;
            }
        }

        #endregion
    }
}
