using System;

namespace LgyUtil
{
    /// <summary>
    /// 将字符串进行Trim，放在class上，将class的所有字符串进行trim
    /// 若属性有此特性，则只trim此类中打上次特性的字符串
    /// 可以new的模型才可以使用
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Class,
        AllowMultiple = false, Inherited = true)]
    public class StringTrimAttribute : Attribute
    {
        /// <summary>
        /// 不用进行trim
        /// </summary>
        public bool NotTrim { get; set; } = false;
    }
}
