using System;

namespace LgyUtil.NStringFormat
{
    internal interface IStringTemplateValueConverter
    {
        bool CanConvert(Type objectType);

        object Convert(object value);
    }
}
