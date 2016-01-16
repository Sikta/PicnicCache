//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System.Collections.Generic;

namespace PicnicCache
{
    public interface ICacheable<TKey, TValue> : IReadOnlyCacheable<TKey, TValue> 
        where TValue : class
    {
        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        void Save(IEnumerable<TValue> addedItems, IEnumerable<TValue> deletedItems, IEnumerable<TValue> modifiedItems);
    }
}