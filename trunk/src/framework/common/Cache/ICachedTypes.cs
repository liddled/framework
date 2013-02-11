using System;
using System.Collections.Generic;

namespace DL.Framework.Common
{
    public interface ICachedTypes
    {
        IList<IGenericCache> GetAllCaches();
        GenericCache<T> GetCache<T>() where T : class, IItemKey;
    }
}
