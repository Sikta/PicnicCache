//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public interface ICache<TKey, TValue> where TValue : class, new()
    {
        /// <summary>
        /// Clear the cache.
        /// </summary>
        void ClearAll();

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
        /// Remove the item from the cache by key.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Remove(TKey key);

        /// <summary>
        /// Remove the item from the cache by value.
        /// </summary>
        /// <param name="value">Item Key</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Remove(TValue value);

        /// <summary>
        /// Remove the item from the cache by key and delete it.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <param name="del">Action to delete the item.</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TKey key, Action del);

        /// <summary>
        /// Remove the item from the cache by key and delete it.
        /// </summary>
        /// <param name="key">Item Key</param>
        /// <param name="del">Action to delete the item by key.</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TKey key, Action<TKey> del);

        /// <summary>
        /// Remove the item from the cache and delete it.
        /// </summary>
        /// <param name="value">Item</param>
        /// <param name="del">Action to delete the item.</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TValue value, Action del);

        /// <summary>
        /// Remove the item from the cache and delete it.
        /// </summary>
        /// <param name="value">Item</param>
        /// <param name="del">Action to delete the item by value.</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TValue value, Action<TValue> del);

        /// <summary>
        /// Remove the item from the cache and delete it.
        /// </summary>
        /// <param name="value">Item</param>
        /// <param name="del">Action to delete the item by key.</param>
        /// <returns>True if item was in cache otherwise false.</returns>
        bool Delete(TValue value, Action<TKey> del);

        /// <summary>
        /// Update the cached item or add it to the cache and save it.
        /// </summary>
        /// <param name="value">Value to update or add and save.</param>
        /// <param name="del">Action to save the item.</param>
        void Save(TValue value, Action del);

        /// <summary>
        /// Update the cached item or add it to the cache and save it.
        /// </summary>
        /// <param name="value">Value to update or add and save.</param>
        /// <param name="del">Action to save the item.</param>
        void Save(TValue value, Action<TValue> del);
        
        /// <summary>
        /// Save all items from the cache.
        /// </summary>
        /// <param name="del">Action to save all cached items.</param>
        void SaveAll(Action<IEnumerable<TValue>> del);
        
        /// <summary>
        /// Update the cached item or add it to the cache and save all items in the cache.
        /// </summary>
        /// <param name="value">Value to update or add.</param>
        /// <param name="del">Action to save all items in the cache.</param>
        void SaveAll(TValue value, Action<IEnumerable<TValue>> del);

        /// <summary>
        /// Update the cached items or add them to the cache and save all items in the cache.
        /// </summary>
        /// <param name="values">Values to update or add.</param>
        /// <param name="del">Action to save all items in the cache.</param>
        void SaveAll(IEnumerable<TValue> values, Action<IEnumerable<TValue>> del);

        /// <summary>
        /// Update the cached item or add it to the cache.
        /// </summary>
        /// <param name="value">Value to update or add.</param>
        void UpdateCache(TValue value);

        /// <summary>
        /// Update the cached items or add them to the cache.
        /// </summary>
        /// <param name="values">Values to update or add.</param>
        void UpdateCache(IEnumerable<TValue> values);
    }
}