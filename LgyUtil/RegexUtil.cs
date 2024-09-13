using System;
using System.Text.RegularExpressions;

namespace LgyUtil
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public sealed class RegexUtil
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
        public const string DomainName = @"^((http://)|(https://))?([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}(/)";
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

        #region 密码正则表达式
        /// <summary>
        /// 密码验证：必须包含数字、字母(不区分大小写)
        /// </summary>
        public const string Pwd_NumberLetter = @"(?=.*\d)(?=.*[a-zA-z]).*";
        /// <summary>
        /// 密码验证：必须包含数字、字母(不区分大小写)、特殊符号(! @ # $ % ^ &amp; * ( ) _ + - = . , { } [ ] ?)
        /// </summary>
        public const string Pwd_NumberLetterSymbols = @"(?=.*\d)(?=.*[a-zA-z])(?=.*[!@#\$%\^&\*\(\)_\+\-=\.,\{\}\[\]\?]).*";
        /// <summary>
        /// 密码验证：必须包含数字、大写字母、小写字母
        /// </summary>
        public const string Pwd_NumberLetterBigSmall = @"(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*";
        /// <summary>
        /// 密码验证：必须包含数字、大写字母、小写字母、特殊符号(! @ # $ % ^ &amp; * ( ) _ + - = . , { } [ ] ?)
        /// </summary>
        public const string Pwd_NumberLetterBigSmallSymbols = @"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#\$%\^&\*\(\)_\+\-=\.,\{\}\[\]\?]).*";

        /// <summary>
        /// 密码验证：必须包含数字、字母(不区分大小写)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPwd_NumberLetter(string password) => password.RegexIsMatch(Pwd_NumberLetter);
        /// <summary>
        /// 密码验证：必须包含数字、字母(不区分大小写)、特殊符号(! @ # $ % ^ &amp; * ( ) _ + - = . , { } [ ] ?)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPwd_NumberLetterSymbols(string password) => password.RegexIsMatch(Pwd_NumberLetterSymbols);
        /// <summary>
        /// 密码验证：必须包含数字、大写字母、小写字母
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPwd_NumberLetterBigSmall(string password) => password.RegexIsMatch(Pwd_NumberLetterBigSmall);
        /// <summary>
        /// 密码验证：必须包含数字、大写字母、小写字母、特殊符号(! @ # $ % ^ &amp; * ( ) _ + - = . , { } [ ] ?)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsPwd_NumberLetterBigSmallSymbols(string password) => password.RegexIsMatch(Pwd_NumberLetterBigSmallSymbols);
        #endregion

        /// <summary>
        /// 身份证号，闰年的正则
        /// </summary>
        public const string IdCardRegexLeapYear="^[1-9]\\d{5}(19|20)\\d{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2]\\d|3[0-1])|(04|06|09|11)(0[1-9]|[1-2]\\d|30)|02(0[1-9]|[1-2]\\d))\\d{3}[\\dXx]$";

        /// <summary>
        /// 身份证号，非闰年的正则
        /// </summary>
        public const string IdCardRegexNotLeapYear = "^[1-9]\\d{5}(19|20)\\d{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2]\\d|3[0-1])|(04|06|09|11)(0[1-9]|[1-2]\\d|30)|02(0[1-9]|1\\d|2[0-8]))\\d{3}[\\dXx]$";
        
        /// <summary>
        /// 是否是正确的18位身份证号
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        public static bool IsIdCardNumber(string cardNum)
        {
            if (cardNum.IsNullOrEmpty())
                return false;
            var year = cardNum.Substring(6, 4).ToInt();
            string regex="";
            if ((year % 400 == 0) || (year % 100 != 0 && (year % 4 == 0)))//闰年
                regex = IdCardRegexLeapYear;
            else//平年
                regex = IdCardRegexNotLeapYear;
            return Regex.IsMatch(cardNum, regex);
        }
    }
}
