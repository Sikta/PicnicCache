//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;
using System.Collections.Generic;

namespace PicnicCache
{
    public class PicnicCacheConfiguration<TKey, TValue> : ICacheConfiguration<TKey, TValue> where TValue : class, new()
    {
        #region Properties

        public Func<TKey, TValue> FetchMethod { get; private set; }

        public Func<IEnumerable<TValue>> FetchAllMethod { get; private set; }

        public Action<IEnumerable<ICacheItem<TValue>>> SaveMethod { get; private set; }

        #endregion

        #region Constructors

        public PicnicCacheConfiguration(Func<TKey, TValue> fetchMethod, Func<IEnumerable<TValue>> fetchAllMethod, 
                                        Action<IEnumerable<ICacheItem<TValue>>> saveMethod)
        {
            ValidateParameterIsNotNull(fetchMethod, "fetchMethod");
            ValidateParameterIsNotNull(fetchAllMethod, "fetchAllMethod");
            ValidateParameterIsNotNull(saveMethod, "saveMethod");

            FetchMethod = fetchMethod;
            FetchAllMethod = fetchAllMethod;
            SaveMethod = saveMethod;
        }

        #endregion

        #region Methods

        protected void ValidateParameterIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentException(string.Format("The parameter: {0} can not be null.", parameter));
        }

        #endregion
    }
}