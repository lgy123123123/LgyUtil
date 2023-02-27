using System;

namespace LgyUtil.NStringFormat
{
    internal interface IStringTemplateValueConverter
    {
        //
        // 摘要:
        //     Determines whether this instance can convert the specified object type.
        //
        // 参数:
        //   objectType:
        //     Type of the value to convert.
        //
        // 返回结果:
        //     true if this instance can convert the specified object type; otherwise, false.
        bool CanConvert(Type objectType);

        //
        // 摘要:
        //     Performs the value conversion.
        //
        // 参数:
        //   value:
        //     The input value.
        //
        // 返回结果:
        //     The converted output value.
        object Convert(object value);
    }
}
