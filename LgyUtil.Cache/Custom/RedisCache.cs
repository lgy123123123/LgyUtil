using System;
using CSRedis;
using Newtonsoft.Json;

namespace LgyUtil.Cache.Custom
{
    /// <summary>
    /// redis内存缓存帮助类，大的类对象或数组，占用内存较大
    /// </summary>
    public sealed class RedisCache : ICache
    {
        private CSRedisClient Client { get; set; }

        /// <summary>
        /// 设置序列化配置，默认空值和默认值忽略
        /// </summary>
        /// <param name="setting">自定义序列化</param>
        public void SetSerializeSetting(JsonSerializerSettings setting = null)
        {
            Client.CurrentSerialize = (obj) => JsonConvert.SerializeObject(obj, setting ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            });
        }

        /// <summary>
        /// 构造Redis缓存对象
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
            SetSerializeSetting();
        }

        /// <summary>
        /// 构造Redis缓存对象，哨兵模式
        /// </summary>
        /// <param name="master">哨兵主节点名称</param>
        /// <param name="connectString">连接字符串,多个用分号分隔</param>
        public RedisCache(string master, string connectString)
        {
            Client = new CSRedisClient(master, connectString.Split(';'));
            SetSerializeSetting();
        }

        /// <summary>
        /// 构造Redis缓存对象，只通过连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        public RedisCache(string connectString)
        {
            Client = new CSRedisClient(connectString);
            SetSerializeSetting();
        }

        /// <inheritdoc/>
        public bool Exists(string key)
        {
            return Client.Exists(key);
        }

        /// <summary>
        /// 滑动过期的key
        /// </summary>
        private const string ExpSlidingKey = "ExpiresSliding";

        /// <summary>
        /// 绝对过期key
        /// </summary>
        private const string ExpAbsoluteKey = "ExpiresAbsolute";

        /// <summary>
        /// 数据key
        /// </summary>
        private const string DataKey = "Data";

        /// <inheritdoc/>
        public void Set<T>(string key, T value, TimeSpan? expiresSliding = null, DateTime? expiresAbsolute = null)
        {
            if (expiresAbsolute != null && expiresAbsolute.Value <= DateTime.Now)
                throw new Exception("绝对过期时间，必须大于当前时间");

            var expiresSlidingTicks = expiresSliding?.Ticks ?? 0;
            var expiresAbsoluteTicks = expiresAbsolute?.Ticks ?? 0;
            Client.HMSet(key, DataKey, value, ExpSlidingKey, expiresSlidingTicks, ExpAbsoluteKey, expiresAbsoluteTicks);
            SetCacheExpires(key, expiresSlidingTicks, expiresAbsoluteTicks);
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        private void SetCacheExpires(string key, long expiresSlidingTicks = -1, long expiresAbsoluteTicks = -1)
        {
            //都为0时，重新获取缓存过期时间
            if (expiresSlidingTicks == -1 && expiresAbsoluteTicks == -1)
            {
                var expTimes = Client.HMGet<long>(key, ExpSlidingKey, ExpAbsoluteKey);
                expiresSlidingTicks = expTimes[0];
                expiresAbsoluteTicks = expTimes[1];
            }

            TimeSpan? expiresSliding = null;
            if (expiresSlidingTicks > 0)
                expiresSliding = new TimeSpan(expiresSlidingTicks);
            DateTime? expiresAbsolute = null;
            if (expiresAbsoluteTicks > 0)
                expiresAbsolute = new DateTime(expiresAbsoluteTicks);

            var expires = GetMinExpiresTime(expiresSliding, expiresAbsolute);
            if (expires != null)
                Client.Expire(key, expires.Value);
        }

        /// <summary>
        /// 获取最小的过期时间间隔
        /// </summary>
        /// <param name="expiresSliding"></param>
        /// <param name="expiresAbsolute"></param>
        /// <returns></returns>
        private TimeSpan? GetMinExpiresTime(TimeSpan? expiresSliding = null, DateTime? expiresAbsolute = null)
        {
            if (expiresSliding != null && expiresAbsolute != null)
            {
                //取时间间隔短的，做为过期时间
                if (DateTime.Now.Add(expiresSliding.Value) > expiresAbsolute)
                    return expiresAbsolute - DateTime.Now;
                else
                    return expiresSliding;
            }
            else if (expiresSliding != null)
                return expiresSliding;
            else if (expiresAbsolute != null)
                return expiresAbsolute - DateTime.Now;

            return null;
        }

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            if (Exists(key))
            {
                var data = Client.HGet<T>(key, DataKey);
                SetCacheExpires(key);
                return data;
            }

            return default;
        }

        /// <inheritdoc/>
        public string GetString(string key)
        {
            if (Exists(key))
            {
                var data = Client.HGet<string>(key, DataKey);
                SetCacheExpires(key);
                return data;
            }

            return null;
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            Client.Del(key);
        }

        /// <inheritdoc/>
        public void RemoveAll(params string[] keys)
        {
            if (keys.Length > 0)
                Client.Del(keys);
            else
                Client.NodesServerManager.FlushDb();
        }

        /// <inheritdoc/>
        public void RemoveAllPrefix(string prefix)
        {
            var keys = GetKeysByPrefix(prefix);
            RemoveAll(keys);
        }

        /// <inheritdoc/>
        public string[] GetKeysByPrefix(string prefix) => Client.Keys(prefix + "*");

        /// <inheritdoc/>
        public string[] GetAllKeys() => Client.Keys("*");
    }
}