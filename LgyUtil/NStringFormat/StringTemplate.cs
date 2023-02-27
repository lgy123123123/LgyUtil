using System;
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

        private static readonly NSCache<string, StringTemplate> TemplateCache = new NSCache<string, StringTemplate>();

        private static readonly NSCache<Type, IStringTemplateValueConverter> ValueConverterCache = new NSCache<Type, IStringTemplateValueConverter>();

        private static readonly NSCache<Type, NSCache<string, Func<object, object>>> GettersCache = new NSCache<Type, NSCache<string, Func<object, object>>>();

        private static readonly Lazy<MethodInfo> ConvertStringTemplateValueMethodInfo = new Lazy<MethodInfo>(() => typeof(IStringTemplateValueConverter).GetRuntimeMethod("Convert", new Type[1] { typeof(object) }));

        private StringTemplate(string template)
        {
            _template = template;
            ParseTemplate(out _templateWithIndexes, out _placeholders);
        }

        //
        // 摘要:
        //     Parses the provided string into a StringTemplate instance. Parsed templates are
        //     cached, so calling this method twice with the same argument will return the same
        //     StringTemplate instance.
        //
        // 参数:
        //   template:
        //     string representation of the template
        //
        // 返回结果:
        //     A StringTemplate instance that can be used to format objects.
        //
        // 言论：
        //     The template syntax is similar to the one used in String.Format, except that
        //     indexes are replaced by names.
        public static StringTemplate Parse(string template)
        {
            return GetTemplate(template);
        }

        //
        // 摘要:
        //     Converts a string to a StringTemplate.
        //
        // 参数:
        //   s:
        //     The string to convert
        //
        // 返回结果:
        //     A StringTemplate using the converted string
        public static implicit operator StringTemplate(string s)
        {
            return GetTemplate(s);
        }

        //
        // 摘要:
        //     Returns a string representation of this StringTemplate.
        //
        // 返回结果:
        //     The string representation of this StringTemplate
        public override string ToString()
        {
            return _template;
        }

        //
        // 摘要:
        //     Replaces the template's placeholders with the values from the specified dictionary.
        //
        // 参数:
        //   values:
        //     A dictionary containing values for each placeholder in the template
        //
        //   throwOnMissingValue:
        //     Indicates whether or not to throw an exception if a value is missing for a placeholder.
        //     If this parameter is false and no value is found, the placeholder is left as
        //     is in the formatted string.
        //
        //   formatProvider:
        //     An object that supplies culture-specific formatting information.
        //
        // 返回结果:
        //     The formatted string
        //
        // 异常:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     throwOnMissingValue is true and no value was found in the dictionary for a placeholder
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

        //
        // 摘要:
        //     Replaces the template's placeholders with the values from the specified object.
        //
        // 参数:
        //   values:
        //     An object containing values for the placeholders. For each placeholder, this
        //     method looks for a corresponding property of field in this object.
        //
        //   throwOnMissingValue:
        //     Indicates whether or not to throw an exception if a value is missing for a placeholder.
        //     If this parameter is false and no value is found, the placeholder is left as
        //     is in the formatted string.
        //
        //   formatProvider:
        //     An object that supplies culture-specific formatting information.
        //
        // 返回结果:
        //     The formatted string
        //
        // 异常:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     throwOnMissingValue is true and no value was found in the dictionary for a placeholder
        public string Format(object values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return Format(MakeDictionary(values), throwOnMissingValue, formatProvider);
        }

        //
        // 摘要:
        //     Replaces the specified template's placeholders with the values from the specified
        //     dictionary.
        //
        // 参数:
        //   template:
        //     The template to use to format the values.
        //
        //   values:
        //     A dictionary containing values for each placeholder in the template
        //
        //   throwOnMissingValue:
        //     Indicates whether or not to throw an exception if a value is missing for a placeholder.
        //     If this parameter is false and no value is found, the placeholder is left as
        //     is in the formatted string.
        //
        //   formatProvider:
        //     An object that supplies culture-specific formatting information.
        //
        // 返回结果:
        //     The formatted string
        //
        // 异常:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     throwOnMissingValue is true and no value was found in the dictionary for a placeholder
        public static string Format(string template, IDictionary<string, object> values, bool throwOnMissingValue = true, IFormatProvider formatProvider = null)
        {
            return GetTemplate(template).Format(values, throwOnMissingValue, formatProvider);
        }

        //
        // 摘要:
        //     Replaces the specified template's placeholders with the values from the specified
        //     object.
        //
        // 参数:
        //   template:
        //     The template to use to format the values.
        //
        //   values:
        //     An object containing values for the placeholders. For each placeholder, this
        //     method looks for a corresponding property of field in this object.
        //
        //   throwOnMissingValue:
        //     Indicates whether or not to throw an exception if a value is missing for a placeholder.
        //     If this parameter is false and no value is found, the placeholder is left as
        //     is in the formatted string.
        //
        //   formatProvider:
        //     An object that supplies culture-specific formatting information.
        //
        // 返回结果:
        //     The formatted string
        //
        // 异常:
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     throwOnMissingValue is true and no value was found in the dictionary for a placeholder
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
            return TemplateCache.GetOrAdd(template, () => new StringTemplate(template));
        }

        private static IStringTemplateValueConverter GetValueConverterFromCache(Type valueConverterType)
        {
            return ValueConverterCache.GetOrAdd(valueConverterType, () => (IStringTemplateValueConverter)Activator.CreateInstance(valueConverterType));
        }

        private static Func<object, object> GetGetterFromCache(Type type, string memberName)
        {
            return GettersCache.GetOrAdd(type, () => new NSCache<string, Func<object, object>>()).GetOrAdd(memberName, () => CreateGetter(type, memberName));
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
