//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PicnicCache
{
    public class PicnicCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class, new()
    {
        #region Fields

        protected readonly IDictionary<TKey, CacheItem<TValue>> _dictionary = new Dictionary<TKey, CacheItem<TValue>>();
        protected readonly Func<TValue, TKey> _keyProperty;
        protected readonly object _lockObject = new object();

        protected bool _isAllLoaded = false;

        #endregion

        #region Constructors

        public PicnicCache(string keyPropertyName)
        {
            ValidateParameterIsNotNull(keyPropertyName, "keyPropertyName");

            PropertyInfo propertyInfo = typeof(TValue).GetRuntimeProperty(keyPropertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("The property: {0} does no exist on the type: {1}.",
                                                          keyPropertyName,
                                                          typeof(TValue).FullName));
            _keyProperty = (Func<TValue, TKey>)(x => (TKey)propertyInfo.GetValue(x));
        }

        public PicnicCache(Func<TValue, TKey> keyProperty)
        {
            ValidateParameterIsNotNull(keyProperty, "keyProperty");

            _keyProperty = keyProperty;
        }

        #endregion

        #region ICache Implementation

        public void ClearAll()
        {
            lock (_lockObject)
            {
                _dictionary.Clear();
            }
        }

        public TValue Fetch(TKey key, Func<TValue> del)
        {
            ValidateParameterIsNotNull(del, "del");
            ValidateParameterIsNotNull(key, "key");

            CacheItem<TValue> cacheValue;
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
                        cacheValue = new CacheItem<TValue>(value, CacheItemState.Unmodified);
                        _dictionary.Add(key, cacheValue);
                    }
                }

            }
            return value;
        }

        public IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del)
        {
            ValidateParameterIsNotNull(del, "del");

            if (!_isAllLoaded)
            {
                FetchAllInternal(del);
            }
            return _dictionary.Values
                       .Where(x => x.Status != CacheItemState.Deleted)
                       .Select(x => x.Item);
        }

        public bool Delete(TKey key)
        {
            ValidateParameterIsNotNull(key, "key");

            bool isItemInCache = false;

            lock (_lockObject)
            {
                CacheItem<TValue> cacheValue;
                if (_dictionary.TryGetValue(key, out cacheValue))
                {
                    cacheValue.Status = CacheItemState.Deleted;
                    isItemInCache = true;
                }
            }

            return isItemInCache;
        }

        public bool Delete(TValue value)
        {
            return Delete(_keyProperty(value));
        }

        public void Save(Action<IEnumerable<CacheItem<TValue>>> saveDelegate)
        {
            ValidateParameterIsNotNull(saveDelegate, "saveDelegate");

            saveDelegate(_dictionary.Values);
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
        }

        public void Save(Action<IEnumerable<TValue>> updateDelegate, 
                         Action<IEnumerable<TValue>> addDelegate, 
                         Action<IEnumerable<TValue>> deleteDelegate)
        {
            ValidateParameterIsNotNull(updateDelegate, "updateDelegate");
            ValidateParameterIsNotNull(addDelegate, "addDelegate");
            ValidateParameterIsNotNull(deleteDelegate, "deleteDelegate");

            var modifiedItems = _dictionary.Values
                                    .Where(x => x.Status == CacheItemState.Modified)
                                    .Select(x => x.Item);
            var addedItems = _dictionary.Values
                                 .Where(x => x.Status == CacheItemState.Added)
                                 .Select(x => x.Item);
            var deletedItems = _dictionary.Values
                                   .Where(x => x.Status == CacheItemState.Deleted)
                                   .Select(x => x.Item);
            updateDelegate(modifiedItems);
            addDelegate(addedItems);
            deleteDelegate(deletedItems);
        }

        public void Update(TValue value)
        {
            ValidateParameterIsNotNull(value, "value");

            var key = _keyProperty(value);
            lock (_lockObject)
            {
                UpdateOrAddValue(key, value);
            }
        }

        public void Update(IEnumerable<TValue> values)
        {
            ValidateParameterIsNotNull(values, "values");

            lock (_lockObject)
            {
                foreach (var value in values)
                {
                    UpdateOrAddValue(_keyProperty(value), value);
                }
            }
        }

        #endregion

        #region Methods

        protected void FetchAllInternal(Func<IEnumerable<TValue>> del)
        {
            IEnumerable<TValue> values = del();
            if (values != null)
            {
                lock (_lockObject)
                {
                    foreach (var value in values.Where(x => !_dictionary.ContainsKey(_keyProperty(x))))
                    {
                        _dictionary.Add(_keyProperty(value), new CacheItem<TValue>(value, CacheItemState.Unmodified));
                    }
                }
            }
            _isAllLoaded = true;
        }

        protected void ValidateParameterIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentException(string.Format("The parameter: {0} can not be null.", parameter));
        }

        protected void UpdateOrAddValue(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = new CacheItem<TValue>(value, CacheItemState.Modified);
            }
            else
            {
                _dictionary.Add(key, new CacheItem<TValue>(value, CacheItemState.Added));
            }
        }

        #endregion
    }
}