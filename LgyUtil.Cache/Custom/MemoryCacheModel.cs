using System;
using Newtonsoft.Json;

namespace LgyUtil.Cache.Custom
{
    /// <summary>
    /// 本地缓存模型
    /// </summary>
    internal class MemoryCacheModel
    {
        /// <summary>
        /// 缓存key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 缓存的值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 滑动过期时间间隔
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? ExpiresSliding { get; set; }
        /// <summary>
        /// 滑动过期的具体时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? SlidingAbsoluteDate { get; set; }
        /// <summary>
        /// 绝对过期时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ExpiressAbsoulte { get; set; }
    }
}
