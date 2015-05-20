//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache
{
    public interface ICacheItem<TValue> where TValue : class, new()
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