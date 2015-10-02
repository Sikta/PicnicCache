//The MIT License (MIT)
//https://github.com/DoloSoftware/PicnicCache/blob/master/LICENSE

using System;

namespace PicnicCache
{
    public abstract class PicnicCacheBaseObject
    {
        #region Methods

        protected void ValidateParameterIsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }

        #endregion
    }
}