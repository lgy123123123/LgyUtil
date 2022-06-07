using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using System;

namespace LgyUtil
{
    /// <summary>
    /// 内存缓存帮助类
    /// </summary>
    public class CacheRedisUtil : ICacheUtil
    {
        /// <summary>
        /// 缓存对象
        /// </summary>
        private CSRedisCache Cache { get; set; }
        private CSRedisClient Client { get; set; }
        /// <summary>
        /// 构造Redis缓存对象，并同时初始化RedisHelper对象
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port">端口号</param>
        /// <param name="database">使用数据库编号</param>
        /// <param name="password">密码</param>
        public CacheRedisUtil(string ip = "127.0.0.1", int port = 6379, int database = 0, string password = null)
        {
            if (string.IsNullOrEmpty(ip))
                ip = "127.0.0.1";
            if (port == 0)
                port = 6379;
            string connectString = $"{ip}:{port},defaultDatabase={database}";
            if (string.IsNullOrEmpty(password))
                connectString += $",password={password}";
            Client = new CSRedis.CSRedisClient(connectString);
            RedisHelper.Initialization(Client);
            Cache = new CSRedisCache(Client);
        }
        /// <summary>
        /// 构造Redis缓存对象，哨兵模式
        /// </summary>
        /// <param name="master">哨兵主节点名称</param>
        /// <param name="connectString">连接字符串,多个用分号分隔</param>
        public CacheRedisUtil(string master, string connectString)
        {
            Client = new CSRedis.CSRedisClient(master, connectString.Split(';'));
            Cache = new CSRedisCache(Client);
        }
        /// <summary>
        /// 构造Redis缓存对象，只通过连接字符串
        /// </summary>
        /// <param name="connectString"></param>
        public CacheRedisUtil(string connectString)
        {
            Client = new CSRedis.CSRedisClient(connectString);
            Cache = new CSRedisCache(Client);
        }
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return Client.Exists(key);
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSliding">滑动过期时间(一段内不访问，则清空缓存，访问后，按滑动时间重新计算)，null则不设置</param>
        /// <param name="expiressAbsoulte">绝对过期时间，null则不设置</param>
        /// <returns></returns>
        public void Set(string key, object value, TimeSpan? expiresSliding = null, DateTime? expiressAbsoulte = null)
        {
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = expiresSliding,
                AbsoluteExpirationRelativeToNow = expiressAbsoulte - DateTime.Now
            };
            Cache.SetObject(key, value, options);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Cache.GetObject<T>(key);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return Cache.GetObject(key);
        }
        /// <summary>
        /// 获取字符串缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetString(string key)
        {
            return Cache.GetString(key);
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }
        /// <summary>
        /// 清空缓存
        /// </summary>
        public void RemoveAll()
        {
            Client.NodesServerManager.FlushDb();
        }
    }
}
