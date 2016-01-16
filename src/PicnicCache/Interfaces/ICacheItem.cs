//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache
{
    internal interface ICacheItem<TValue> where TValue : class
    {
        /// <summary>
        /// Wrapped item.
        /// </summary>
        TValue Item { get; set; }

        /// <summary>
        /// ItemState (Unmodified, Modified, Added, Deleted)
        /// </summary>
        CacheItemState Status { get; set; }
    }
}