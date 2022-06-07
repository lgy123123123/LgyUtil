using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LgyUtil
{
    public static class StringTrimUtil
    {
        /// <summary>
        /// trim类的缓存
        /// </summary>
        static readonly ConcurrentDictionary<string, List<PropertyInfo>> dicTrimCache = new ConcurrentDictionary<string, List<PropertyInfo>>();
        /// <summary>
        /// 对所有标记StringTrimAttribute的字符串进行trim
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public static void TrimAll<T>(this T model) where T : class, new()
        {
            var t = model.GetType();
            List<PropertyInfo> NeedTrimList;
            if (!dicTrimCache.TryGetValue(t.FullName, out NeedTrimList))
            {
                var thisAttr = t.GetCustomAttribute<StringTrimAttribute>();
                bool isTrimAll = thisAttr != null && !thisAttr.NotTrim;
                var props = t.GetProperties().ToList().FindAll(p => p.PropertyType == typeof(string));
                NeedTrimList = new List<PropertyInfo>();
                foreach (var propInfo in props)
                {
                    var propAttr = propInfo.GetCustomAttribute<StringTrimAttribute>();
                    if (isTrimAll || propAttr != null)
                    {
                        if (propAttr != null && propAttr.NotTrim)//标记nottrim的不要
                            continue;
                        NeedTrimList.Add(propInfo);
                    }
                }
                NeedTrimList.TrimExcess();
                dicTrimCache.TryAdd(t.FullName, NeedTrimList);
            }

            if (NeedTrimList.Count == 0) return;
            NeedTrimList.ForEach(p =>
            {
                object str = p.GetValue(model);
                if (str != null)
                {
                    p.SetValue(model, (str as string).Trim());
                }
            });
        }
    }
}
