namespace LgyUtil
{
    /// <summary>
    /// 数字帮助工具
    /// </summary>
    public static class NumberUtil
    {
        #region Between int

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this int num, int min, int max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this int num, int min, int max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this int num, int min, int max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this int num, int min, int max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between long

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this long num, long min, long max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this long num, long min, long max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this long num, long min, long max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this long num, long min, long max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between float

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this float num, float min, float max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this float num, float min, float max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this float num, float min, float max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this float num, float min, float max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between double

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this double num, double min, double max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this double num, double min, double max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this double num, double min, double max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this double num, double min, double max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between decimal

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this decimal num, decimal min, decimal max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this decimal num, decimal min, decimal max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this decimal num, decimal min, decimal max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this decimal num, decimal min, decimal max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between byte

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this byte num, byte min, byte max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this byte num, byte min, byte max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this byte num, byte min, byte max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this byte num, byte min, byte max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Between short

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool Between(this short num, short min, short max)
        {
            return num >= min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt;= max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(包含)</param>
        /// <returns></returns>
        public static bool BetweenNotLeft(this short num, short min, short max)
        {
            return num > min && num <= max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt;= min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotRight(this short num, short min, short max)
        {
            return num >= min && num < max;
        }

        /// <summary>
        /// 判断数字是否在指定范围内(num &gt; min &amp;&amp; num &lt; max)
        /// </summary>
        /// <param name="num"></param>
        /// <param name="min">最小值(不包含)</param>
        /// <param name="max">最大值(不包含)</param>
        /// <returns></returns>
        public static bool BetweenNotBoth(this short num, short min, short max)
        {
            return num > min && num < max;
        }

        #endregion

        #region Percent百分比

        /// <summary>
        /// 求一个数值，占另一个数值的百分比
        /// </summary>
        /// <param name="num">数值</param>
        /// <param name="total">总数</param>
        /// <param name="digit">百分比小数精度</param>
        /// <returns></returns>
        public static string Percent(this double num, double total, int digit = 2)
        {
            if (total == 0) return "100%";
            if (num == 0) return "0%";
            return (num / total * 100).ToString("0.".PadRight(digit + 2, '0')) + "%";
        }
        
        /// <summary>
        /// 求一个数值，占另一个数值的百分比
        /// </summary>
        /// <param name="num">数值</param>
        /// <param name="total">总数</param>
        /// <param name="digit">百分比小数精度</param>
        /// <returns></returns>
        public static string Percent(this int num, double total, int digit = 2)
        {
            if (total == 0) return "100%";
            if (num == 0) return "0%";
            return (num / total * 100).ToString("0.".PadRight(digit + 2, '0')) + "%";
        }

        #endregion
    }
}