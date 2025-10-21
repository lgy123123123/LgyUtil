using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LgyUtil.Cache.Custom
{
    /// <summary>
    /// 内存缓存帮助类
    /// </summary>
    public sealed class MemoryCache : ICache
    {
        /// <summary>
        /// 所有缓存存储
        /// </summary>
        ConcurrentDictionary<string, MemoryCacheModel> dicCache { get; set; } = new ConcurrentDictionary<string, MemoryCacheModel>();

        /// <summary>
        /// 内存缓存帮助类
        /// </summary>
        /// <param name="clearExpirationTime">定期清理缓存的时间，默认5分钟清理一次，使用TimeSpan.Zero，不进行清理</param>
        public MemoryCache(TimeSpan? clearExpirationTime = null)
        {
            //默认5分钟清理一次
            if (clearExpirationTime == null)
                clearExpirationTime = TimeSpan.FromMinutes(5);

            if (clearExpirationTime != TimeSpan.Zero)//zero不清理
                //开一个线程，定时检查缓存是否过期
                Task.Run(() =>
                {
                    Thread.Sleep(clearExpirationTime.Value);
                    foreach (var val in dicCache.Values.Where(c => c.ExpiresSliding != null || c.ExpiressAbsoulte != null))
                    {
                        CheckDataExpires(val.Key);
                    }
                });
        }
        /// <inheritdoc/>
        public bool Exists(string key)
        {
            CheckDataExpires(key);
            return dicCache.ContainsKey(key);
        }
        /// <inheritdoc/>
        public void Set<T>(string key, T value, TimeSpan? expiresSliding = null, DateTime? expiresAbsolute = null)
        {
            if (expiresAbsolute != null && expiresAbsolute.Value < DateTime.Now)
                throw new Exception("绝对过期时间，不能小于当前时间");
            var cache = new MemoryCacheModel
            {
                Key = key,
                Value = value,
                ExpiresSliding = expiresSliding,
                ExpiressAbsoulte = expiresAbsolute
            };
            if (expiresSliding != null)
                cache.SlidingAbsoluteDate = DateTime.Now.Add(expiresSliding.Value);
            dicCache.AddOrUpdate(key, cache, (k, c) => cache);
        }
        /// <summary>
        /// 检查缓存是否过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isGet">是否是get方法检查，此时需要增加滑动过期时间</param>
        private void CheckDataExpires(string key, bool isGet = false)
        {
            if (!dicCache.TryGetValue(key, out var cache)) return;
            lock (cache)
            {
                //先检查绝对过期时间
                if (cache.ExpiressAbsoulte != null && cache.ExpiressAbsoulte.Value <= DateTime.Now)
                {
                    Remove(key);
                    return;
                }
                if (cache.ExpiresSliding == null) return;
                //检查滑动过期时间
                //过期删除
                if (cache.SlidingAbsoluteDate.Value <= DateTime.Now)
                {
                    Remove(key);
                }
                //未过期，如果是get获取，增加滑动过期时间
                else if (isGet)
                {
                    cache.SlidingAbsoluteDate = DateTime.Now.Add(cache.ExpiresSliding.Value);
                }
            }
        }
        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            CheckDataExpires(key, true);
            if (dicCache.TryGetValue(key, out var cache))
                return (T)cache.Value;
            return default;
        }
        /// <inheritdoc/>
        public string GetString(string key)
        {
            CheckDataExpires(key, true);
            if (dicCache.TryGetValue(key, out var cache))
                return cache.Value as string;
            return null;
        }
        /// <inheritdoc/>
        public void Remove(string key) => dicCache.TryRemove(key, out _);

        /// <inheritdoc/>
        public void RemoveAll(params string[] keys)
        {
            if (keys.Length > 0)
            {
                foreach (string key in keys)
                {
                    Remove(key);
                }
            }
            else
                dicCache.Clear();
        }
        /// <inheritdoc/>
        public void RemoveAllPrefix(string prefix)
        {
            var findKeys = GetKeysByPrefix(prefix);
            foreach (var key in findKeys)
            {
                Remove(key);
            }
        }
        /// <inheritdoc/>
        public string[] GetKeysByPrefix(string prefix) => dicCache.Keys.Where(k => k.StartsWith(prefix)).ToArray();

        /// <summary>
        /// 获取Key中包含的字符串
        /// </summary>
        /// <param name="contains"></param>
        /// <returns></returns>
        public string[] GetKeysByContains(string contains) => dicCache.Keys.Where(k => k.Contains(contains)).ToArray();
        /// <inheritdoc/>
        public string[] GetAllKeys() => dicCache.Keys.ToArray();
    }
}
