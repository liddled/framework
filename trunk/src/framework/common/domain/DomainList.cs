using System;
using System.Collections;
using System.Collections.Generic;

namespace DL.Framework.Common
{
    public class DomainList<T> : IDomainList<T> where T : class, IItemKey
    {
        private readonly IDictionary<IComparable, T> _items;
        private readonly IList<IComparable> _keys;

        public DomainList()
        {
            _items = new Dictionary<IComparable, T>();
            _keys = new List<IComparable>();
        }

        public DomainList(IEnumerable<T> items)
        {
            _items = new Dictionary<IComparable, T>();
            _keys = new List<IComparable>();

            foreach (var i in items)
                Add(i);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            _items.Clear();
            _keys.Clear();
        }

        public bool Contains(IComparable key)
        {
            return _items.ContainsKey(key);
        }

        public bool Contains(T item)
        {
            return Contains(item.Key);
        }
        
        public bool TryGetValue(IComparable key, out T item)
        {
            return _items.TryGetValue(key, out item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
			_items.Values.CopyTo(array, arrayIndex);
        }

        public T this[IComparable key]
        {
            get { return _items[key]; }
            set
            {
                lock (this)
                {
                    if (_items.ContainsKey(key))
                    {
                        _items[key] = value;
                    }
                    else
                    {
                        _items.Add(key, value);
                        _keys.Add(key);
                    }
                }
            }
        }

        public T GetAt(int index)
        {
            lock (this)
            {
                return _items[_keys[index]];
            }
        }

        public void Add(T item)
        {
            lock (this)
            {
                AddNoLock(item);
            }
        }

        private void AddNoLock(T item)
        {
            var key = item.Key;
            _items.Add(key, item);
            _keys.Add(key);
        }

        public bool Remove(T item)
        {
            return Remove(item.Key);
        }

        public bool Remove(IComparable key)
        {
            lock (this)
            {
                if (_items.ContainsKey(key))
                {
                    _items.Remove(key);
                    _keys.Remove(key);
                    return true;
                }
                return false;
            }
        }

        public int Count
        {
			get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }
    }
}
