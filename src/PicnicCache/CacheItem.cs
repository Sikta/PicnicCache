using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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