using System;
using System.Collections.Generic;
using System.Linq;

namespace LgyUtil
{
    /// <summary>
    /// 数组帮助类
    /// </summary>
    public static class ArrayUtil
    {
        /// <summary>
        /// 循环对象数组，就是调用了Array.ForEach
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this T[] arr,Action<T> action)
        {
            Array.ForEach(arr, action);
        }
        /// <summary>
        /// 数组存在某个条件的对象，就是调用了Array.Exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static bool Exists<T>(this T[] array, Predicate<T> match)
        {
            return Array.Exists(array, match);
        }
        /// <summary>
        /// 找到一地个匹配的结果，就是调用了Array.Find
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T Find<T>(this T[] array, Predicate<T> match)
        {
            return Array.Find(array, match);
        }
        /// <summary>
        /// 找到所有匹配的结果，就是调用了Array.FindAll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindAll(array, match);
        }
        /// <summary>
        /// 找到匹配结果的索引，就是调用了Array.FindIndex
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this T[] array, Predicate<T> match)
        {
            return Array.FindIndex(array, match);
        }
        /// <summary>
        /// 排序，就是调用了Array.Sort
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="comparison">委托排序方法</param>
        public static void Sort<T>(this T[] array, Comparison<T> comparison)
        {
            Array.Sort(array, comparison);
        }
        /// <summary>
        /// 强制转换数组内容
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="array"></param>
        /// <param name="converter">委托转换方法，可以直接写int.Parse之类</param>
        /// <returns></returns>
        public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(array, converter);
        }
        /// <summary>
        /// 数组拼接成字符串，默认逗号拼接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator">字符串分隔符，默认逗号</param>
        /// <returns></returns>
        public static string JoinToString<T>(this T[] list, string separator = ",")
        {
            return string.Join(separator, list);
        }
        /// <summary>
        /// 数组拼接成字符串，默认逗号拼接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selectField">筛选拼接的字段</param>
        /// <param name="separator">字符串分隔符，默认逗号</param>
        /// <returns></returns>
        public static string JoinToString<T>(this T[] list, Func<T, object> selectField, string separator = ",")
        {
            return list.Select(selectField).JoinToString(separator);
        }
        /// <summary>
        /// 数组拼接成字符串，默认逗号拼接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator">字符串分隔符，默认逗号</param>
        /// <returns></returns>
        public static string JoinToString<T>(this IEnumerable<T> list, string separator = ",")
        {
            return string.Join(separator, list);
        }

        /// <summary>
        /// 数组拼接成字符串，默认逗号拼接
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selectField">筛选拼接的字段</param>
        /// <param name="separator">字符串分隔符，默认逗号</param>
        /// <returns></returns>
        public static string JoinToString<T>(this IEnumerable<T> list, Func<T, object> selectField, string separator = ",")
        {
            return list.Select(selectField).JoinToString(separator);
        }

        /// <summary>
        /// 数组包含某些项目，符合一个就返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="compareObj">包含的内容</param>
        /// <returns></returns>
        public static bool Contains2<T>(this IEnumerable<T> array, params T[] compareObj)
        {
            return array.Any(compareObj.Contains);
        }
    }
}
