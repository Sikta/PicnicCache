using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicnicCache
{
    public interface ICache<TKey, TValue> where TValue : class, new()
    {
        TValue Fetch(TKey key, Func<TValue> del);
        IEnumerable<TValue> FetchAll(Func<IEnumerable<TValue>> del);
        void UpdateCache(TValue value);
        void UpdateCacheAndSave(TValue value, Action del);
        void UpdateCacheAndSave(TValue value, Action<TValue> del);
        //void UpdateCacheAndSaveAll(TValue value, Action<IEnumerable<TValue>> del);
        //void UpdateCacheAndSaveAll(IEnumerable<TValue> values, Action<IEnumerable<TValue>> del);
        //void SaveAll(Action<IEnumerable<TValue>> del);
    }
}