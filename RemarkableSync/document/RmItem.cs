using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RemarkableSync.document
{
    public class RmItem
    {
        public static string CollectionType = "CollectionType";
        public static string DocumentType = "DocumentType";

        public RmItem()
        {
            Children = new List<RmItem>();
        }

        public string ID { get; set; }
        public int Version { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string BlobURLGet { get; set; }
        public DateTime BlobURLGetExpires { get; set; }
        public DateTime ModifiedClient { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastOpened { get; set; }
        public string Type { get; set; }
        public string VissibleName { get; set; }
        public int CurrentPage { get; set; }
        public bool Bookmarked { get; set; }
        public string Parent { get; set; }

        public List<RmItem> Children { get; set; }

        public static List<RmItem> SortItems(List<RmItem> collection)
        {
            foreach (RmItem item in collection)
            {
                if (item.Children.Count > 0)
                {
                    item.Children = SortItems(item.Children);
                }
            }

            collection.Sort(
                delegate (RmItem p1, RmItem p2)
                {
                    int compareType = p1.Type.CompareTo(p2.Type);
                    if (compareType == 0)
                    {
                        return p1.VissibleName.CompareTo(p2.VissibleName);
                    }
                    return compareType;
                }
            );
            return collection;
        }

        public static List<RmItem> SortItemsLastModified(List<RmItem> collection)
        {
            List<RmItem> expandedList = ExpandCollection(collection, "/");

            expandedList.Sort(
                delegate (RmItem p1, RmItem p2)
                {
                    int compareType = p2.LastModified.CompareTo(p1.LastModified);
                    if (compareType == 0)
                    {
                        return p1.VissibleName.CompareTo(p2.VissibleName);
                    }
                    return compareType;
                }
            );
            return expandedList;
        }

        private static List<RmItem> ExpandCollection(List<RmItem> collection, String parent)
        {
            List<RmItem> expandedList = new List<RmItem >();

            foreach (RmItem item in collection)
            {
                item.Parent = parent;
                if (item.Children.Count > 0)
                {
                    expandedList.AddRange(ExpandCollection(item.Children, item.VissibleName + "/"));
                }
                else
                {
                    expandedList.Add(item);
                }
            }
            return expandedList;
        }
    }
}



