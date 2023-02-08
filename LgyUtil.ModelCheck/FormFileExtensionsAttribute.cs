using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LgyUtil
{
    /// <summary>
    /// 验证FormFile类的扩展名，可以是数组，也可以是单个对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormFileExtensionsAttribute : ValidationAttribute
    {
        /// <summary>
        /// 允许使用的扩展名，多个用英文逗号分隔
        /// </summary>
        public string Extensions { get; set; }
        /// <summary>
        /// 验证FormFile类的扩展名
        /// </summary>
        public FormFileExtensionsAttribute()
        {

        }
        /// <summary>
        /// 验证FormFile类的扩展名
        /// </summary>
        /// <param name="extensions">允许的扩展名，多个用英文逗号分隔</param>
        public FormFileExtensionsAttribute(string extensions)
        {
            Extensions = extensions;
        }
        public override bool IsValid(object value)
        {
            var AllowedExtensions = Extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (value is IEnumerable<IFormFile> files)
            {
                foreach (var f in files)
                {
                    if(!AllowedExtensions.Any(y=> f.FileName.EndsWith(y)))
                        return false;
                }
            }
            else if(value is IFormFile file)
            {
                return AllowedExtensions.Any(y => file.FileName.EndsWith(y));
            }
            return true;
        }
    }
}
