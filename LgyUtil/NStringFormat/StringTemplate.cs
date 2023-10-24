using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LgyUtil.NStringFormat
{
    internal class StringTemplate
    {
        private readonly string _template;

        private readonly string _templateWithIndexes;

        private readonly IList<string> _placeholders;

        private static readonly Regex TemplateRegex = new Regex("(?<open>{+)(?<key>\\w+)\\s*(?<alignment>,\\s*-?\\d+)?\\s*(?<format>:[^}]+)?(?<close>}+)");

        private static readonly ConcurrentDictionary<string, StringTemplate> TemplateCache = new ConcurrentDictionary<string, StringTemplate>();

        private static readonly ConcurrentDictionary<Type, IStringTemplateValueConverter> ValueConverterCache = new ConcurrentDictionary<Type, IStringTemplateValueConverter>();

        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> GettersCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();

        private static readonly Lazy<MethodInfo> ConvertStringTemplateValueMethodInfo = new Lazy<MethodInfo>(() => typeof(IStringTemplateValueConverter).GetRuntimeMethod("Convert", new Type[1] { typeof(object) }));

        private StringTemplate(string template)
        {
            _template = template;
            ParseTemplate(out _templateWithIndexes, out _placeholders);
        }

        public static StringTemplate Parse(string template)
        {
            return GetTemplate(template);
        }

        public static implicit operator StringTemplate(string s)
        {
            return GetTemplate(s);
        }

        public override string ToString()
        {
            return _template;
        }
        public string Format(IDictionary<string, object> values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            object[] array = new object[_placeholders.Count];
            for (int i = 0; i < _placeholders.Count; i++)
            {
                string text = _placeholders[i];
                if (!values.TryGetValue(text, out var value))
                {
                    if (throwOnMissingValue)
                    {
                        throw new LgyUtilException($"键：{text}，未找到");
                    }

                    value = "{" + text + "}";
                }

                array[i] = value;
            }

            return string.Format(formatProvider, _templateWithIndexes, array);
        }

        public string Format(object values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return Format(MakeDictionary(values), throwOnMissingValue, formatProvider);
        }

        public static string Format(string template, IDictionary<string, object> values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue, formatProvider);
        }

        public static string Format(string template, object values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue, formatProvider);
        }

        //
        // 摘要:
        //     Clears the cached templates and property getters.
        public static void ClearCache()
        {
            TemplateCache.Clear();
            ValueConverterCache.Clear();
            GettersCache.Clear();
        }

        private void ParseTemplate(out string templateWithIndexes, out IList<string> placeholders)
        {
            List<string> tmp = new List<string>();
            templateWithIndexes = TemplateRegex.Replace(_template, Evaluator);
            placeholders = tmp.AsReadOnly();
            string Evaluator(Match m)
            {
                string value = m.Groups["open"].Value;
                string value2 = m.Groups["close"].Value;
                string value3 = m.Groups["key"].Value;
                string value4 = m.Groups["alignment"].Value;
                string value5 = m.Groups["format"].Value;
                if (value.Length % 2 == 0)
                {
                    return m.Value;
                }

                value = RemoveLastChar(value);
                value2 = RemoveLastChar(value2);
                if (!tmp.Contains(value3))
                {
                    tmp.Add(value3);
                }

                int num = tmp.IndexOf(value3);
                return $"{value}{{{num}{value4}{value5}}}{value2}";
            }
        }

        private static string RemoveLastChar(string str)
        {
            if (str.Length > 1)
            {
                return str.Substring(0, str.Length - 1);
            }

            return string.Empty;
        }

        private IDictionary<string, object> MakeDictionary(object obj)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (string placeholder in _placeholders)
            {
                if (TryGetMemberValue(obj, placeholder, out var value))
                {
                    dictionary.Add(placeholder, value);
                }
            }

            return dictionary;
        }

        private static bool TryGetMemberValue(object obj, string memberName, out object value)
        {
            Func<object, object> getterFromCache = GetGetterFromCache(obj.GetType(), memberName);
            if (getterFromCache != null)
            {
                value = getterFromCache(obj);
                return true;
            }

            value = null;
            return false;
        }

        private static StringTemplate GetTemplate(string template)
        {
            return TemplateCache.GetOrAdd(template, new StringTemplate(template));
        }

        private static IStringTemplateValueConverter GetValueConverterFromCache(Type valueConverterType)
        {
            return ValueConverterCache.GetOrAdd(valueConverterType, (IStringTemplateValueConverter)Activator.CreateInstance(valueConverterType));
        }

        private static Func<object, object> GetGetterFromCache(Type type, string memberName)
        {
            return GettersCache.GetOrAdd(type, new ConcurrentDictionary<string, Func<object, object>>()).GetOrAdd(memberName, CreateGetter(type, memberName));
        }

        private static Func<object, object> CreateGetter(Type type, string memberName)
        {
            MemberInfo memberInfo = null;
            while (type != null)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                PropertyInfo declaredProperty = typeInfo.GetDeclaredProperty(memberName);
                if (declaredProperty != null && declaredProperty.CanRead && declaredProperty.GetMethod.IsPublic)
                {
                    memberInfo = declaredProperty;
                    break;
                }

                FieldInfo declaredField = typeInfo.GetDeclaredField(memberName);
                if (declaredField != null && declaredField.IsPublic)
                {
                    memberInfo = declaredField;
                    break;
                }

                type = typeInfo.BaseType;
            }

            if (memberInfo == null)
            {
                return null;
            }

            ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "x");
            MemberExpression memberExpression = Expression.MakeMemberAccess(Expression.Convert(parameterExpression, type), memberInfo);
            StringTemplateValueConverterAttribute customAttribute = memberInfo.GetCustomAttribute<StringTemplateValueConverterAttribute>();
            Expression expression;
            if (customAttribute == null)
            {
                expression = memberExpression;
            }
            else
            {
                if (!(customAttribute.ConverterType != null))
                {
                    throw new InvalidOperationException("An instance of StringTemplateValueConverterAttribute must have its ConverterType property set.");
                }

                expression = MakeStringTemplateValueConversionExpression(memberExpression, customAttribute.ConverterType);
            }

            if (expression.Type.GetTypeInfo().IsValueType)
            {
                expression = Expression.Convert(expression, typeof(object));
            }

            return Expression.Lambda<Func<object, object>>(expression, new ParameterExpression[1] { parameterExpression }).Compile();
        }

        private static MethodCallExpression MakeStringTemplateValueConversionExpression(MemberExpression memberAccess, Type valueConverterType)
        {
            IStringTemplateValueConverter valueConverterFromCache = GetValueConverterFromCache(valueConverterType);
            if (!valueConverterFromCache.CanConvert(memberAccess.Type))
            {
                throw new NotSupportedException($"Member \"{memberAccess.Member}\" cannot be converted by an instance of \"{valueConverterType}\".");
            }

            Expression expression = memberAccess;
            if (memberAccess.Type.GetTypeInfo().IsValueType)
            {
                expression = Expression.Convert(memberAccess, typeof(object));
            }

            return Expression.Call(Expression.Constant(valueConverterFromCache), ConvertStringTemplateValueMethodInfo.Value, expression);
        }
    }
}
