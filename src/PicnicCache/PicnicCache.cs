//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PicnicCache
{
    public class PicnicCache<TKey, TValue> : PicnicCacheBase<TKey, TValue>, ICache<TKey, TValue> where TValue : class, new()
    {
        #region Constructors

        public PicnicCache(string keyPropertyName)
            : base(keyPropertyName)
        {
        }

        public PicnicCache(Func<TValue, TKey> keyProperty)
            : base(keyProperty)
        {
        }

        #endregion

        #region ICache Implementation
        
        public TValue Fetch(TKey key, Func<TValue> del)
        {
            ValidateParameterIsNotNull(del, "del");
            ValidateParameterIsNotNull(key, "key");

            return FetchInternal(key, del);
        }

        public IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del)
        {
            ValidateParameterIsNotNull(del, "del");

            return FetchAllInternal(del);
        }

        public void Save(Action<IEnumerable<ICacheItem<TValue>>> saveDelegate)
        {
            ValidateParameterIsNotNull(saveDelegate, "saveDelegate");

            saveDelegate(_dictionary.Values);

            ResetCacheItemStatus();
        }
        
        public void Save(Action<IEnumerable<TValue>> updateAddDelegate,
                         Action<IEnumerable<TValue>> deleteDelegate)
        {
            ValidateParameterIsNotNull(updateAddDelegate, "updateAddDelegate");
            ValidateParameterIsNotNull(deleteDelegate, "deleteDelegate");

            var modifiedItems = _dictionary.Values
                                    .Where(x => x.Status == CacheItemState.Modified || x.Status == CacheItemState.Added)
                                    .Select(x => x.Item);
            var deletedItems = _dictionary.Values
                                   .Where(x => x.Status == CacheItemState.Deleted)
                                   .Select(x => x.Item);
            updateAddDelegate(modifiedItems);
            deleteDelegate(deletedItems);

            ResetCacheItemStatus();
        }

        public void Save(Action<IEnumerable<TValue>> updateDelegate, 
                         Action<IEnumerable<TValue>> addDelegate, 
                         Action<IEnumerable<TValue>> deleteDelegate)
        {
            ValidateParameterIsNotNull(updateDelegate, "updateDelegate");
            ValidateParameterIsNotNull(addDelegate, "addDelegate");
            ValidateParameterIsNotNull(deleteDelegate, "deleteDelegate");

            var addedItems = _dictionary.Values
                                 .Where(x => x.Status == CacheItemState.Added)
                                 .Select(x => x.Item);
            var deletedItems = _dictionary.Values
                                   .Where(x => x.Status == CacheItemState.Deleted)
                                   .Select(x => x.Item);
            var modifiedItems = _dictionary.Values
                                    .Where(x => x.Status == CacheItemState.Modified)
                                    .Select(x => x.Item);

            addDelegate(addedItems);
            deleteDelegate(deletedItems);
            updateDelegate(modifiedItems);

            ResetCacheItemStatus();
        }

        #endregion
    }
}