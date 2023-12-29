using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LgyUtil
{
    /// <summary>
    /// 验证FormFile的文件大小，可以是数组，也可以是单个对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormFileSizeAttribute : ValidationAttribute
    {
        /// <summary>
        /// 限制文件最大MB(优先级B>KB>MB)
        /// </summary>
        public int MB { get; set; }
        /// <summary>
        /// 限制文件最大KB(优先级B>KB>MB)
        /// </summary>
        public int KB { get; set; }
        /// <summary>
        /// 限制文件最大字节(优先级B>KB>MB)
        /// </summary>
        public long B { get; set; }

        /// <summary>
        /// 设置最终验证的文件大小B
        /// </summary>
        private void SetFinalBSize()
        {
            //最终按照字节计算
            if (B > 0)
                return;
            if (KB > 0)
            {
                B = KB * 1024;
                return;
            }
            if (MB > 0)
                B = MB * 1024 * 1024;
        }
        public override bool IsValid(object value)
        {
            SetFinalBSize();
            if (B == 0)//没有字节，跳过验证
                return true;
            if (value is IEnumerable<IFormFile> files)
            {
                foreach (var f in files)
                {
                    if(f.Length>B)
                        return false;
                }
            }
            else if(value is IFormFile file)
            {
                return file.Length <= B;
            }
            return true;
        }
    }
}
