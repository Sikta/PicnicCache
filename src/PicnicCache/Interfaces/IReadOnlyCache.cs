//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicnicCache
{
    public interface IReadOnlyCache<TKey, TValue> where TValue : class
    {
        /// <summary>
        /// Clear the cache.
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Fetch item by key.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>Item</returns>
        TValue Fetch(TKey key);

        /// <summary>
        /// Fetch item by key async.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>Item</returns>
        Task<TValue> FetchAsync(TKey key);

        /// <summary>
        /// Fetch all items.
        /// </summary>
        /// <returns>Items</returns>
        IEnumerable<TValue> FetchAll();

        /// <summary>
        /// Fetch all items async.
        /// </summary>
        /// <returns>Items</returns>
        Task<IEnumerable<TValue>> FetchAllAsync();
    }
}