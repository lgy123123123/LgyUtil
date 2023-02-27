using System;
using System.Collections.Immutable;
using System.Threading;

namespace LgyUtil.NStringFormat
{
    internal class NSCache<TKey, TValue>
    {
        private IImmutableDictionary<TKey, TValue> _cache = ImmutableDictionary.Create<TKey, TValue>();

        public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
        {
            Lazy<TValue> lazy = new Lazy<TValue>(valueFactory);
            IImmutableDictionary<TKey, TValue> cache;
            TValue value;
            IImmutableDictionary<TKey, TValue> value2;
            do
            {
                cache = _cache;
                if (cache.TryGetValue(key, out value))
                {
                    return value;
                }

                value = lazy.Value;
                value2 = cache.Add(key, value);
            }
            while (Interlocked.CompareExchange(ref _cache, value2, cache) != cache);
            return value;
        }

        public void Clear()
        {
            IImmutableDictionary<TKey, TValue> cache;
            IImmutableDictionary<TKey, TValue> value;
            do
            {
                cache = _cache;
                value = _cache.Clear();
            }
            while (Interlocked.CompareExchange(ref _cache, value, cache) != cache);
        }
    }
}
