using System;

namespace DL.Framework.Common
{
    [Serializable]
	public abstract class DomainObject<T> : IComparable, IItemKey where T : class, IItemKey
	{
        public Status Status { get; set; }
        public abstract IComparable Key { get; }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override bool Equals(object obj)
        {
            var item = obj as IItemKey;
            return item != null && Key.Equals(item.Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public virtual int CompareTo(object obj)
        {
            var item = obj as IItemKey;
            if (item == null) return -1;
            return Key.CompareTo(item.Key);
        }
    }
}
