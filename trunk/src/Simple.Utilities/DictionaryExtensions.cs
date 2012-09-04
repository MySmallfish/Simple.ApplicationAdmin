using System;
using System.Collections.Generic;

namespace Simple.Utilities
{
    public static class DictionaryExtensions
    {
        public static TValue ValueOr<TKey,TValue>(this Dictionary<TKey,TValue> source, TKey key, Func<TValue> whenNotFound)
        {
            Requires.ArgumentNotNull(source, "source");
            
            if (!source.ContainsKey(key))
            {
                return whenNotFound();
            }

            return source[key];
        }

        public static TValue ValueOrDefault<TKey,TValue>(this Dictionary<TKey,TValue> source, TKey key)
        {
            return source.ValueOr(key, () => default(TValue));
        }
    }
}
