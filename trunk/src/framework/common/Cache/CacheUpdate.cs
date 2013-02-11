using System;
using System.Collections.Generic;

namespace DL.Framework.Common
{
    [Serializable]
    public class CacheUpdateItem
    {
        public IItemKey UpdatedItem { get; set; }
        public IItemKey OldItem { get; set; }

        /*public bool IsRequired(Delegate predicate)
        {
            return (bool)predicate.DynamicInvoke(UpdatedItem);
        }*/

        public override string ToString()
        {
            return UpdatedItem == null ? base.ToString() : UpdatedItem.Key.ToString();
        }
    }

    [Serializable]
    public class CacheUpdate
    {
        public List<CacheUpdateItem> UpdatedItems { get; private set; }
        public EventAction Reason { get; set; }
        public bool LastInSequence { get; set; }
        public DateTime UpdatedTime { get; set; }

        public CacheUpdate()
        {
            UpdatedItems = new List<CacheUpdateItem>();
            Reason = EventAction.None;
            LastInSequence = true;
            UpdatedTime = DateTime.UtcNow;
        }

        public CacheUpdate(IItemKey updatedItem, EventAction reason)
        {
            UpdatedItems = new List<CacheUpdateItem> { new CacheUpdateItem { UpdatedItem = updatedItem } };
            Reason = reason;
            LastInSequence = true;
            UpdatedTime = DateTime.UtcNow;
        }

        public CacheUpdate(IItemKey updatedItem, IItemKey oldItem, EventAction reason)
        {
            UpdatedItems = new List<CacheUpdateItem> { new CacheUpdateItem { UpdatedItem = updatedItem, OldItem = oldItem } };
            Reason = reason;
            LastInSequence = true;
            UpdatedTime = DateTime.UtcNow;
        }

        public CacheUpdate(IList<IItemKey> updatedItems, EventAction reason)
        {
            UpdatedItems = new List<CacheUpdateItem>();
            foreach (var ui in updatedItems)
                UpdatedItems.Add(new CacheUpdateItem { UpdatedItem = ui });
            Reason = reason;
            LastInSequence = true;
            UpdatedTime = DateTime.UtcNow;
        }
    }
}
