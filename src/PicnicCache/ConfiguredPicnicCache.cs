//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PicnicCache
{
    public class ConfiguredPicnicCache<TKey, TValue> : PicnicCacheBase<TKey, TValue>, IConfiguredCache<TKey, TValue> where TValue : class, new()
    {
        #region Fields

        protected ICacheConfiguration<TKey, TValue> _configuration;

        #endregion

        #region Constructors

        protected ConfiguredPicnicCache(string keyPropertyName)
            : base(keyPropertyName)
        {
        }

        protected ConfiguredPicnicCache(Func<TValue, TKey> keyProperty)
            : base(keyProperty)
        {
        }

        public ConfiguredPicnicCache(string keyPropertyName, ICacheConfiguration<TKey, TValue> configuration)
            : base(keyPropertyName)
        {
            Initialize(configuration);
        }

        public ConfiguredPicnicCache(Func<TValue, TKey> keyProperty, ICacheConfiguration<TKey, TValue> configuration)
            : base(keyProperty)
        {
            Initialize(configuration);
        }

        #endregion

        #region IConfiguredCache Implementation

        public TValue Fetch(TKey key)
        {
            return FetchInternal(key, () => _configuration.FetchMethod(key));
        }

        public Task<TValue> FetchAsync(TKey key)
        {
            return Task.Factory.StartNew(() => Fetch(key));
        }

        public IEnumerable<TValue> FetchAll()
        {
            return FetchAllInternal(_configuration.FetchAllMethod);
        }

        public Task<IEnumerable<TValue>> FetchAllAsync()
        {
            return Task.Factory.StartNew(() => FetchAll());
        }

        public void Save()
        {
            _configuration.SaveMethod(_dictionary.Values);

            ResetCacheItemStatus();
        }

        public Task SaveAsync()
        {
            return Task.Factory.StartNew(() => Save());
        }

        #endregion

        #region Methods

        protected void Initialize(ICacheConfiguration<TKey, TValue> configuration)
        {
            ValidateParameterIsNotNull(configuration, "configuration");

            _configuration = configuration;
        }

        #endregion
    }
}