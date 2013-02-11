using System;
using System.Collections.Generic;

namespace DL.Framework.Common
{
    public class CachedTypes : ICachedTypes
    {
        private readonly IDictionary<Type, object> _cachedTypes = new Dictionary<Type, object>();

        private readonly object _lock = new object();

        public CachedTypes(IEnumerable<IGenericCache> caches)
        {
            foreach (var cache in caches)
            {
                if (cache.GetType().IsGenericType && cache.GetType().GetGenericTypeDefinition() == typeof(GenericCache<>))
                {
                    _cachedTypes.Add(cache.GetType().GetGenericArguments()[0], cache);
                }
                else
                {
                    throw new Exception("Type " + cache.GetType().Name + " not handled for cache");
                }
            }
        }

        public IList<IGenericCache> GetAllCaches()
        {
            var cacheList = new List<IGenericCache>();
            lock (_lock)
            {
                foreach (var type in _cachedTypes)
                {
                    cacheList.Add((IGenericCache)type.Value);
                }
            }
            return cacheList;
        }

        public GenericCache<T> GetCache<T>() where T : class, IItemKey
        {
            object i;
            if (!_cachedTypes.TryGetValue(typeof(T), out i))
            {
                throw new Exception("Type is not cached " + typeof(T).Name);
            }
            return (GenericCache<T>)i;
        }
    }
}