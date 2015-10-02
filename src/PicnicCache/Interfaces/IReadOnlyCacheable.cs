//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System.Collections.Generic;

namespace PicnicCache
{
    public interface IReadOnlyCacheable<TKey, TValue> 
        where TValue : class
    {
        /// <summary>
        /// Fetch item by key.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>Item</returns>
        TValue Fetch(TKey key);

        /// <summary>
        /// Fetch all items.
        /// </summary>
        /// <returns>Items</returns>
        IEnumerable<TValue> FetchAll();
    }
}