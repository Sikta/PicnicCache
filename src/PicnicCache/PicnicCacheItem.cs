//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

namespace PicnicCache
{
    internal class PicnicCacheItem<TValue> : ICacheItem<TValue> 
        where TValue : class
    {
        #region Properties

        public TValue Item { get; set; }

        public CacheItemState Status { get; set; }

        #endregion

        #region Constructors

        internal PicnicCacheItem(TValue value, CacheItemState status)
        {
            Item = value;
            Status = status;
        }

        #endregion
    }

    internal enum CacheItemState
    {
        Added,
        Deleted,
        Modified,
        Unmodified,
    }
}