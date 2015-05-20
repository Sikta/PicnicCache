//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public interface ICacheConfiguration<TKey, TValue> where TValue : class, new()
    {
        //TODO: Change to Func<TValue, TKey>
        Func<TKey, TValue> FetchMethod { get; }
        Func<IEnumerable<TValue>> FetchAllMethod { get; }
        Action<IEnumerable<ICacheItem<TValue>>> SaveMethod { get; }
    }
}