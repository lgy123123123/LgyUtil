using System;

namespace LgyUtil.Cache.Custom
{
    /// <summary>
    /// 缓存接口，两种实现：MemoryCache和ReidsCache
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSliding">滑动过期时间(一段内不访问，则清空缓存，访问后，按滑动时间重新计算)，null则不设置</param>
        /// <param name="expiressAbsoulte">绝对过期时间，null则不设置</param>
        /// <returns></returns>
        void Set<T>(string key, T value, TimeSpan? expiresSliding = null,
            DateTime? expiressAbsoulte = null);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取字符串缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <param name="keys">可选key，不填清空所有</param>
        void RemoveAll(params string[] keys);
        /// <summary>
        /// 根据key前缀，删除缓存
        /// </summary>
        /// <param name="prefix">key前缀</param>
        void RemoveAllPrefix(string prefix);
        /// <summary>
        /// 根据前缀获取key
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns></returns>
        string[] GetKeysByPrefix(string prefix);
        /// <summary>
        /// 获取所有key
        /// </summary>
        /// <returns></returns>
        string[] GetAllKeys();
    }
}
