//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PicnicCache
{
    public class ReadOnlyPicnicCache<TKey, TValue> : PicnicCacheBaseObject, IReadOnlyCache<TKey, TValue> 
        where TValue : class
    {
        #region Fields

        internal readonly IDictionary<TKey, ICacheItem<TValue>> _dictionary = new Dictionary<TKey, ICacheItem<TValue>>();
        protected readonly object _lockObject = new object();

        protected bool _isAllLoaded = false;
        protected Func<TValue, TKey> _keyProperty;
        protected IReadOnlyCacheable<TKey, TValue> _readOnlyCacheable;

        #endregion

        #region Properties

        private IReadOnlyCacheable<TKey, TValue> ReadOnlyCacheable
        {
            get { return _readOnlyCacheable; }
            set { _readOnlyCacheable = value; }
        }

        #endregion

        #region Constructors

        protected ReadOnlyPicnicCache()
        {
        }

        protected ReadOnlyPicnicCache(IReadOnlyCacheable<TKey, TValue> readOnlyCacheable)
        {
            ValidateParameterIsNotNull(readOnlyCacheable, "readOnlyCacheable");
            ReadOnlyCacheable = readOnlyCacheable;
        }

        public ReadOnlyPicnicCache(string keyPropertyName, IReadOnlyCacheable<TKey, TValue> readOnlyCacheable)
            : this(readOnlyCacheable)
        {
            SetKeyProperty(keyPropertyName);
        }

        public ReadOnlyPicnicCache(Func<TValue, TKey> keyProperty, IReadOnlyCacheable<TKey, TValue> readOnlyCacheable)
            : this(readOnlyCacheable)
        {
            ValidateParameterIsNotNull(keyProperty, "keyProperty");

            _keyProperty = keyProperty;
        }

        #endregion

        #region IReadOnlyCache

        public void ClearAll()
        {
            lock (_lockObject)
            {
                _dictionary.Clear();
            }
            _isAllLoaded = false;
        }

        public TValue Fetch(TKey key)
        {
            ValidateParameterIsNotNull(key, "key");

            return FetchInternal(key, () => ReadOnlyCacheable.Fetch(key));
        }

        public Task<TValue> FetchAsync(TKey key)
        {
            return Task.Factory.StartNew(() => Fetch(key));
        }

        public IEnumerable<TValue> FetchAll()
        {
            return FetchAllInternal(() => ReadOnlyCacheable.FetchAll());
        }

        public Task<IEnumerable<TValue>> FetchAllAsync()
        {
            return Task.Factory.StartNew(() => FetchAll());
        }

        #endregion

        #region Methods

        protected void AddLoadedItem(TValue value)
        {
            _dictionary.Add(_keyProperty(value), new PicnicCacheItem<TValue>(value, CacheItemState.Unmodified));
        }

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
                       .Select(x => x.Item).ToList();
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

        protected void SetKeyProperty(string keyPropertyName)
        {
            ValidateParameterIsNotNull(keyPropertyName, "keyPropertyName");

            PropertyInfo propertyInfo = typeof(TValue).GetRuntimeProperty(keyPropertyName);
            if (propertyInfo == null)
                throw new ArgumentException(string.Format("The property ({0}) does not exist on the type ({1}).",
                                                          keyPropertyName,
                                                          typeof(TValue).Name));
            _keyProperty = x => (TKey)propertyInfo.GetValue(x);
        }

        #endregion
    }
}