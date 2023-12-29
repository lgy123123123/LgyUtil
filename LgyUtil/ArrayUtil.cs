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
        public static void ForEach<T>(this T[] arr, Action<T> action)
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
        /// 尝试找到第一个匹配的结果
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="findValue">找到的第一个结果</param>
        /// <param name="match">查询条件</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>是否找到匹配项</returns>
        public static bool TryFind<T>(this IEnumerable<T> array, out T findValue, Func<T, bool> match)
        {
            if (array.IsNullOrEmpty())
            {
                findValue = default(T);
                return false;
            }

            var findObj = array.Where(match).FirstOrDefault();
            if (findObj != null)
            {
                findValue = findObj;
                return true;
            }

            findValue = default(T);
            return false;
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
        /// 尝试找到所有匹配的结果
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="findValues">找到的所有匹配项</param>
        /// <param name="match">匹配条件</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>是否找到匹配结果</returns>
        public static bool TryFindAll<T>(this IEnumerable<T> array, out IEnumerable<T> findValues, Func<T, bool> match)
        {
            if (array.IsNullOrEmpty())
            {
                findValues = null;
                return false;
            }

            var findObjs = array.Where(match);
            if (findObjs.HaveContent())
            {
                findValues = findObjs;
                return true;
            }

            findValues = null;
            return false;
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
        public static string JoinToString<T>(this IEnumerable<T> list, string separator = ",")
        {
            if (list == null)
                return "";
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
            if (list == null)
                return "";
            return list.Select(selectField).JoinToString(separator);
        }

        /// <summary>
        /// 数组包含某些项目，符合一个就返回true（解决了数组的Contains方法只能输入一个参数匹配的问题）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="compareObj">包含的内容</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> array, params T[] compareObj)
        {
            return array.Any(compareObj.Contains);
        }

        /// <summary>
        /// 数组包含某些项目，符合一个就返回true（解决了数组的Contains方法只能输入一个参数匹配的问题）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="compareObj">包含的数组</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> array, IEnumerable<T> compareObj)
        {
            return array.Any(compareObj.Contains);
        }

        /// <summary>
        /// 数组切片，在.net6出现了新的语法糖[1..2]可以替代这个方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="start">切片起始位置，从0开始</param>
        /// <param name="end">切片结束位置，结果包含结束位置这个内容</param>
        /// <returns></returns>
        public static IEnumerable<T> Slice<T>(this IEnumerable<T> array, int start, int end)
        {
            return array.Skip(start).Take(end - start + 1);
        }

        /// <summary>
        /// 判断数组不为空，并且有值(对于any方法的扩展)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool HaveContent<T>(this IEnumerable<T> array)
        {
            return array != null && array.Any();
        }

        /// <summary>
        /// 数组是否为空，数组为null，不报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> array)
        {
            return array is null || !array.Any();
        }

        /// <summary>
        /// 按照windows系统文件名排序规则排序，升序
        /// </summary>
        /// <param name="array">字符串数组</param>
        /// <returns></returns>
        public static IEnumerable<string> SortByWindowsFileName(this IEnumerable<string> array)
        {
            return FileUtil.SortByWindowsFileName(array);
        }

        /// <summary>
        /// 按照windows系统文件名排序规则排序，升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">要排序的对象数组</param>
        /// <param name="orderField">选择排序的字符串字段</param>
        /// <returns></returns>
        public static IEnumerable<T> SortByWindowsFileName<T>(this IEnumerable<T> array, Func<T, string> orderField) where T : class
        {
            return FileUtil.SortByWindowsFileName(array, orderField);
        }

        /// <summary>
        /// 按照windows系统文件名排序规则排序，降序
        /// </summary>
        /// <param name="array">字符串数组</param>
        /// <returns></returns>
        public static IEnumerable<string> SortByWindowsFileNameDesc(this IEnumerable<string> array)
        {
            return FileUtil.SortByWindowsFileNameDesc(array);
        }

        /// <summary>
        /// 按照windows系统文件名排序规则排序，降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">要排序的对象数组</param>
        /// <param name="orderField">选择排序的字符串字段</param>
        /// <returns></returns>
        public static IEnumerable<T> SortByWindowsFileNameDesc<T>(this IEnumerable<T> array, Func<T, string> orderField) where T : class
        {
            return FileUtil.SortByWindowsFileNameDesc(array, orderField);
        }

        /// <summary>
        /// 随机抽取数组中的项目，返回新的数组，且不改变原数组顺序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="randomCount">随机抽取的个数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetRandomArr<T>(this IEnumerable<T> array, int randomCount)
        {
            //随机取的数量，不能超过总数
            if (array.Count() < randomCount)
                randomCount = array.Count();
            Random ram = new Random();
            //最终返回的数组
            T[] retArr = new T[randomCount];
            //复制一份，用于排序
            var copyArr = array.Select(l => l).ToArray();
            for (int i = 0; i < randomCount; i++)
            {
                var index = ram.Next(0, copyArr.Length - i);
                retArr[i] = copyArr[index];
                copyArr[index] = copyArr[copyArr.Length - i - 1];
                copyArr[copyArr.Length - i - 1] = retArr[i];
            }

            return retArr;
        }
    }
}