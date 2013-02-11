using System;
using System.Collections.Generic;

namespace DL.Framework.Common
{
    public interface IDomainList<T> : ICollection<T>
    {
        T this[IComparable key] { get; set; }
        T GetAt(int index);
        bool Contains(IComparable key);
        bool TryGetValue(IComparable key, out T item);
        bool Remove(IComparable key);
    }
}
