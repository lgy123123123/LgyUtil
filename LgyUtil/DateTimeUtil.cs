using System;
using System.Globalization;

namespace LgyUtil
{
    /// <summary>
    /// 日期扩展方法
    /// </summary>
    public static class DateTimeUtil
    {
        /// <summary>
        /// 格式化日期为yyyy-MM-dd
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMdd(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 格式化日期为yyyy年MM月dd日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddCH(this DateTime dt)
        {
            return dt.ToString("yyyy年MM月dd日");
        }
        /// <summary>
        /// 格式化日期为yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddHHmmss(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// 格式化日期为yyyy年MM月dd日 HH:mm:ss
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToyyyyMMddHHmmssCH(this DateTime dt)
        {
            return dt.ToString("yyyy年MM月dd日 HH:mm:ss");
        }
        /// <summary>
        /// 获取当前unix时间戳(13位)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds.ToLong();
        }
        /// <summary>
        /// utc时间戳，转本地时间
        /// </summary>
        /// <param name="timestamp">13位时间戳(js生成的时间戳)</param>
        /// <returns></returns>
        public static DateTime GetDateTimeByTimestamp(string timestamp)
        {
            DateTime dtTimestamp = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1), TimeZoneInfo.Local);
            return dtTimestamp.AddMilliseconds(timestamp.ToLong());
        }
        /// <summary>
        /// 添加季度
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="numQuarter">增加的季度数</param>
        /// <returns></returns>
        public static DateTime AddQuarter(this DateTime dt, int numQuarter)
        {
            return dt.AddMonths(numQuarter * 3);
        }
        /// <summary>
        /// 获取季度
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetQuarter(this DateTime dt)
        {
            return Math.Ceiling((double)dt.Month / 3).ToInt();
        }
        /// <summary>
        /// <para>获取月份差，返回正整数</para>
        /// <para>例如：5月和3月的月份差为2</para>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static int GetMonthBetween(this DateTime dt, DateTime dt2)
        {
            var dtBig = dt > dt2 ? dt : dt2;
            var dtSmall = dt < dt2 ? dt : dt2;
            return (dtBig.Year - dtSmall.Year) * 12 + dtBig.Month - dtSmall.Month;
        }
        /// <summary>
        /// 获取当前年第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetYearsStart(this DateTime dt, bool formatTime = true)
        {
            if (formatTime)
                return new DateTime(dt.Year, 1, 1, 0, 0, 0);
            else
                return new DateTime(dt.Year, 1, 1, dt.Hour, dt.Minute, dt.Second);
        }
        /// <summary>
        /// 获取当前年最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetYearsEnd(this DateTime dt, bool formatTime = true)
        {
            if (formatTime)
                return new DateTime(dt.Year, 12, 31, 0, 0, 0);
            else
                return new DateTime(dt.Year, 12, 31, dt.Hour, dt.Minute, dt.Second); ;
        }
        /// <summary>
        /// 获取当前季度第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetQuarterStart(this DateTime dt, bool formatTime = true)
        {
            if (formatTime)
                return new DateTime(dt.Year, ((dt.GetQuarter() - 1) * 3 + 1).ToInt(), 1, 0, 0, 0);
            else
                return new DateTime(dt.Year, ((dt.GetQuarter() - 1) * 3 + 1).ToInt(), 1, dt.Hour, dt.Minute, dt.Second);
        }
        /// <summary>
        /// 获取当前季度最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetQuarterEnd(this DateTime dt, bool formatTime = true)
        {
            return dt.GetQuarterStart(formatTime).AddQuarter(1).AddDays(-1);
        }
        /// <summary>
        /// 获取当前月第一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetMonthStart(this DateTime dt, bool formatTime = true)
        {
            if (formatTime)
                return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0);
            else
                return new DateTime(dt.Year, dt.Month, 1, dt.Hour, dt.Minute, dt.Second);
        }
        /// <summary>
        /// 获取当前月最后一天
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="formatTime">是否格式化时间(false:留原有的时分秒)</param>
        /// <returns></returns>
        public static DateTime GetMonthEnd(this DateTime dt, bool formatTime = true)
        {
            return dt.GetMonthStart(formatTime).AddMonths(1).AddDays(-1);
        }
        /// <summary>
        /// 获取当前日开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetDaysStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }
        /// <summary>
        /// 获取当前日结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetDaysEnd(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
        }
        /// <summary>
        /// 获取当前时开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetHourStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }
        /// <summary>
        /// 获取当前时结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetHourEnd(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 59, 59);
        }
        /// <summary>
        /// 获取当前分开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMinuteStart(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }
        /// <summary>
        /// 获取当前分结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime GetMinuteEnd(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 59);
        }

        /// <summary>
        /// 格式化日期，Q代表季度，只解析第一个Q(解决linux格式化/转成-问题)
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">格式化字符串，Q代表季度，只解析第一个Q</param>
        /// <returns></returns>
        public static string ToStringExt(this DateTime dt, string format)
        {
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = format;
            dtFormat.FullDateTimePattern = format;
            if (format.Contains("Q"))
            {
                int quarterIndex = format.IndexOf("Q");
                int quarterNum = dt.GetQuarter();
                dtFormat.ShortDatePattern = format.Replace("Q", "");
                dtFormat.FullDateTimePattern = dtFormat.ShortDatePattern;
                string ret = dt.ToString(dtFormat.ShortDatePattern, dtFormat);
                return ret.Insert(quarterIndex, quarterNum.ToString());
            }
            else
                return dt.ToString(format, dtFormat);
        }
    }
}
