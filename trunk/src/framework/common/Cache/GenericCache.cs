using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DL.Framework.Common
{
    public class GenericCache<T> : IGenericCache where T : class, IItemKey
    {
        private readonly IDomainList<T> _items;
        private readonly ReaderWriterLockSlim _lockList;

        private Action<CacheUpdate> _event;
        private IDictionary<Action<CacheUpdate>, FilteredEventHandler> _eventHandlers;
        private readonly object _lockEvent;

        public GenericCache()
        {
            _items = new DomainList<T>();
            _lockList = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            _eventHandlers = new Dictionary<Action<CacheUpdate>, FilteredEventHandler>();
            _lockEvent = new object();
        }

        public void AddEventHandler(Action<CacheUpdate> evt)
        {
            AddEventHandler(new FilteredEventHandler(evt, RemoveEventHandler));
        }

        public void AddEventHandler(FilteredEventHandler eh)
        {
            _lockList.EnterReadLock();
            try
            {
                lock (_lockEvent)
                {
                    if (_eventHandlers.ContainsKey(eh.Event))
                    {
                        var feh = _eventHandlers[eh.Event];
                        // change filter here ??
                    }
                    else
                    {
                        _eventHandlers.Add(eh.Event, eh);
                        _event += eh.Event;
                    }
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public void RemoveEventHandler(Action<CacheUpdate> evt)
        {
            _lockList.EnterReadLock();
            try
            {
                lock (_lockEvent)
                {
                    if (!_eventHandlers.ContainsKey(evt))
                        return;

                    _eventHandlers.Remove(evt);
                    _event -= evt;
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public IList<T> GetAllItems()
        {
            _lockList.EnterReadLock();
            try
            {
                return _items.ToList();
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public IList<T> GetFilteredList(Func<T, bool> predicate)
        {
            _lockList.EnterReadLock();
            try
            {
                return _items.AsParallel().Where(predicate).ToList();
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public IList<T> GetFilteredList<T1, TKey>(Func<T, bool> predicate, IEnumerable<T1> joinItems, Func<T, TKey> outerKeySelector, Func<T1, TKey> innerKeySelector, Func<T, T1, T> resultSelector)
        {
            _lockList.EnterReadLock();
            try
            {
                return _items.Join<T, T1, TKey, T>(joinItems, outerKeySelector, innerKeySelector, resultSelector).Where(predicate).ToList();
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public T GetItem(Func<T, bool> predicate)
        {
            _lockList.EnterReadLock();
            try
            {
                return _items.AsParallel().SingleOrDefault(predicate);
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public T GetItem(IComparable key)
        {
            _lockList.EnterReadLock();
            try
            {
                return UnsafeGetItem(key);
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        private T UnsafeGetItem(IComparable key)
        {
            T item = null;
            if (key != null)
                _items.TryGetValue(key, out item);
            return item;
        }

        public void AddItem(T item)
        {
            AddItem(item, EventAction.None);
        }

        public void AddItem(T item, EventAction overrideReason)
        {
            AddItems(new[] { item }, overrideReason);
        }

        public void AddItems(IEnumerable<T> itemsToAdd)
        {
            AddItems(itemsToAdd, EventAction.None);
        }

        public void AddItems(IEnumerable<T> itemsToAdd, EventAction overrideReason)
        {
            if (itemsToAdd == null)
                return;

            _lockList.EnterWriteLock();
            try
            {
                AddItemsNoLock(itemsToAdd, overrideReason);
            }
            finally
            {
                _lockList.ExitWriteLock();
            }
        }

        private void AddItemsNoLock(IEnumerable<T> itemsToAdd, EventAction overrideReason)
        {
            var notifications = new Dictionary<EventAction, CacheUpdate>();
            foreach (var item in itemsToAdd)
            {
                var cu = AddItemNoLock(item);
                if (overrideReason != EventAction.None)
                    cu.Reason = overrideReason;
                UpdateNotifications(notifications, cu);
            }
            Notify(notifications.Values);
        }

        private CacheUpdate AddItemNoLock(T item)
        {
            if (item.Key == null)
                throw new Exception("Item key cannot be null");

            var removedItem = RemoveItemByKey(item.Key);
            _items.Add(item);

            var reason = (removedItem != null) ? EventAction.Update : EventAction.Add;
            return new CacheUpdate(item, reason);
        }

        public void UpdateItem(T item)
        {
            UpdateItem(item, EventAction.None);
        }

        public void UpdateItem(T item, EventAction reasonOverride)
        {
            UpdateItems(new[] { item }, reasonOverride);
        }

        public void UpdateItems(IEnumerable<T> itemsToUpdate, EventAction reasonOverride)
        {
            if (itemsToUpdate == null)
                return;

            var notifications = new Dictionary<EventAction, CacheUpdate>();

            _lockList.EnterWriteLock();
            try
            {
                foreach (var i in itemsToUpdate)
                {
                    var originalCacheItem = UnsafeGetItem(i.Key);
                    if (originalCacheItem == null) continue;

                    _items[i.Key] = i;

                    var cu = new CacheUpdate(i, originalCacheItem, EventAction.Update);
                    if (reasonOverride != EventAction.None)
                        cu.Reason = reasonOverride;
                    UpdateNotifications(notifications, cu);
                }
                Notify(notifications.Values);
            }
            finally
            {
                _lockList.ExitWriteLock();
            }
        }

        public void UpdateItems<T1>(T1 updatedItem, Func<IDomainList<T>, IList<T1>> getUpdateItems)
        {
            _lockList.EnterReadLock();
            try
            {
                var existingItems = getUpdateItems(_items);

                for (int i = existingItems.Count() - 1; i >= 0; i--)
                {
                    existingItems[i] = updatedItem;
                }
            }
            finally
            {
                _lockList.ExitReadLock();
            }
        }

        public void RemoveItems(IEnumerable<T> items)
        {
            if (items == null)
                return;
            
            var itemsRemoved = new List<IItemKey>();
            _lockList.EnterWriteLock();
            try
            {
                itemsRemoved.AddRange(items.Select(i => i.Key).Select(RemoveItemByKey).Where(i => i != null));
            }
            finally
            {
                _lockList.ExitWriteLock();
            }
            if (itemsRemoved.Count > 0)
                Notify(new CacheUpdate(itemsRemoved, EventAction.Delete));
        }

        private T RemoveItemByKey(IComparable key)
        {
            if (_items.Contains(key))
            {
                T item = _items[key];
                _items.Remove(key);
                return item;
            }
            return null;
        }

        private static void UpdateNotifications(IDictionary<EventAction, CacheUpdate> notifications, CacheUpdate cu)
        {
            CacheUpdate cacheUpdate;
            if (notifications.TryGetValue(cu.Reason, out cacheUpdate))
            {
                cacheUpdate.UpdatedItems.AddRange(cu.UpdatedItems);
            }
            else
            {
                notifications.Add(cu.Reason, cu);
            }
        }

        private void Notify(IEnumerable<CacheUpdate> notificationList)
        {
            foreach (var cu in notificationList)
            {
                Notify(cu);
            }
        }

        protected void Notify(CacheUpdate item)
        {
            if (_event == null)
                return;

            var handlers = _event.GetInvocationList();
            
            if (handlers == null)
                return;

            foreach (Action<CacheUpdate> evt in handlers)
            {
                evt(item);
            }
        }
    }
}