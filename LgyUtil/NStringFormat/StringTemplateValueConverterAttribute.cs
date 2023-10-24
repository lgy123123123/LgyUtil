using System;

namespace LgyUtil.NStringFormat
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal sealed class StringTemplateValueConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public StringTemplateValueConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
