//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PicnicCache
{
    public abstract class PicnicCacheBase<TKey, TValue> : ICacheBase<TKey, TValue> where TValue : class, new()
    {
        #region Fields

        protected readonly IDictionary<TKey, ICacheItem<TValue>> _dictionary = new Dictionary<TKey, ICacheItem<TValue>>();
        protected readonly object _lockObject = new object();
        protected readonly Func<TValue, TKey> _keyProperty;

        protected bool _isAllLoaded = false;

        #endregion

        #region Constructors

        public PicnicCacheBase(string keyPropertyName)
        {
            ValidateParameterIsNotNull(keyPropertyName, "keyPropertyName");

            PropertyInfo propertyInfo = typeof(TValue).GetRuntimeProperty(keyPropertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("The property: {0} does no exist on the type: {1}.",
                                                          keyPropertyName,
                                                          typeof(TValue).FullName));
            _keyProperty = (Func<TValue, TKey>)(x => (TKey)propertyInfo.GetValue(x));
        }

        public PicnicCacheBase(Func<TValue, TKey> keyProperty)
        {
            ValidateParameterIsNotNull(keyProperty, "keyProperty");

            _keyProperty = keyProperty;
        }

        #endregion

        #region ICacheBase Implementation

        public void Add(TValue value)
        {
            lock (_lockObject)
            {
                _dictionary.Add(_keyProperty(value), new CacheItem<TValue>(value, CacheItemState.Added));
            }
        }

        public void ClearAll()
        {
            lock (_lockObject)
            {
                _dictionary.Clear();
            }
            _isAllLoaded = false;
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
            return Delete(_keyProperty(value));
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
                    UpdateValue(_keyProperty(value), value);
                }
            }
        }

        public void Update<T>(TKey key, IEnumerable<T> dtos, Func<T, TValue, TValue> updateCacheItemMethod)
        {
            ValidateParameterIsNotNull(key, "key");
            ValidateParameterIsNotNull(dtos, "dtos");
            ValidateParameterIsNotNull(updateCacheItemMethod, "updateCacheItemMethod");

            lock (_lockObject)
            {
                foreach (var dto in dtos)
                {
                    UpdateValue(key, dto, updateCacheItemMethod);
                }
            }
        }

        #endregion

        #region Methods

        protected TValue FetchInternal(TKey key, Func<TValue> del)
        {
            ICacheItem<TValue> cacheValue;
            TValue value;
            lock (_lockObject)
            {
                if (_dictionary.TryGetValue(key, out cacheValue))
                {
                    value = cacheValue.Status == CacheItemState.Deleted ? null : cacheValue.Item;
                }
                else
                {
                    value = del();
                    if (value != null)
                    {
                        AddLoadedItem(value);
                    }
                }

            }
            return value;
        }

        protected IEnumerable<TValue> FetchAllInternal(Func<IEnumerable<TValue>> del)
        {
            if (!_isAllLoaded)
            {
                Load(del);
            }
            return _dictionary.Values
                       .Where(x => x.Status != CacheItemState.Deleted)
                       .Select(x => x.Item);
        }

        protected void Load(Func<IEnumerable<TValue>> del)
        {
            IEnumerable<TValue> values = del();
            if (values != null)
            {
                lock (_lockObject)
                {
                    foreach (var value in values.Where(x => !_dictionary.ContainsKey(_keyProperty(x))))
                    {
                        AddLoadedItem(value);
                    }
                }
            }
            _isAllLoaded = true;
        }

        protected void AddLoadedItem(TValue value)
        {
            _dictionary.Add(_keyProperty(value), new CacheItem<TValue>(value, CacheItemState.Unmodified));
        }

        protected void ValidateParameterIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentException(string.Format("The parameter: {0} can not be null.", parameter));
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
                    return;

                CacheItemState state = cacheItem.Status == CacheItemState.Added ? CacheItemState.Added : CacheItemState.Modified;
                _dictionary[key] = new CacheItem<TValue>(value, state);
            }
        }

        protected void UpdateValue<T>(TKey key, T dto, Func<T, TValue, TValue> del)
        {
            ICacheItem<TValue> cacheItem;
            TValue newValue;
            if (_dictionary.TryGetValue(key, out cacheItem))
            {
                if (cacheItem.Status == CacheItemState.Deleted)
                    return;

                CacheItemState state = cacheItem.Status == CacheItemState.Added ? CacheItemState.Added : CacheItemState.Modified;
                newValue = del(dto, cacheItem.Item);
                _dictionary[key] = new CacheItem<TValue>(newValue, state);
            }
        }

        #endregion
    }
}