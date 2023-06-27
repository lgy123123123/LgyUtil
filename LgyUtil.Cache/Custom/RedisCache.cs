using CSRedis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace LgyUtil.Cache.Custom
{
    /// <summary>
    /// 内存缓存帮助类
    /// </summary>
    public sealed class RedisCache : ICache
    {
        private CSRedisClient Client { get; set; }
        /// <summary>
        /// 构造Redis缓存对象，并同时初始化RedisHelper对象
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">端口号</param>
        /// <param name="database">使用数据库编号</param>
        /// <param name="password">密码</param>
        public RedisCache(string ip = "127.0.0.1", int port = 6379, int database = 0, string password = null)
        {
            if (string.IsNullOrEmpty(ip))
                ip = "127.0.0.1";
            if (port == 0)
                port = 6379;
            string connectString = $"{ip}:{port},defaultDatabase={database}";
            if (string.IsNullOrEmpty(password))
                connectString += $",password={password}";
            Client = new CSRedisClient(connectString);
            RedisHelper.Initialization(Client);
        }
        /// <summary>
        /// 构造Redis缓存对象，哨兵模式
        /// </summary>
        /// <param name="master">哨兵主节点名称</param>
        /// <param name="connectString">连接字符串,多个用分号分隔</param>
        public RedisCache(string master, string connectString)
        {
            Client = new CSRedisClient(master, connectString.Split(';'));
            RedisHelper.Initialization(Client);
        }
        /// <summary>
        /// 构造Redis缓存对象，只通过连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        public RedisCache(string connectString)
        {
            Client = new CSRedisClient(connectString);
            RedisHelper.Initialization(Client);
        }
        /// <inheritdoc/>
        public bool Exists(string key)
        {
            return Client.Exists(key);
        }
        /// <inheritdoc/>
        public void Set<T>(string key, T value, TimeSpan? expiresSliding = null, DateTime? expiressAbsoulte = null)
        {
            if (expiressAbsoulte != null && expiressAbsoulte.Value <= DateTime.Now)
                throw new Exception("绝对过期时间，必须大于当前时间");
            var cache = new RedisCacheModel<T>
            {
                Value = value,
                ExpiresSliding = expiresSliding,
                ExpiressAbsoulte = expiressAbsoulte
            };
            if (expiresSliding != null)
                cache.SlidingAbsoluteDate = DateTime.Now.Add(expiresSliding.Value);
            RedisHelper.Set(key, cache);
            SetCacheExpiress(key, cache);
        }
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cache"></param>
        private void SetCacheExpiress<T>(string key, RedisCacheModel<T> cache)
        {
            var expiress = GetMinExpiressTime(cache.ExpiresSliding, cache.ExpiressAbsoulte);
            if (expiress != null)
                RedisHelper.Expire(key, expiress.Value);
        }
        /// <summary>
        /// 获取最小的过期时间间隔
        /// </summary>
        /// <param name="expiresSliding"></param>
        /// <param name="expiressAbsoulte"></param>
        /// <returns></returns>
        private TimeSpan? GetMinExpiressTime(TimeSpan? expiresSliding = null, DateTime? expiressAbsoulte = null)
        {
            if (expiresSliding != null && expiressAbsoulte != null)
            {
                //取时间间隔短的，做为过期时间
                if (DateTime.Now.Add(expiresSliding.Value) > expiressAbsoulte)
                    return expiressAbsoulte - DateTime.Now;
                else
                    return expiresSliding;
            }
            else if (expiresSliding != null)
                return expiresSliding;
            else if (expiressAbsoulte != null)
                return expiressAbsoulte - DateTime.Now;
            return null;
        }

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            if (Exists(key))
            {
                var cache = RedisHelper.Get<RedisCacheModel<T>>(key);
                SetCacheExpiress(key, cache);

                return cache.Value;
            }
            return default;
        }
        /// <inheritdoc/>
        public string GetString(string key)
        {
            if (Exists(key))
            {
                var cache = RedisHelper.Get<RedisCacheModel<string>>(key);
                SetCacheExpiress(key, cache);
                return cache.Value;
            }
            return null;
        }
        /// <inheritdoc/>
        public void Remove(string key)
        {
            RedisHelper.Del(key);
        }
        /// <inheritdoc/>
        public void RemoveAll(params string[] keys)
        {
            if (keys.Length > 0)
                RedisHelper.Del(keys);
            else
                RedisHelper.NodesServerManager.FlushDb();
        }
        /// <inheritdoc/>
        public void RemoveAllPrefix(string prefix)
        {
            var keys = GetKeysByPrefix(prefix);
            RemoveAll(keys);
        }
        /// <inheritdoc/>
        public string[] GetKeysByPrefix(string prefix) => RedisHelper.Keys(prefix + "*");

        /// <inheritdoc/>
        public string[] GetAllKeys() => RedisHelper.Keys("*");
    }
}
