using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace LgyUtil
{
    /// <summary>
    /// 枚举扩展类，获取枚举名称或枚举数字的方法，Enum类里有
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// 获取枚举项的Description属性描述，若没有配置，返回空字符串
        /// </summary>
        /// <param name="e"></param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns>Description属性描述</returns>
        public static string GetEnumDescription(this Enum e, bool isDesc2 = false)
        {
            FieldInfo field = e.GetType().GetField(e.ToString());
            return GetEnumDescription(field, isDesc2);
        }

        private static string GetEnumDescription(FieldInfo field, bool isDesc2 = false)
        {
            if (isDesc2)
            {
                var attribute = field.GetCustomAttribute<Description2Attribute>();
                return attribute is null ? "" : attribute.Description;
            }
            else
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                return attribute is null ? "" : attribute.Description;
            }
        }

        /// <summary>
        /// 获取枚举的所有Description属性描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns>Description属性描述List</returns>
        public static List<string> GetEnumDescriptionList(Type enumType, bool isDesc2 = false)
        {
            List<string> listDesc = new List<string>();
            foreach (string name in enumType.GetEnumNames())
            {
                listDesc.Add(GetEnumDescription(enumType.GetField(name), isDesc2));
            }
            return listDesc;
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="isDesc2">是否是Description2Attribute</param>
        /// <returns>key:枚举名称  value:Description</returns>
        public static Dictionary<string, string> GetEnumDescriptionDic(Type enumType, bool isDesc2 = false)
        {
            Dictionary<string, string> dicDesc = new Dictionary<string, string>();
            foreach (string name in enumType.GetEnumNames())
            {
                dicDesc.Add(name, GetEnumDescription(enumType.GetField(name), isDesc2));
            }
            return dicDesc;
        }
        /// <summary>
        /// 获取枚举的所有Description属性描述和枚举名称
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>key:枚举名称  value:枚举数字</returns>
        public static Dictionary<string, int> GetEnumNameValueDic(Type enumType)
        {
            Dictionary<string, int> dicDesc = new Dictionary<string, int>();
            foreach (int eCode in Enum.GetValues(enumType))
            {
                dicDesc.Add(Enum.GetName(enumType, eCode), eCode);
            }
            return dicDesc;
        }
        
        /// <summary>
        /// 获取枚举的所有详细信息
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static List<EnumUtilETypeDetail> GetEnumDetailList(Type enumType)
        {
            List<EnumUtilETypeDetail> listDetail = new List<EnumUtilETypeDetail>();
            foreach (int eCode in Enum.GetValues(enumType))
            {
                var detail = new EnumUtilETypeDetail
                {
                    EnumName = Enum.GetName(enumType, eCode),
                    EnumNumber = eCode,
                    EnumDescription = GetEnumDescription(enumType.GetField(Enum.GetName(enumType, eCode)))
                };
                listDetail.Add(detail);
            }
            return listDetail;
        }
    }

    /// <summary>
    /// 枚举详细信息
    /// </summary>
    public class EnumUtilETypeDetail
    {
        /// <summary>
        /// 枚举名称
        /// </summary>
        public string EnumName { get; set; }
        /// <summary>
        /// 枚举数值
        /// </summary>
        public int EnumNumber { get; set; }
        /// <summary>
        /// 枚举Description中的描述
        /// </summary>
        public string EnumDescription { get; set; }
    }
    /// <summary>
    /// 枚举的第二个属性，方便使用，有需要的话，再增加第三个
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class Description2Attribute : Attribute
    {
        public Description2Attribute() : this(string.Empty)
        { }
        public Description2Attribute(string description)
        {
            this.DescriptionValue = description;
        }
        public virtual string Description => this.DescriptionValue;
        protected string DescriptionValue { get; set; }
    }
}
