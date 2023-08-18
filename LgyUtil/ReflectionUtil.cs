using LgyUtil.OtherSource;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace LgyUtil
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public sealed class ReflectionUtil
    {
        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="dll">程序集名称/dll名称/dll全路径</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string dll)
        {
            Assembly ass;
            if (dll.EndsWith(".dll"))
                ass = Assembly.LoadFrom(dll);
            else
                ass = Assembly.Load(dll);
            return ass;
        }
        /// <summary>
        /// 执行dll
        /// </summary>
        /// <param name="dllPath">dll名称或路径</param>
        /// <param name="className">全路径类名</param>
        /// <param name="funcName">方法名</param>
        /// <param name="param">执行参数</param>
        /// <returns></returns>
        public static object ExecDll(string dllPath, string className, string funcName, params object[] param)
        {
            Assembly assembly = GetAssembly(dllPath);
            return ExecAssembly(assembly, className, funcName, param);
        }
        /// <summary>
        /// 执行程序集
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <param name="className">全路径类名</param>
        /// <param name="funcName">方法名</param>
        /// <param name="param">执行参数</param>
        /// <returns></returns>
        public static object ExecAssembly(Assembly assembly, string className, string funcName, params object[] param)
        {
            Type t = assembly.GetType(className);
            if (t is null)
                throw new LgyUtilException($"反射错误：未找到名为{className}的类型");
            MethodInfo method = t.GetMethod(funcName);
            if (method is null)
                throw new LgyUtilException($"反射错误：未找到名为{funcName}的方法名");
            object ret;
            if (method.IsStatic)
            {
                ret = method.Invoke(null, param);
            }
            else
            {
                var instance = Activator.CreateInstance(t);
                ret = method.Invoke(instance, param);
            }
            return ret;
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(Assembly ass, string className, object[] param, bool ignoreCase = true) where T : class
        {
            Type t = ass.GetType(className, false, ignoreCase);
            if (t == null)
                throw new LgyUtilException($"反射错误：未找到名为{className}的类型");
            if ((param == null || param.Length == 0) && typeof(T) != typeof(object))//object类型，不能使用泛型返回，否则返回的对象不正确
                return Activator.CreateInstance<T>();
            return (T)Activator.CreateInstance(t, param);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(string dllName, string className, object[] param, bool ignoreCase = true) where T : class
        {
            Assembly ass = GetAssembly(dllName);
            if (ass is null)
                throw new LgyUtilException($"反射错误：未找到名为{dllName}的程序集");
            return GetInstance<T>(ass, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(string dllName, string className, bool ignoreCase = true) where T : class
        {
            return GetInstance<T>(dllName, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static T GetInstance<T>(Assembly ass, string className, bool ignoreCase = true) where T : class
        {
            return GetInstance<T>(ass, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(string dllName, string className, object[] param, bool ignoreCase = true)
        {
            return GetInstance<object>(dllName, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="param">实例化参数</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(Assembly ass, string className, object[] param, bool ignoreCase = true)
        {
            return GetInstance<object>(ass, className, param, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="dllName">dll的名称，或者路径</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(string dllName, string className, bool ignoreCase = true)
        {
            return GetInstance<object>(dllName, className, null, ignoreCase);
        }
        /// <summary>
        /// 反射获取实例
        /// </summary>
        /// <param name="ass">程序集</param>
        /// <param name="className">命名空间.类名</param>
        /// <param name="ignoreCase">查找类名时，是否忽略大小写</param>
        /// <returns></returns>
        public static object GetInstance(Assembly ass, string className, bool ignoreCase = true)
        {
            return GetInstance<object>(ass, className, null, ignoreCase);
        }

        #region 通过表达式树构建方法，并执行

        /// <summary>
        /// method缓存
        /// </summary>
        static Lazy<ConcurrentDictionary<string, Func<object, object[], object>>> dicGetSetMethodCache = new Lazy<ConcurrentDictionary<string, Func<object, object[], object>>>(true);
        /// <summary>
        /// 获取缓存的key
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        static string GetGetSetMethodKey(Type modelType, string methodName)
        {
            return $"{modelType.Namespace}.{modelType.Name}.{methodName}";
        }

        /// <summary>
        /// 给类的属性赋值(调用的是属性set方法)
        /// </summary>
        /// <param name="instance">类的实例</param>
        /// <param name="propName">属性名</param>
        /// <param name="setValue">赋值内容</param>
        /// <exception cref="LgyUtilException"></exception>
        public static void SetModelPropertyValue(object instance, string propName, object setValue)
        {
            var modelType = instance.GetType();

            string key = GetGetSetMethodKey(modelType, "Set_" + propName);
            if (dicGetSetMethodCache.Value.TryGetValue(key, out var func))
            {
                func(instance, new object[] { setValue });
                return;
            }

            var prop = modelType.GetProperty(propName);
            if (prop is null)
                throw new LgyUtilException($"属性名错误：{modelType.Name}.{propName}");
            var setMethod = prop.GetSetMethod();
            if (setMethod == null)
                throw new LgyUtilException($"属性没有Set方法：{modelType.Name}.{propName}");

            var func2 = ExpressionExecMethod.GetExecFuntion(setMethod);
            dicGetSetMethodCache.Value.TryAdd($"{modelType.Namespace}.{modelType.Name}.Set{propName}", func2);

            func2(instance, new object[] { setValue });
        }

        /// <summary>
        /// 获取类的属性值(调用的是属性的get方法)
        /// </summary>
        /// <param name="instance">类的实例</param>
        /// <param name="propName">属性名</param>
        /// <returns></returns>
        /// <exception cref="LgyUtilException"></exception>
        public static object GetModelPropertyValue(object instance, string propName)
        {
            var modelType = instance.GetType();

            string key = GetGetSetMethodKey(modelType, "Get_" + propName);
            if (dicGetSetMethodCache.Value.TryGetValue(key, out var func))
                return func(instance, null);

            var prop = modelType.GetProperty(propName);
            if (prop is null)
                throw new LgyUtilException($"属性名错误：{modelType.Name}.{propName}");
            var getMethod = prop.GetGetMethod();
            if (getMethod == null)
                throw new LgyUtilException($"属性没有Get方法：{modelType.Name}.{propName}");

            var func2 = ExpressionExecMethod.GetExecFuntion(getMethod);

            dicGetSetMethodCache.Value.TryAdd($"{modelType.Namespace}.{modelType.Name}.Set{propName}", func2);

            return func2(instance, null);
        }

        /// <summary>
        /// 执行类中的方法，也可以是静态方法
        /// </summary>
        /// <param name="instance">类实例，执行静态方法时填null</param>
        /// <param name="method">执行的方法</param>
        /// <param name="methodParams">方法参数</param>
        /// <returns></returns>
        public static object ExecMethod(object instance, MethodInfo method, params object[] methodParams)
        {
            return ExpressionExecMethod.GetExecFuntion(method)(instance, methodParams);
        }

        /// <summary>
        /// 执行类中的方法，也可以是静态方法
        /// </summary>
        /// <param name="instance">类实例，执行静态方式时填null</param>
        /// <param name="instanceType">类的类型</param>
        /// <param name="methodName">执行的方法名</param>
        /// <param name="methodParams">方法参数</param>
        /// <returns></returns>
        /// <exception cref="LgyUtilException"></exception>
        public static object ExecModelMethod(object instance, Type instanceType, string methodName, params object[] methodParams)
        {
            string key = GetGetSetMethodKey(instanceType, methodName);
            if(dicGetSetMethodCache.Value.TryGetValue((string)key, out var func)) 
                return func(instance, methodParams);

            var method = instanceType.GetMethod(methodName);
            if (method == null)
                throw new LgyUtilException($"没有找到方法：{instanceType.Name}.{methodName}");

            var func2 = ExpressionExecMethod.GetExecFuntion(method);
            dicGetSetMethodCache.Value.TryAdd(key, func2);

            return func2(instance, methodParams);
        }

        #endregion
    }
}
