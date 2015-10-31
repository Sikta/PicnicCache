//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public class PicnicCacheMapping<TKey, TValue> : ReadOnlyPicnicCacheMapping<TKey, TValue>, ICacheable<TKey, TValue> 
        where TValue : class
    {
        #region Fields

        private Action<IEnumerable<TValue>, IEnumerable<TValue>, IEnumerable<TValue>> _save;

        #endregion

        #region Constructors

        public PicnicCacheMapping(Func<TKey, TValue> fetch, Func<IEnumerable<TValue>> fetchAll,
                                  Action<IEnumerable<TValue>, IEnumerable<TValue>, IEnumerable<TValue>> save)
            : base(fetch, fetchAll)
        {
            ValidateParameterIsNotNull(save, "save");

            _save = save;
        }

        #endregion

        #region ICacheable

        public void Save(IEnumerable<TValue> addedItems, IEnumerable<TValue> deletedItems, IEnumerable<TValue> modifiedItems)
        { 
            _save(addedItems, deletedItems, modifiedItems);
        }

        #endregion
    }
}