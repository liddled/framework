using System;

namespace DL.Framework.Common
{
    public interface IItemKey
    {
        /// <summary>
        /// This key is the primary key and should not be amendable
        /// </summary>
        IComparable Key { get; }
    }
}
