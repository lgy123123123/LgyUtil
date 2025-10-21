using System;
using System.Linq;
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
        /// 全局前缀
        /// </summary>
        private string GlobalPrefix { get; set; }

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
            GlobalPrefix = connectString.Split(';').FirstOrDefault().Split(',').Where(c => c.StartsWith("prefix=")).Select(c => c.Replace("prefix=", "")).FirstOrDefault();
            SetSerializeSetting();
        }

        /// <summary>
        /// 构造Redis缓存对象，只通过连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        public RedisCache(string connectString)
        {
            Client = new CSRedisClient(connectString);
            GlobalPrefix = connectString.Split(',').Where(c => c.StartsWith("prefix=")).Select(c => c.Replace("prefix=", "")).FirstOrDefault();
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

            var pipe = Client
                .StartPipe()
                .HMSet(key, DataKey, value, ExpSlidingKey, expiresSlidingTicks, ExpAbsoluteKey, expiresAbsoluteTicks);
            var expiredTime = GetCacheExpiresTime(expiresSlidingTicks, expiresAbsoluteTicks);
            if (expiredTime != null)
                pipe = pipe.Expire(key, expiredTime.Value);
            pipe.EndPipe();
        }

        /// <summary>
        /// 获取缓存的过期时间
        /// </summary>
        private TimeSpan? GetCacheExpiresTime(long expiresSlidingTicks, long expiresAbsoluteTicks)
        {
            TimeSpan? expiresSliding = null;
            if (expiresSlidingTicks > 0)
                expiresSliding = new TimeSpan(expiresSlidingTicks);
            DateTime? expiresAbsolute = null;
            if (expiresAbsoluteTicks > 0)
                expiresAbsolute = new DateTime(expiresAbsoluteTicks);

            if (expiresSliding != null && expiresAbsolute != null)
            {
                //取时间间隔短的，做为过期时间
                if (DateTime.Now.Add(expiresSliding.Value) > expiresAbsolute)
                    return expiresAbsolute - DateTime.Now;
                return expiresSliding;
            }

            if (expiresSliding != null)
                return expiresSliding;
            if (expiresAbsolute != null)
                return expiresAbsolute - DateTime.Now;
            return null;
        }

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            var pipe = Client.StartPipe();
            pipe.Exists(key);
            pipe.HGet<T>(key, DataKey);
            pipe.HGet<long>(key, ExpSlidingKey);
            pipe.HGet<long>(key, ExpAbsoluteKey);
            var result = pipe.EndPipe();
            if (!(bool) result[0])
                return default;
            var expiresTime = GetCacheExpiresTime((long) result[2], (long) result[3]);
            if (expiresTime != null)
                Client.Expire(key, expiresTime.Value);
            return (T) result[1];
        }

        /// <inheritdoc/>
        public string GetString(string key)
        {
            return Get<string>(key);
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
            // Scan 方式更安全
            var pattern = GlobalPrefix + prefix + "*";
            var cursor = 0L;
            do
            {
                var scanResult = Client.Scan(cursor, pattern, 1000);
                cursor = scanResult.Cursor;
                if (scanResult.Items.Length > 0)
                {
                    Client.Del(scanResult.Items.Select(k => k.Replace(GlobalPrefix, "")).ToArray());
                }
            } while (cursor != 0);
        }

        /// <inheritdoc/>
        public string[] GetKeysByPrefix(string prefix) => Client.Keys(GlobalPrefix + prefix + "*");

        /// <inheritdoc/>
        public string[] GetAllKeys() => Client.Keys("*");
    }
}