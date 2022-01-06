using System;

namespace LgyUtil
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public class RegexUtil
    {
        /// <summary>
        /// 中国大陆手机号
        /// </summary>
        public const string PhoneNumberCN = "^1[3|4|5|6|7|8|9][0-9]{9}$";
        /// <summary>
        /// 是否是中国大陆手机号
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        public static bool IsPhoneNumerCN(string phone)
        {
            return phone.RegexIsMatch(PhoneNumberCN);
        }
        /// <summary>
        /// 全是数字
        /// </summary>
        public const string Number = "^[0-9]*$";
        /// <summary>
        /// 是否全是数字
        /// </summary>
        /// <param name="number">数字</param>
        /// <returns></returns>
        public static bool IsNumber(string number)
        {
            return number.RegexIsMatch(Number);
        }
        /// <summary>
        /// 全是汉字
        /// </summary>
        public const string Chinese = "^[\u4e00-\u9fa5]{0,}$";
        /// <summary>
        /// 是否全是汉字
        /// </summary>
        /// <param name="chinese"></param>
        /// <returns></returns>
        public static bool IsChinese(string chinese)
        {
            return chinese.RegexIsMatch(Chinese);
        }
        /// <summary>
        /// 域名
        /// </summary>
        public const string DomainName = "[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?";
        /// <summary>
        /// 是否是域名
        /// </summary>
        /// <param name="domainname"></param>
        /// <returns></returns>
        public static bool IsDomainName(string domainname)
        {
            return domainname.RegexIsMatch(DomainName);
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        /// <summary>
        /// 是否是邮件
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return email.RegexIsMatch(Email);
        }
        /// <summary>
        /// IP地址
        /// </summary>
        public const string IP = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
        /// <summary>
        /// 是否是IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return ip.RegexIsMatch(IP);
        }
    }
}
