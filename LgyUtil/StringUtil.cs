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
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, s, args);
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
        /// 字符串转日期
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s, string format = null)
        {
            if (format.IsNullOrEmpty())
                return DateTime.Parse(s);
            else
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
            return (TEnum)Enum.Parse(typeof(TEnum), s, ignoreCase);
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
        /// <returns></returns>
        public static bool RegexIsMatch(this string s, string pattern)
        {
            return Regex.IsMatch(s, pattern);
        }

        /// <summary>
        /// 是否匹配正则表达式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">RegexUtil里面的内容，也可自己写表达式</param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static bool RegexIsMatch(this string s, string pattern, RegexOptions option)
        {
            return Regex.IsMatch(s, pattern, option);
        }
        /// <summary>
        /// 将字符串下标序号内容，替换成指定字符串
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
            return s.Insert(startIndex, replaceContent).Remove(startIndex + replaceContent.Length, length);
        }
        /// <summary>
        /// 正则表达式替换字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="replacement">替换内容</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string s, string pattern, string replacement)
        {
            return Regex.Replace(s, pattern, replacement);
        }
        /// <summary>
        /// 正则表达式替换字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="replacement">替换内容</param>
        /// <param name="option">正则表达式选项</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string s, string pattern, string replacement, RegexOptions option)
        {
            return Regex.Replace(s, pattern, replacement, option);
        }
        /// <summary>
        /// 根据正则表达式获取内容
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern">正则表达式</param>
        /// <returns></returns>
        public static string GetStringByRegex(this string s, string pattern)
        {
            return Regex.Match(s, pattern).Value;
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
            return s.Split(new string[] { separator }, includeEmpty ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
        }
        /// <summary>
        /// 字符串模板，格式化内容(使用的NString)
        /// </summary>
        /// <param name="template">模板，参数用{name}表格</param>
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
        /// <summary>
        /// Trim扩展，可直接填写字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimString">清空两边的字符串</param>
        /// <returns></returns>
        public static string Trim(this string s,string trimString)
        {
            if(trimString.IsNullOrEmpty()) return s;
            return s.Trim(trimString.ToArray());
        }
        /// <summary>
        /// TrimStart扩展，可直接填写字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimString">清空起始的字符串</param>
        /// <returns></returns>
        public static string TrimStart(this string s,string trimString)
        {
            if (trimString.IsNullOrEmpty()) return s;
            return s.TrimStart(trimString.ToArray());
        }
        /// <summary>
        /// TrimEnd扩展，可直接填写字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="trimString">清空结尾的字符串</param>
        /// <returns></returns>
        public static string TrimEnd(this string s, string trimString)
        {
            if (trimString.IsNullOrEmpty()) return s;
            return s.TrimEnd(trimString.ToArray());
        }
    }
}
