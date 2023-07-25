using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace LgyUtil
{
    /// <summary>
    /// 配置项初始化帮助类，支持热重载
    /// </summary>
    public sealed class AppSettingUtil
    {
        /// <summary>
        /// 初始化配置对象类，并返回实例
        /// <para>使用的是newtonjson进行反序列化</para>
        /// </summary>
        /// <param name="jsonPath">配置文件绝对路径</param>
        /// <param name="hotReload">是否启动热重载</param>
        /// <param name="afterInitOrReload">初始化或重载后执行的方法</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Init<T>(string jsonPath, bool hotReload = false, Action<T> afterInitOrReload = null) where T : class, new()
        {
            var jsonObj = GetJobject(jsonPath);
            var setting = jsonObj.ToObject<T>();
            if (hotReload)
                SetupHotReload(jsonPath, setting, afterInitOrReload, typeof(T));
            afterInitOrReload?.Invoke(setting);
            return setting;
        }

        /// <summary>
        /// 初始化配置类，只赋值 public static 这种静态属性
        /// <para>使用的是newtonjson进行反序列化</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath">配置文件绝对路径</param>
        /// <param name="hotReload">是否启动热重载</param>
        /// <param name="afterInitOrReload">初始化或重载后执行的方法</param>
        public static void InitStatic<T>(string jsonPath, bool hotReload = false, Action<T> afterInitOrReload = null) where T : class, new()
        {
            var jsonObj = GetJobject(jsonPath);
            //自动解析配置项
            Type settingType = typeof(T);
            PropertyInfo[] props = typeof(T).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            props.ForEach(p =>
            {
                if (jsonObj.ContainsKey(p.Name))
                {
                    p.SetValue(null, jsonObj[p.Name].ToObject(p.PropertyType));
                }
            });
            afterInitOrReload?.Invoke(null);
            if (hotReload)
                SetupHotReload(jsonPath, null, afterInitOrReload, settingType);
        }

        /// <summary>
        /// 获取配置文件的JObject
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static JObject GetJobject(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                throw new LgyUtilException("没有找到配置文件");
            return JObject.Parse(FileUtil.ReadFileShare(jsonPath));
        }

        /// <summary>
        /// 准备热重载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonPath"></param>
        /// <param name="setting"></param>
        /// <param name="afterInitOrReload"></param>
        /// <param name="t"></param>
        private static void SetupHotReload<T>(string jsonPath, T setting, Action<T> afterInitOrReload, Type t) where T : class, new()
        {
            //监视文件变化
            FileUtil.WatchFileChanged(Path.GetDirectoryName(jsonPath), arg =>
            {
                if (arg.ChangeType != WatcherChangeTypes.Changed)
                    return;
                var jsonObj = GetJobject(jsonPath);
                if (setting == null)
                {
                    var props = t.GetProperties(BindingFlags.Public | BindingFlags.Static);
                    props.ForEach(p =>
                    {
                        if (jsonObj.ContainsKey(p.Name))
                            p.SetValue(null, jsonObj[p.Name].ToObject(p.PropertyType));
                    });
                    afterInitOrReload?.Invoke(setting);
                }
                else
                {
                    lock (setting)
                    {
                        var props = t.GetProperties(BindingFlags.Public|BindingFlags.Instance);
                        props.ForEach(p =>
                        {
                            if(jsonObj.ContainsKey(p.Name))
                                p.SetValue(setting, jsonObj[p.Name].ToObject(p.PropertyType));
                        });
                        afterInitOrReload?.Invoke(setting);
                    }
                }
            }, Path.GetFileName(jsonPath));
        }
    }
}
