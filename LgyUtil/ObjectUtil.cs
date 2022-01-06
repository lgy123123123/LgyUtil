﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Mapster;
using Newtonsoft.Json;

namespace System
{
    /// <summary>
    /// Object扩展方法
    /// </summary>
    public static class ObjectUtil
    {
        /// <summary>
        /// 克隆对象（按照NewtonJson方式）
        /// </summary>
        /// <typeparam name="T">可序列化的类</typeparam>
        /// <param name="obj"></param>
        /// <returns>克隆后的对象</returns>
        public static T CloneNewtonJson<T>(this T obj)
        {
            var str = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(str);
        }
        /// <summary>
        /// 克隆对象（二进制方法），需要给克隆的类以及这个类中使用的类都加上[Serializable]特性，否则会有异常！！！
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        #region In
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this string obj, params string[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this int obj, params int[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this float obj, params float[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this double obj, params double[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this decimal obj, params decimal[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In(this byte obj, params byte[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] compareObj) where T : struct
        {
            return compareObj.Contains(obj);
        }
        #endregion

        #region NotIn
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this string obj, params string[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this int obj, params int[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this float obj, params float[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this double obj, params double[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this decimal obj, params decimal[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn(this byte obj, params byte[] compareObj)
        {
            return !compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T obj, params T[] compareObj) where T : struct
        {
            return !compareObj.Contains(obj);
        }
        #endregion
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
        /// 比较两个对象的内容是否相同(通过newtonjson序列化)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool EqualClassContent<T>(this T t1, T t2) where T : class, new()
        {
            return t1.SerializeNewtonJson() == t2.SerializeNewtonJson();
        }
        /// <summary>
        /// 比较两个对象的内容是否相同(通过xml序列化)
        /// <para>！！！！注意类中只能有普通类型或list,否则会报错</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool EqualClassConentByXml<T>(this T t1, T t2) where T : class, new()
        {
            return t1.SerializeByXml() == t2.SerializeByXml();
        }
        #region (反)序列化
        /// <summary>
        /// 序列化(按照NewtonJson方式序列化)，日期格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isFormat">格式化序列化后的字符串，默认false</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeNewtonJson(this object obj, bool isFormat = false)
        {
            return JsonConvert.SerializeObject(obj, isFormat ? Formatting.Indented : Formatting.None, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd HH:mm:ss" });
        }
        /// <summary>
        /// 序列化(按照NewtonJson方式序列化)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="setting">序列化配置</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeNewtonJson(this object obj, JsonSerializerSettings setting)
        {
            return JsonConvert.SerializeObject(obj, setting);
        }
        /// <summary>
        /// 序列化(按照NewtonJson方式序列化)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isFormat">格式化序列化后的字符串，默认false</param>
        /// <param name="setting">序列化配置</param>
        /// <returns>序列化后的字符串</returns>
        public static string SerializeNewtonJson(this object obj, bool isFormat, JsonSerializerSettings setting)
        {
            return JsonConvert.SerializeObject(obj, isFormat ? Formatting.Indented : Formatting.None, setting);
        }
        /// <summary>
        /// 序列化成xml字符串(类中只能有普通类型或list可以进行序列化)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeByXml<T>(this T obj) where T : class, new()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, obj);
                return textWriter.ToString();
            }
        }
        #endregion

        #region 类型转换

        /// <summary>
        /// Convert.ToInt32，参数为true时忽略转换失败
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static int ToInt(this object obj, bool ignoreError = false)
        {
            try { return Convert.ToInt32(obj); }
            catch { if (ignoreError) return 0; else throw; }
        }
        /// <summary>
        /// Nullable int
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int? ToIntNullable(this object obj)
        {
            int? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToInt32(obj);
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// Convert.ToDouble，参数为true时忽略转换失败
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, bool ignoreError = false)
        {
            try { return Convert.ToDouble(obj); }
            catch { if (ignoreError) return 0; else throw; }
        }
        /// <summary>
        /// Nullable double
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double? ToDoubleNullable(this object obj)
        {
            double? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToDouble(obj);
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// Convert.ToInt64，参数为true时忽略转换失败
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static long ToLong(this object obj, bool ignoreError = false)
        {
            try { return Convert.ToInt64(obj); }
            catch { if (ignoreError) return 0; else throw; }
        }
        /// <summary>
        /// Nullable long
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long? ToLongNullable(this object obj)
        {
            long? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToInt64(obj);
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// Convert.ToDecimal，参数为true时忽略转换失败
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, bool ignoreError = false)
        {
            try { return Convert.ToDecimal(obj); }
            catch { if (ignoreError) return 0; else throw; }
        }
        /// <summary>
        /// Nullable decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static decimal? ToDecimalNullable(this object obj)
        {
            decimal? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToDecimal(obj);
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// Convert.ToSigle，参数为true时忽略转换失败
        /// 谨慎使用！！！由于浮点数精度问题，转换结果可能有问题，不建议使用float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreError">忽略错误，转换错误时，返回0</param>
        /// <returns></returns>
        public static float ToFloat(this object obj, bool ignoreError = false)
        {
            try { return Convert.ToSingle(obj); }
            catch { if (ignoreError) return 0; else throw; }
        }
        /// <summary>
        /// Nullable float
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float? ToFloatNullable(this object obj)
        {
            float? ret = null;
            try
            {
                if (!(obj is null))
                    ret = Convert.ToSingle(obj);
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// 转换成bool类型，1或true，返回true
        /// 支持string,int,double,float,decimal,bool,byte,sbyte类型
        /// 转换失败会报错
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToBool(this object obj)
        {
            if (obj == null)
                return false;
            switch (obj)
            {
                case string s: return s.ToLower() == "true" || s == "1";
                case int i: return i == 1;
                case double d: return d == 1;
                case float f: return f == 1;
                case decimal de: return de == 1;
                case bool b: return b;
                case byte bt: return bt == 1;
                case sbyte sb: return sb == 1;
            }

            throw new Exception($"{obj}转换bool失败");
        }
        /// <summary> 
        /// 将一个object对象序列化，返回一个byte[]
        /// 若是一个类，则需要给类打上[Serializable]标签
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>         
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter(); formatter.Serialize(ms, obj); return ms.GetBuffer();
            }
        }

        /// <summary> 
        /// 将一个序列化后的byte[]数组还原
        /// 若是一个类，则需要给类打上[Serializable]标签
        /// </summary>
        /// <param name="Bytes"></param>         
        /// <returns></returns> 
        public static object ToObject(this byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                IFormatter formatter = new BinaryFormatter(); return formatter.Deserialize(ms);
            }
        }
        /// <summary>
        /// 数字转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int i) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), i))
            {
                return (T)Enum.ToObject(typeof(T), i);
            }
            return default;
        }
        #endregion

        #region 对象映射相关
        /// <summary>
        /// 自定义映射
        /// 将类的属性，映射到另一个类，只能映射普通的类对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source"></param>
        /// <param name="dest">映射目的对象</param>
        /// <param name="customConfig">里面的对象，可以.Map(s=>s.Name,d=>d.name1).Map(s=>s.Value,d=>d.value1)来配置自定义映射</param>
        /// <returns>返回传入的目的对象</returns>
        public static TDestination MappingTo<TSource, TDestination>(this TSource source, TDestination dest, Action<TypeAdapterSetter<TSource, TDestination>> customConfig = null) where TSource : class, new() where TDestination : class, new()
        {
            TypeAdapterSetter<TSource, TDestination> setter = TypeAdapterConfig.GlobalSettings.ForType<TSource, TDestination>();
            customConfig?.Invoke(setter);
            return source.Adapt<TSource,TDestination>(dest);
        }
        /// <summary>
        /// 普通类映射
        /// 将类的属性，映射成另一个新的类
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination MappingTo<TDestination>(this object source) where TDestination : class, new()
        {
            return source.Adapt<TDestination>();
        }
        /// <summary>
        /// 普通类映射
        /// 将类的属性，映射到另一个新的类
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination">目的类型(注意，不是对象，是类型)</param>
        /// <returns>返回一个new目的类型 需要自己进行as 转换</returns>
        public static object MappingTo<TSource>(this TSource source,Type destination) where TSource : class, new()
        {
            return source.Adapt(typeof(TSource),destination);
        }
        #endregion

        #region 字符串trim
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
        #endregion

    }

    /// <summary>
    /// 将字符串进行Trim，放在class上，将class的所有字符串进行trim
    /// 可以new的模型才可以使用
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class,
        AllowMultiple = false, Inherited = true)]
    public class StringTrimAttribute : Attribute
    {
        /// <summary>
        /// 不用进行trim
        /// </summary>
        public bool NotTrim { get; set; } = false;
    }
}
