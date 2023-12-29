using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LgyUtil
{
    /// <summary>
    /// 模型验证帮助类
    /// System.ComponentModel.DataAnnotations
    /// </summary>
    public sealed class ModelCheckUtil
    {
        /// <summary>
        /// 验证模型，返回验证结果，自行处理
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">验证的模型</param>
        /// <returns></returns>
        public static ValidResult CheckModelResult<T>(T model) where T : class, new()
        {
            ValidResult result = new ValidResult();
            CheckModelDetail(model, result);
            return result;
        }

        /// <summary>
        /// 需要验证模型的参数
        /// key:Type.FullName  value:需要验证的参数
        /// </summary>
        private static ConcurrentDictionary<string, NeedCheckProp> dicIsCheckType = new ConcurrentDictionary<string, NeedCheckProp>();

        /// <summary>
        /// 具体验证的方法
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="result">结果</param>
        private static void CheckModelDetail(object model, ValidResult result)
        {
            if (model is null)
                return;
            try
            {
                var validationContext = new ValidationContext(model);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(model, validationContext, results, true);

                if (!isValid)
                {
                    result.IsVaild = false;
                    foreach (var item in results)
                    {
                        result.ErrorMembers.Add(new ErrorMember()
                        {
                            ErrorMessage = item.ErrorMessage,
                            ErrorMemberName = item.MemberNames.FirstOrDefault()
                        });
                    }
                }

                //深度验证，查找属性是类、泛型的
                var modelType = model.GetType();
                if (modelType.GetCustomAttribute<DeepCheckAttribute>() != null)
                {
                    //查询需要验证的属性
                    if (!dicIsCheckType.TryGetValue(modelType.FullName, out var checkInfo))
                    {
                        checkInfo = new NeedCheckProp();
                        var props = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p as MemberInfo);
                        var fields = modelType.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(p => p as MemberInfo);
                        List<MemberInfo> members = new List<MemberInfo>();
                        members.AddRange(props);
                        members.AddRange(fields);
                        foreach (var mem in members)
                        {

                            Type mType;
                            if (mem.MemberType == MemberTypes.Field)
                                mType = (mem as FieldInfo).FieldType;
                            else
                                mType = (mem as PropertyInfo).PropertyType;

                            if (mType == typeof(string))
                                continue;

                            if (mType.IsClass)
                            {
                                //数组、泛型数组
                                if (mType.GetInterface(typeof(IEnumerable).FullName) != null)
                                {
                                    //不支持dictionary
                                    if (mType.GetInterface(typeof(IDictionary).FullName) != null)
                                        continue;

                                    //数组对象类型
                                    Type eleType = null;
                                    if (mType.IsArray)
                                        eleType = mType.GetElementType();
                                    //泛型数组
                                    else
                                        eleType = mType.GenericTypeArguments[0];

                                    //要类和非字符串
                                    if (eleType.IsClass && eleType != typeof(string))
                                        checkInfo.ArrayProp.Add(mem);
                                }
                                //普通类
                                else
                                {
                                    //系统类，不验证
                                    if (mType.FullName.StartsWith("System."))
                                        continue;
                                    checkInfo.ClassProp.Add(mem);
                                }
                            }
                        }
                        dicIsCheckType.TryAdd(modelType.FullName, checkInfo);
                    }
                    //数组类型深度验证
                    checkInfo.ArrayProp.ForEach(p =>
                    {
                        object val;
                        if (p.MemberType == MemberTypes.Field)
                            val = (p as FieldInfo).GetValue(model);
                        else
                            val = (p as PropertyInfo).GetValue(model);

                        var arr = val as IEnumerable;
                        if (arr == null)
                            return;
                        foreach (var item in arr)
                        {
                            CheckModelDetail(item, result);
                        }
                    });
                    //普通类的深度验证
                    checkInfo.ClassProp.ForEach(p =>
                    {
                        object val;
                        if (p.MemberType == MemberTypes.Field)
                            val = (p as FieldInfo).GetValue(model);
                        else
                            val = (p as PropertyInfo).GetValue(model);

                        if (val == null)
                            return;
                        CheckModelDetail(val, result);
                    });
                }
            }
            catch (Exception ex)
            {
                result.IsVaild = false;
                result.ErrorMembers.Add(new ErrorMember() { ErrorMessage = ex.Message, ErrorMemberName = "" });
            }
        }

        /// <summary>
        /// 验证模型，返回是否通过
        /// System.ComponentModel.DataAnnotations中的验证
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">验证的模型</param>
        /// <param name="deepCheck">深度验证(当属性是类、数组，对其也进行验证)</param>
        /// <returns></returns>
        public static bool CheckModel<T>(T model, bool deepCheck = false) where T : class, new()
        {
            return CheckModelResult(model).IsVaild;
        }

        /// <summary>
        /// 方法缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, ParameterInfo[]> dicParamInfo =
            new ConcurrentDictionary<string, ParameterInfo[]>();

        /// <summary>
        /// 验证方法参数(静态方法不可用)
        /// System.ComponentModel.DataAnnotations中的验证
        /// 使用方法：在要验证的方法内部this.CheckMethodParamsResult(nameof(方法名),.....按顺序放入方法的参数)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="methodName">方法名，使用nameof(方法)</param>
        /// <param name="paramsObjs">方法参数具体值，按顺序放入，注意:如果是List，请ToArray()</param>
        /// <returns></returns>
        public static ValidResult CheckMethodParamsResult<T>(T model, string methodName, params object[] paramsObjs) where T : class, new()
        {
            var modelType = model.GetType();
            string modelName = modelType.FullName;
            if (!dicParamInfo.ContainsKey(modelName))
            {
                var methodInfo = modelType.GetMethod(methodName);
                if (methodInfo is null)
                    throw new Exception("验证方法参数的方法名不正确");
                else
                    dicParamInfo.TryAdd(modelName, methodInfo.GetParameters());
            }
            return CheckMethodParamsResult(model, dicParamInfo[modelName], paramsObjs);
        }

        /// <summary>
        /// 验证方法参数(静态方法不可用)
        /// System.ComponentModel.DataAnnotations中的验证
        /// 使用方法：在要验证的方法内部this.CheckMethodParamsResult(nameof(方法名),.....按顺序放入方法的参数)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="paramInfos">方法的参数信息</param>
        /// <param name="paramsObjs">方法参数具体值，按顺序放入，注意:如果是List，请ToArray()</param>
        /// <returns></returns>
        public static ValidResult CheckMethodParamsResult<T>(T model, ParameterInfo[] paramInfos, params object[] paramsObjs) where T : class, new()
        {
            ValidResult result = new ValidResult();
            try
            {
                for (var i = 0; i < paramsObjs.Length; i++)
                {
                    var paramInfo = paramInfos[i];
                    var value = paramsObjs[i];
                    if (paramInfo.ParameterType.IsClass && paramInfo.ParameterType != typeof(string))//类处理，用上面的方法
                    {
                        var results = CheckModelResult(value);
                        if (!results.IsVaild)
                        {
                            result.IsVaild = false;
                            result.ErrorMembers.AddRange(results.ErrorMembers);
                        }
                    }
                    else//非类处理
                    {
                        var attrValid = paramInfo.GetCustomAttributes<ValidationAttribute>();
                        if (!attrValid.Any())
                            continue;
                        var context = new ValidationContext(paramInfo);
                        var results = new List<ValidationResult>();
                        var isValid = Validator.TryValidateValue(value, context, results, attrValid);
                        if (!isValid)
                        {
                            result.IsVaild = false;
                            foreach (var item in results)
                            {
                                result.ErrorMembers.Add(new ErrorMember()
                                {
                                    ErrorMessage = item.ErrorMessage,
                                    ErrorMemberName = item.MemberNames.FirstOrDefault()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsVaild = false;
                result.ErrorMembers.Add(new ErrorMember() { ErrorMessage = ex.Message, ErrorMemberName = "" }); ;
            }

            return result;
        }
        /// <summary>
        /// 验证方法参数(静态方法不可用)
        /// System.ComponentModel.DataAnnotations中的验证
        /// 使用方法：在要验证的方法内部this.CheckMethodParamsResult(nameof(方法名),.....按顺序放入方法的参数)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="paramInfos">方法的参数信息</param>
        /// <param name="paramsObjs">方法参数具体值，按顺序放入，注意:如果是List，请ToArray()</param>
        /// <returns></returns>
        public static bool CheckMethodParams<T>(T model, ParameterInfo[] paramInfos, params object[] paramsObjs) where T : class, new()
        {
            return CheckMethodParamsResult(model, paramInfos, paramsObjs).IsVaild;
        }
        /// <summary>
        /// 验证方法参数(静态方法不可用)
        /// System.ComponentModel.DataAnnotations中的验证
        /// 使用方法：在要验证的方法内部this.CheckMethodParamsResult(nameof(方法名),.....按顺序放入方法的参数)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="methodName">方法名，使用nameof(方法)</param>
        /// <param name="paramsObjs">方法参数具体值，按顺序放入，注意:如果是List，请ToArray()</param>
        /// <returns></returns>
        public static bool CheckMethodParams<T>(T model, string methodName, params object[] paramsObjs) where T : class, new()
        {
            return CheckMethodParamsResult(model, methodName, paramsObjs).IsVaild;
        }
    }

    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidResult
    {
        /// <summary>
        /// 错误结果
        /// </summary>
        public List<ErrorMember> ErrorMembers { get; set; } = new List<ErrorMember>();
        /// <summary>
        /// 验证是否通过
        /// </summary>
        public bool IsVaild { get; set; } = true;
    }
    /// <summary>
    /// 报错信息
    /// </summary>
    public class ErrorMember
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// 发生错误的变量名
        /// </summary>
        public string ErrorMemberName { get; set; }
    }

    /// <summary>
    /// 模型中，需要检测的属性
    /// </summary>
    internal class NeedCheckProp
    {
        /// <summary>
        /// 数组属性
        /// </summary>
        public List<MemberInfo> ArrayProp { get; set; } = new List<MemberInfo>(0);
        /// <summary>
        /// 类属性
        /// </summary>
        public List<MemberInfo> ClassProp { get; set; } = new List<MemberInfo>(0);
    }

    /// <summary>
    /// 是否对类中的所有属性进行深度验证(仅支持普通类、一维数组、一维泛型数组)
    /// 只对使用ModelCheckUtil中的验证方法，才有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DeepCheckAttribute : Attribute
    {
    }
}
