using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PicnicCache
{
    public class PicnicCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class, new()
    {
        #region Fields

        private readonly IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly Func<TValue, TKey> _keyProperty;

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

        public virtual TValue Fetch(TKey key, Func<TValue> del)
        {
            TValue value;
            if (!_dictionary.TryGetValue(key, out value))
            {
                value = del();
                if (value != null)
                    _dictionary.Add(key, value);
            }
            return value;
        }

        public virtual IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del)
        {
            if (!_isAllLoaded)
            {
                FetchAllInternal(del);
            }
            return _dictionary.Values;
        }

        public virtual void UpdateCache(TValue value)
        {
            var key = _keyProperty(value);
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }

        public virtual void UpdateCacheAndSave(TValue value, Action del)
        {
            UpdateCache(value);
            del();
        }

        public virtual void UpdateCacheAndSave(TValue value, Action<TValue> del)
        {
            UpdateCache(value);
            del(value);
        }

        #endregion

        #region Methods

        protected virtual void FetchAllInternal(Func<IEnumerable<TValue>> del)
        {
            IEnumerable<TValue> values = del();
            if (values != null)
            {
                foreach(var value in values.Except(_dictionary.Values))
                {
                    _dictionary.Add(_keyProperty(value), value);
                }
            }

            _isAllLoaded = true;
        }

        #endregion
    }
}