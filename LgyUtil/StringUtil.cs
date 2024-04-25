using LgyUtil.NStringFormat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LgyUtil
{
    /// <summary>
    /// 字符串工具
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// 字符串是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 字符串trim后是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyTrim(this string s)
        {
            return s == null || string.IsNullOrEmpty(s.Trim());
        }

        /// <summary>
        /// 字符串是否为不为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 转换成base64的字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding">编码格式，默认是utf8</param>
        /// <returns></returns>
        public static string ToBase64String(string s, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(s));
        }

        /// <summary>
        /// 将base64字符串转成普通字符串
        /// </summary>
        /// <param name="base64str">base64字符串</param>
        /// <param name="encoding">编码格式，默认是utf8</param>
        /// <returns></returns>
        public static string ToStringFromBase64(string base64str, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return encoding.GetString(Convert.FromBase64String(base64str));
        }

        /// <summary>
        /// string.Format格式化字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string s, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, s, args);
        }

        /// <summary>
        /// 反序列化NewtonJson序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeserializeNewtonJson<T>(this string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

        /// <summary>
        /// 反序列化NewtonJson序列化的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T DeserializeNewtonJson<T>(this string s, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(s, settings);
        }

        /// <summary>
        /// 字符串转日期，支持季度格式，Q代表季度，只解析第一个Q
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format">日期格式，Q代表季度，只解析第一个Q</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s, string format = null)
        {
            if (format.IsNullOrEmpty())
                return DateTime.Parse(s);
            //季度
            if (format.Contains("Q"))
            {
                int quarterStrIndex = format.IndexOf("Q");
                //获取季度数字
                int quarterNum = s.Substring(quarterStrIndex, 1).ToInt();
                //去掉季度格式字符串
                format = format.Replace("Q", "");
                s = s.Remove(quarterStrIndex, 1);
                //执行正常格式化
                var dtTemp = DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
                //添加季度
                return dtTemp.AddQuarter(quarterNum - 1);
            }

            //其它正常格式
            return DateTime.ParseExact(s, format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 字符串转换成byte数组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encoding">默认是utf8格式</param>
        /// <returns></returns>
        public static byte[] ToByteArr(this string s, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return encoding.GetBytes(s);
        }

        /// <summary>
        /// byte数组转字符串
        /// </summary>
        /// <param name="b"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ByteToString(this byte[] b, Encoding encoding = null)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;
            return encoding.GetString(b);
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="s"></param>
        /// <param name="ignoreCase">是否忽略大小写，默认false</param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string s, bool ignoreCase = false) where TEnum : Enum
        {
            return (TEnum) Enum.Parse(typeof(TEnum), s, ignoreCase);
        }

        /// <summary>
        /// 删除微信昵称里的Emoji图片
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveEmoji(string str)
        {
            foreach (var a in str)
            {
                byte[] bts = Encoding.UTF32.GetBytes(a.ToString());

                if (bts[0].ToString() == "253" && bts[1].ToString() == "255")
                {
                    str = str.Replace(a.ToString(), "");
                }
            }

            return str;
        }

        /// <summary>
        /// 是否匹配正则表达式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">RegexUtil里面的内容，也可自己写表达式</param>
        /// <param name="options">正则表达式选项</param>
        /// <returns></returns>
        public static bool RegexIsMatch(this string s, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.IsMatch(s, pattern, options);
        }

        /// <summary>
        /// 指定索引位置，替换成其它字符串(手机号掩码时使用)
        /// </summary>
        /// <param name="s"></param>
        /// <param name="startIndex">替换起始索引序号</param>
        /// <param name="length">替换长度</param>
        /// <param name="replaceContent">替换内容</param>
        /// <returns></returns>
        public static string ReplaceByIndex(this string s, int startIndex, int length, string replaceContent)
        {
            if (s.IsNullOrEmpty() || startIndex < 0 || s.Length - 1 < startIndex)
                return s;
            if (s.Length < startIndex + length)
                length = s.Length - startIndex;
            return s.Remove(startIndex, length).Insert(startIndex, replaceContent);
        }

        /// <summary>
        /// 正则表达式替换字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="replacement">替换内容</param>
        /// <param name="option">正则表达式选项</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string s, string pattern, string replacement, RegexOptions option = RegexOptions.None)
        {
            return Regex.Replace(s, pattern, replacement, option);
        }

        /// <summary>
        /// 根据正则表达式获取内容
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="options">正则表达式选项</param>
        /// <returns></returns>
        public static string GetStringByRegex(this string s, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.Match(s, pattern, options).Value;
        }

        /// <summary>
        /// 分隔字符串，返回数组
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">字符串分隔符</param>
        /// <param name="includeEmpty">空内容是否包含在返回数组中</param>
        /// <returns></returns>
        public static string[] Split(this string s, string separator, bool includeEmpty = true)
        {
            return s.Split(new string[] {separator}, includeEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 字符串模板，格式化内容(使用的NString)，模板中若有{}，使用{{ }}，否则会被识别为模板而报错
        /// </summary>
        /// <param name="template">模板，参数用{name}表示，若有{}，使用{{ }}，否则会被识别为模板而报错</param>
        /// <param name="obj">模板内的对象，键值对</param>
        /// <param name="throwOnMissingValue">缺少对象时报错，默认true</param>
        /// <returns></returns>
        public static string FormatTemplate(this string template, IDictionary<string, object> obj, bool throwOnMissingValue = true)
        {
            return StringTemplate.Format(template, obj, throwOnMissingValue);
        }

        /// <summary>
        /// 字符串模板，格式化内容(使用的NString)
        /// </summary>
        /// <param name="template">模板，参数用{name}表格</param>
        /// <param name="obj">模板内的对象，可以使用匿名类，或者直接是类对象</param>
        /// <param name="throwOnMissingValue">缺少对象时报错，默认true</param>
        /// <returns></returns>
        public static string FormatTemplate(this string template, object obj, bool throwOnMissingValue = true)
        {
            return StringTemplate.Format(template, obj, throwOnMissingValue);
        }

        #region Trim扩展

        /// <summary>
        /// Trim扩展，可直接填写字符串，若替换一次后，开头或结尾还有要trim的内容，不再进行替换
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimStrings">清空两边的字符串</param>
        /// <returns></returns>
        public static string Trim(this string s, params string[] trimStrings)
        {
            if (trimStrings.IsNullOrEmpty())
                return s;
            string trimRegex = GetTrimRegex(trimStrings, ts => $"(^{Regex.Escape(ts)})|({Regex.Escape(ts)}$)");
            if (trimRegex.IsNullOrEmpty())
                return s;
            return s.ReplaceRegex(trimRegex, "", RegexOptions.Compiled);
        }

        /// <summary>
        /// TrimStart扩展，可直接填写字符串，若替换一次后，开头还有要trim的内容，不再进行替换
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimStrings">清空起始的字符串</param>
        /// <returns></returns>
        public static string TrimStart(this string s, params string[] trimStrings)
        {
            if (trimStrings.IsNullOrEmpty())
                return s;
            string trimRegex = GetTrimRegex(trimStrings, ts => $"(^{Regex.Escape(ts)})");
            if (trimRegex.IsNullOrEmpty())
                return s;
            return s.ReplaceRegex(trimRegex, "", RegexOptions.Compiled);
        }

        /// <summary>
        /// TrimEnd扩展，可直接填写字符串，若替换一次后，结尾还有要trim的内容，不再进行替换
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimStrings">清空结尾的字符串</param>
        /// <returns></returns>
        public static string TrimEnd(this string s, params string[] trimStrings)
        {
            if (trimStrings.IsNullOrEmpty())
                return s;
            string trimRegex = GetTrimRegex(trimStrings, ts => $"({Regex.Escape(ts)}$)");
            if (trimRegex.IsNullOrEmpty())
                return s;
            return s.ReplaceRegex(trimRegex, "", RegexOptions.Compiled);
        }

        private static string GetTrimRegex(string[] trimStrings, Func<string, string> replaceFunc)
        {
            return trimStrings.Where(ts => ts.IsNotNullOrEmpty()).JoinToString(replaceFunc, "|");
        }

        #endregion

        /// <summary>
        /// EndsWith扩展，可以匹配多个
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EndsWith(this string s, params string[] value)
        {
            if (value.Length == 0) return false;
            return value.Any(s.EndsWith);
        }

        /// <summary>
        /// StartsWith扩展，可以匹配多个
        /// </summary>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWith(this string s, params string[] value)
        {
            if (value.Length == 0) return false;
            return value.Any(s.StartsWith);
        }

        /// <summary>
        /// 字符串包含任意一个匹配项，就返回true
        /// </summary>
        /// <param name="s"></param>
        /// <param name="containsObj"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string s, params string[] containsObj)
        {
            return containsObj.Any(s.Contains);
        }

        /// <summary>
        /// 字符串包含任意一个匹配项，就返回true
        /// </summary>
        /// <param name="s"></param>
        /// <param name="containsObj"></param>
        /// <returns></returns>
        public static bool ContainsAny(this string s, IEnumerable<string> containsObj)
        {
            return containsObj.Any(s.Contains);
        }
    }
}