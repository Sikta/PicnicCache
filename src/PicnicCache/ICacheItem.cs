using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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