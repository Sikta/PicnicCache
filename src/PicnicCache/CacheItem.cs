//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache
{
    public class CacheItem<TValue> : ICacheItem<TValue> where TValue : class, new()
    {
        public TValue Item { get; set; }

        public CacheItemState Status { get; set; }

        internal CacheItem(TValue value, CacheItemState status)
        {
            Item = value;
            Status = status;
        }
    }

    public enum CacheItemState
    {
        Added,
        Deleted,
        Modified,
        NoTracking,
        Unmodified,
    }
}