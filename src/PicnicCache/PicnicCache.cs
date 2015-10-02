//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicnicCache
{
    public class PicnicCache<TKey, TValue> : ReadOnlyPicnicCache<TKey, TValue>, ICache<TKey, TValue> 
        where TValue : class
    {
        #region Properties

        private ICacheable<TKey, TValue> Cacheable
        {
            get { return (ICacheable<TKey, TValue>)_readOnlyCacheable; }
            set { _readOnlyCacheable = value; }
        }

        #endregion

        #region Constructors

        protected PicnicCache(ICacheable<TKey, TValue> cacheable)
        {
            ValidateParameterIsNotNull(cacheable, "cacheable");
            Cacheable = cacheable;
        }

        public PicnicCache(string keyPropertyName, ICacheable<TKey, TValue> cacheable)
            : this(cacheable)
        {
            SetKeyProperty(keyPropertyName);
        }

        public PicnicCache(Func<TValue, TKey> keyProperty, ICacheable<TKey, TValue> cacheable)
            : this(cacheable)
        {
            ValidateParameterIsNotNull(keyProperty, "keyProperty");
            _keyProperty = keyProperty;
        }

        #endregion

        #region ICache

        public void Add(TValue value)
        {
            ValidateParameterIsNotNull(value, "value");

            lock (_lockObject)
            {
                _dictionary.Add(_keyProperty(value), new PicnicCacheItem<TValue>(value, CacheItemState.Added));
            }
        }

        public bool Delete(TKey key)
        {
            ValidateParameterIsNotNull(key, "key");

            bool isItemInCache = false;

            lock (_lockObject)
            {
                ICacheItem<TValue> cacheValue;
                if (_dictionary.TryGetValue(key, out cacheValue))
                {
                    if (cacheValue.Status == CacheItemState.Added)
                    {
                        _dictionary.Remove(key);
                    }
                    else
                    {
                        cacheValue.Status = CacheItemState.Deleted;
                    }
                    isItemInCache = true;
                }
            }

            return isItemInCache;
        }

        public bool Delete(TValue value)
        {
            ValidateParameterIsNotNull(value, "value");

            return Delete(_keyProperty(value));
        }

        public void Save()
        {
            var addedItems = GetCacheItemsByState(CacheItemState.Added);
            var deletedItems = GetCacheItemsByState(CacheItemState.Deleted);
            var modifiedItems = GetCacheItemsByState(CacheItemState.Modified);

            Cacheable.Save(addedItems, deletedItems, modifiedItems);
            ResetCacheItemStatus();
        }

        public Task SaveAsync()
        {
            return Task.Factory.StartNew(() => Save());
        }
        
        public void Update(TValue value)
        {
            ValidateParameterIsNotNull(value, "value");

            lock (_lockObject)
            {
                UpdateValue(_keyProperty(value), value);
            }
        }

        public void Update<T>(TKey key, T dto, Func<T, TValue, TValue> updateCacheItemMethod)
        {
            ValidateParameterIsNotNull(key, "key");
            ValidateParameterIsNotNull(dto, "dto");
            ValidateParameterIsNotNull(updateCacheItemMethod, "updateCacheItemMethod");

            lock (_lockObject)
            {
                UpdateValue(key, dto, updateCacheItemMethod);
            }
        }

        public void Update(IEnumerable<TValue> values)
        {
            ValidateParameterIsNotNull(values, "values");

            lock (_lockObject)
            {
                foreach (var value in values)
                {
                    if (value != null)
                    {
                        UpdateValue(_keyProperty(value), value);
                    }
                }
            }
        }

        //public void Update<T>(TKey key, IEnumerable<T> dtos, Func<T, TValue, TValue> updateCacheItemMethod)
        //{
        //    ValidateParameterIsNotNull(key, "key");
        //    ValidateParameterIsNotNull(dtos, "dtos");
        //    ValidateParameterIsNotNull(updateCacheItemMethod, "updateCacheItemMethod");

        //    lock (_lockObject)
        //    {
        //        foreach (var dto in dtos)
        //        {
        //            UpdateValue(key, dto, updateCacheItemMethod);
        //        }
        //    }
        //}

        #endregion

        #region Methods

        internal IEnumerable<TValue> GetCacheItemsByState(CacheItemState state)
        {
            return _dictionary.Values.Where(v => v.Status == state).Select(i => i.Item).ToList();
        }

        protected void ResetCacheItemStatus()
        {
            if (_dictionary.Count == 0)
                return;

            var items = _dictionary.Values.Where(x => x.Status != CacheItemState.Unmodified);
            if (items != null)
            {
                foreach (var cacheItem in items)
                {
                    cacheItem.Status = CacheItemState.Unmodified;
                }
            }
        }

        protected void UpdateValue(TKey key, TValue value)
        {
            ICacheItem<TValue> cacheItem;
            if (_dictionary.TryGetValue(key, out cacheItem))
            {
                if (cacheItem.Status == CacheItemState.Deleted)
                    throw new InvalidOperationException(string.Format("The item (key: {0}) is currently deleted.", key));

                CacheItemState state = cacheItem.Status == CacheItemState.Added ? CacheItemState.Added : CacheItemState.Modified;
                _dictionary[key] = new PicnicCacheItem<TValue>(value, state);
            }
            else
            {
                throw new InvalidOperationException(string.Format("The item (key: {0}) is not in the cache.", key));
            }
        }

        protected void UpdateValue<T>(TKey key, T dto, Func<T, TValue, TValue> del)
        {
            ICacheItem<TValue> cacheItem;
            TValue newValue;
            if (_dictionary.TryGetValue(key, out cacheItem))
            {
                if (cacheItem.Status == CacheItemState.Deleted)
                    throw new InvalidOperationException(string.Format("The item (key: {0}) is currently deleted.", key));

                CacheItemState state = cacheItem.Status == CacheItemState.Added ? CacheItemState.Added : CacheItemState.Modified;
                newValue = del(dto, cacheItem.Item);
                _dictionary[key] = new PicnicCacheItem<TValue>(newValue, state);
            }
            else
            {
                throw new InvalidOperationException(string.Format("The item (key: {0}) is not in the cache.", key));
            }
        }

        #endregion
    }
}