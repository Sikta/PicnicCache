//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public interface ICache<TKey, TValue> : ICacheBase<TKey, TValue> where TValue : class, new()
    {
        /// <summary>
        /// Fetch item by key.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <param name="del">Func to get the item if it's not already in the cache.</param>
        /// <returns>Item</returns>
        TValue Fetch(TKey key, Func<TValue> del);

        /// <summary>
        /// Fetch all items.
        /// </summary>
        /// <param name="del">Func to get all items if they have not already been cached.</param>
        /// <returns>Items</returns>
        IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del);

        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        /// <param name="saveDelegate">Action to update, add and delete all CacheItems.</param>
        void Save(Action<IEnumerable<ICacheItem<TValue>>> saveDelegate);

        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        /// <param name="updateDelegate">Action to update the cached items</param>
        /// <param name="addDelegate">Action to add the cached items</param>
        /// <param name="deleteDelegate">Action to delete the cached items</param>
        void Save(Action<IEnumerable<TValue>> updateDelegate, Action<IEnumerable<TValue>> addDelegate, Action<IEnumerable<TValue>> deleteDelegate);

        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        /// <param name="updateAddDelegate">Action to update and add the cached items.</param>
        /// <param name="deleteDelegate">Action to delete the cached items.</param>
        void Save(Action<IEnumerable<TValue>> updateAddDelegate, Action<IEnumerable<TValue>> deleteDelegate);
    }
}