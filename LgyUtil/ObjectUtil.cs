using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Mapster;
using Newtonsoft.Json;

namespace LgyUtil
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
        public static T CloneBinary<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new LgyUtilException("The type must be serializable.");
            }

            if (ReferenceEquals(source, null))
            {
                return default;
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
        /// <summary>
        /// 克隆对象，使用Mapster，只能克隆普通类(DataTable不能克隆)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T CloneMapster<T>(this T source) where T : class, new()
        {
            return source.Adapt<T>();
        }
        
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] compareObj)
        {
            return compareObj.Contains(obj);
        }
        /// <summary>
        /// 类似sql的in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, IEnumerable<T> compareObj)
        {
            return compareObj.Contains(obj);
        }

        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T obj, params T[] compareObj)
        {
            return !compareObj.Contains(obj);
        }

        /// <summary>
        /// 类似sql的not in方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public static bool NotIn<T>(this T obj, IEnumerable<T> compareObj)
        {
            return !compareObj.Contains(obj);
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

        #region 对象映射相关
        /// <summary>
        /// 普通类映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source">源对象</param>
        /// <param name="dest">目的对象，会覆盖这个对象的属性，可以传null</param>
        /// <returns>返回传入的目的对象</returns>
        public static TDestination MappingTo<TSource, TDestination>(this TSource source, TDestination dest) where TSource : class, new() where TDestination : class
        {
            return source.Adapt(dest);
        }

        /// <summary>
        /// 自定义映射
        /// 将类的属性，映射到另一个类，只能映射普通的类对象
        /// 自定义配置，查询文档(官方英文文档)https://github.com/MapsterMapper/Mapster
        /// (热心网友中文翻译文档)https://www.cnblogs.com/staneee/p/14912794.html
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source"></param>
        /// <param name="dest">目的对象，会覆盖这个对象的属性，可以传null</param>
        /// <param name="customConfig">映射配置，例如：setter=>{setter.Map(dest=>dest.Name,source=>source.name1).Ignore(dest=>dest.value1)}</param>
        /// <param name="key1">不用传</param>
        /// <param name="key2">不用传</param>
        /// <returns>返回传入的目的对象</returns>
        public static TDestination MappingTo<TSource, TDestination>(this TSource source, TDestination dest, Action<TypeAdapterSetter<TSource, TDestination>> customConfig, [CallerFilePath] string key1 = "", [CallerLineNumber] int key2 = 0) where TSource : class where TDestination : class
        {
            var config = TypeAdapterConfig.GlobalSettings.Fork((conf) =>
            {
                var setter = conf.ForType<TSource, TDestination>();
                customConfig(setter);
                setter.Config.Compile();
            }, key1, key2);
            return source.Adapt<TSource, TDestination>(dest, config);
        }
        /// <summary>
        /// 普通类映射
        /// 将类的属性，映射成另一个新的类
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source"></param>
        /// <returns>返回映射目的对象</returns>
        public static TDestination MappingTo<TDestination>(this object source) where TDestination : class
        {
            return source.Adapt<TDestination>();
        }
        /// <summary>
        /// 自定义映射
        /// 将类的属性，映射到另一个类，只能映射普通的类对象
        /// 自定义配置，查询文档(官方英文文档)https://github.com/MapsterMapper/Mapster
        /// (热心网友中文翻译文档)https://www.cnblogs.com/staneee/p/14912794.html
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="customConfig">映射配置，例如：setter=>{setter.Map&lt;TDestination,TSource&gt;(dest=>dest.Name,source=>source.name1).Ignore(dest=>dest.value1)}</param>
        /// <param name="key1">不用传</param>
        /// <param name="key2">不用传</param>
        /// <returns>返回映射目的对象</returns>
        public static TDestination MappingTo<TDestination>(this object source, Action<TypeAdapterSetter<TDestination>> customConfig, [CallerFilePath] string key1 = "", [CallerLineNumber] int key2 = 0) where TDestination : class
        {
            var config = TypeAdapterConfig.GlobalSettings.Fork((conf) =>
            {
                var setter = conf.ForDestinationType<TDestination>();
                customConfig(setter);
                setter.Config.Compile();
            }, key1, key2);
            return source.Adapt<TDestination>(config);
        }
        /// <summary>
        /// 普通类映射
        /// 将类的属性，映射到另一个新的类
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination">目的类型(注意，不是对象，是类型)</param>
        /// <returns>返回一个new目的类型 需要自己进行as 转换</returns>
        public static object MappingTo<TSource>(this TSource source, Type destination) where TSource : class
        {
            return source.Adapt(typeof(TSource), destination);
        }

        /// <summary>
        /// 数组，映射为List泛型数组
        /// </summary>
        /// <param name="source">数组对象</param>
        /// <typeparam name="TDestination">目的类型</typeparam>
        /// <returns></returns>
        public static List<TDestination> MappingToList<TDestination>(this IList source) where TDestination : class
        {
            return source.MappingTo<List<TDestination>>();
        }

        /// <summary>
        /// 数组，映射为
        /// </summary>
        /// <param name="source">数组对象</param>
        /// <typeparam name="TDestination">目的类型</typeparam>
        /// <returns></returns>
        public static TDestination[] MappingToArray<TDestination>(this IList source) where TDestination : class
        {
            return source.MappingTo<TDestination[]>();
        }
        #endregion
    }
}
