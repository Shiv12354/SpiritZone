using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace SZInfrastructure
{

    public class InMemoryCache : ICacheService
    {
        public IList<T> GetOrSet<T>(string cacheKey, Func<IList<T>> getItemCallback) where T : class
        {
            IList<T> item = MemoryCache.Default.Get(cacheKey) as IList<T>;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(1));
            }
            return item;
        }
    }

    interface ICacheService
    {
        IList<T> GetOrSet<T>(string cacheKey, Func<IList<T>> getItemCallback) where T : class;
    }
}
