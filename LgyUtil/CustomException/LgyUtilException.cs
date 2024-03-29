﻿using System;

namespace LgyUtil
{
    /// <summary>
    /// 自定义异常，用于过滤异常
    /// </summary>
    public class LgyUtilException:Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public LgyUtilException()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public LgyUtilException(string message):base(message)
        {
            
        }
        /// <summary>
        /// 获取最底层异常类信息（递归获取，多个异常，只会获取到第一个）
        /// </summary>
        /// <param name="e">异常</param>
        /// <returns>最底层异常类</returns>
        public static Exception GetInnerException(Exception e)
        {
            if (e.InnerException == null)
                return e;
            else
                return GetInnerException(e.InnerException);
        }
    }
}
