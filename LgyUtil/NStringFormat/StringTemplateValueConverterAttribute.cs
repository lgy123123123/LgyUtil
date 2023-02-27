using System;

namespace LgyUtil.NStringFormat
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    internal sealed class StringTemplateValueConverterAttribute : Attribute
    {
        //
        // 摘要:
        //     The type of the NString.IStringTemplateValueConverter to use.
        public Type ConverterType { get; }

        //
        // 摘要:
        //     Constructs an instance of NString.StringTemplateValueConverterAttribute with
        //     a converter type.
        //
        // 参数:
        //   converterType:
        //     The type of the NString.IStringTemplateValueConverter to use.
        public StringTemplateValueConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
