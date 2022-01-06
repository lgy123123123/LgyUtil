using System;

namespace LgyUtil.Cache
{
    /// <summary>
    /// 缓存接口，两种实现：CacheMemoryUtil和CacheReidsUtil
    /// </summary>
    public interface ICacheUtil
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
        void Set(string key, object value, TimeSpan? expiresSliding = null,
            DateTime? expiressAbsoulte = null);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

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
        void RemoveAll();
    }
}
