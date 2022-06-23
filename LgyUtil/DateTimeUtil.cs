using System;

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
        /// <param name="timestamp"></param>
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
        public static DateTime AddQuarter(this DateTime dt,int numQuarter)
        {
            return dt.AddMonths(numQuarter * 3);
        }
        /// <summary>
        /// 获取季度
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetQuarter(this DateTime dt)
        {
            return Math.Ceiling((double)dt.Month / 3);
        }
        /// <summary>
        /// <para>获取月份差，返回正整数</para>
        /// <para>例如：5月和3月的月份差为2</para>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static int GetMonthBetween(this DateTime dt,DateTime dt2)
        {
            var dtBig = dt > dt2 ? dt : dt2;
            var dtSmall = dt < dt2 ? dt : dt2;
            return (dtBig.Year - dtSmall.Year) * 12 + dtBig.Month - dtSmall.Month;
        }
    }
}
