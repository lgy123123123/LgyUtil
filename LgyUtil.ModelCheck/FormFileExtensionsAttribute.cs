using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LgyUtil.ModelCheck
{
    /// <summary>
    /// 验证FormFile类的扩展名
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
            IFormFile file = value as IFormFile; 
            if (file != null) 
            { 
                var fileName = file.FileName;
                var AllowedExtensions = Extensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return AllowedExtensions.Any(y => fileName.EndsWith(y)); 
            } 
            return true; 
        } 
    }
}
