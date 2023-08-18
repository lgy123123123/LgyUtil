using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LgyUtil.OtherSource
{
    /// <summary>
    /// 使用表达式，执行方法
    /// </summary>
    internal class ExpressionExecMethod
    {
        /// <summary>
        /// 缓存已生成过的方法
        /// </summary>
        static ConcurrentDictionary<MethodInfo, Func<object, object[], object>> dicMethodInfoCache = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();

        /// <summary>
        /// 通过表达式数，构建要执行的方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Func<object, object[], object> GetExecFuntion(MethodInfo method)
        {
            if(dicMethodInfoCache.TryGetValue(method, out var func))
                return func;

            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "instance");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");
            //构造参数
            List<Expression> parameterExpressions = new List<Expression>();
            ParameterInfo[] paramInfos = method.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);
                parameterExpressions.Add(valueCast);
            }
            //静态方法转换
            Expression instanceCast = method.IsStatic ? null : Expression.Convert(instanceParameter, method.ReflectedType);
            //构造返回值
            Func<object, object[], object> retFunc = null;
            MethodCallExpression methodCall = Expression.Call(instanceCast, method, parameterExpressions);
            if (methodCall.Type == typeof(void))//无返回值
            {
                Expression<Action<object, object[]>> lambda =
                    Expression.Lambda<Action<object, object[]>>(
                        methodCall, instanceParameter, parametersParameter);

                Action<object, object[]> execute = lambda.Compile();
                retFunc =(instance, parameters) =>
                {
                    execute(instance, parameters);
                    return null;
                };
            }
            else//有返回值
            {
                UnaryExpression castMethodCall = Expression.Convert(
                    methodCall, typeof(object));
                Expression<Func<object, object[], object>> lambda =
                    Expression.Lambda<Func<object, object[], object>>(
                        castMethodCall, instanceParameter, parametersParameter);

                retFunc = lambda.Compile();
            }

            dicMethodInfoCache.TryAdd(method, retFunc);
            
            return retFunc;
        }
    }
}
