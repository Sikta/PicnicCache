//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public class ReadOnlyPicnicCacheMapping<TKey, TValue> : PicnicCacheBaseObject, IReadOnlyCacheable<TKey, TValue>
        where TValue : class
    {
        #region Fields

        private Func<TKey, TValue> _fetch;
        private Func<IEnumerable<TValue>> _fetchAll;

        #endregion

        #region Constructors

        public ReadOnlyPicnicCacheMapping(Func<TKey, TValue> fetch, Func<IEnumerable<TValue>> fetchAll)
        {
            ValidateParameterIsNotNull(fetch, "fetch");
            ValidateParameterIsNotNull(fetchAll, "fetchAll");

            _fetch = fetch;
            _fetchAll = fetchAll;
        }

        #endregion

        #region IReadOnlyCacheable

        public TValue Fetch(TKey key)
        {
            return _fetch.Invoke(key);
        }

        public IEnumerable<TValue> FetchAll()
        {
            return _fetchAll.Invoke();
        }

        #endregion
    }
}