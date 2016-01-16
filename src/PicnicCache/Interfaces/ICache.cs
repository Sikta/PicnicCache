//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicnicCache
{
    public interface ICache<TKey, TValue> : IReadOnlyCache<TKey, TValue> where TValue : class
    {
        /// <summary>
        /// Add item to the cache.
        /// </summary>
        /// <param name="value"></param>
        void Add(TValue value);

        /// <summary>
        /// Set the item state to deleted.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TKey key);

        /// <summary>
        /// Set the item state to deleted by value.
        /// </summary>
        /// <param name="value">Item Key</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TValue value);

        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        void Save();

        /// <summary>
        /// Save all items from the cache async.
        /// </summary>
        Task SaveAsync();

        /// <summary>
        /// Update the cached item.
        /// </summary>
        /// <param name="value">Value to update or add.</param>
        void Update(TValue value);

        /// <summary>
        /// Update the cached item from a dto.
        /// </summary>
        /// <typeparam name="T">DTO Type</typeparam>
        /// <param name="key">Item Key</param>
        /// <param name="dto">DTO to update the cache item.</param>
        /// <param name="updateCacheItemMethod">Method to update cached item.</param>
        void Update<T>(TKey key, T dto, Func<T, TValue, TValue> updateCacheItemMethod);

        /// <summary>
        /// Update the cached items.
        /// </summary>
        /// <param name="values">Values to update or add.</param>
        void Update(IEnumerable<TValue> values);
    }
}