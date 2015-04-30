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

        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly Func<TValue, TKey> _keyProperty;
        private readonly object _lockObject = new object();

        private bool _isAllLoaded = false;

        #endregion

        #region Constructors

        public PicnicCache(string keyPropertyName)
        {
            PropertyInfo propertyInfo = typeof(TValue).GetRuntimeProperty(keyPropertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("The property: {0} does no exist on the type: {1}.", keyPropertyName, typeof(TValue).FullName));
            _keyProperty = (Func<TValue, TKey>)(x => (TKey)propertyInfo.GetValue(x));
        }

        public PicnicCache(Func<TValue, TKey> keyProperty)
        {
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
            TValue value;
            lock (_lockObject)
            {
                if (!_dictionary.TryGetValue(key, out value))
                {
                    value = del();
                    if (value != null)
                        _dictionary.Add(key, value);
                }
            }
            return value;
        }

        public IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del)
        {
            if (!_isAllLoaded)
            {
                FetchAllInternal(del);
            }
            return _dictionary.Values;
        }

        public bool Remove(TKey key)
        {
            bool isItemInCache = false;

            lock (_lockObject)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary.Remove(key);
                    isItemInCache = true;
                }
            }

            return isItemInCache;
        }

        public bool Remove(TValue value)
        {
            return Remove(_keyProperty(value));
        }

        public bool Delete(TKey key, Action del)
        {
            var result = Remove(key);
            del();
            return result;
        }

        public bool Delete(TKey key, Action<TKey> del)
        {
            var result = Remove(key);
            del(key);
            return result;
        }

        public bool Delete(TValue value, Action del)
        {
            var result = Remove(_keyProperty(value));
            del();
            return result;
        }

        public bool Delete(TValue value, Action<TValue> del)
        {
            var result = Remove(_keyProperty(value));
            del(value);
            return result;
        }

        public bool Delete(TValue value, Action<TKey> del)
        {
            TKey key = _keyProperty(value);
            var result = Remove(key);
            del(key);
            return result;
        }

        public void SaveAll(Action<IEnumerable<TValue>> del)
        {
            del(_dictionary.Values);
        }

        public void Save(TValue value, Action del)
        {
            UpdateCache(value);
            del();
        }

        public void Save(TValue value, Action<TValue> del)
        {
            UpdateCache(value);
            del(value);
        }

        public void SaveAll(TValue value, Action<IEnumerable<TValue>> del)
        {
            UpdateCache(value);
            del(_dictionary.Values);
        }

        public void SaveAll(IEnumerable<TValue> values, Action<IEnumerable<TValue>> del)
        {
            foreach (var value in values)
            {
                UpdateCache(value);
            }
            del(_dictionary.Values);
        }
        
        public void UpdateCache(TValue value)
        {
            var key = _keyProperty(value);
            lock (_lockObject)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = value;
                }
                else
                {
                    _dictionary.Add(key, value);
                }
            }
        }

        public void UpdateCache(IEnumerable<TValue> values)
        {
            foreach (var value in values)
            {
                UpdateCache(value);
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
                        _dictionary.Add(_keyProperty(value), value);
                    }
                }
            }
            _isAllLoaded = true;
        }

        #endregion
    }
}