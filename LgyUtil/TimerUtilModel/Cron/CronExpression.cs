﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace LgyUtil.TimerUtilModel.Cron
{
    /// <summary>
    /// cron表达式解析类
    /// </summary>
    [Serializable]
    internal class CronExpression
    {
        /// <summary>
        /// Field specification for second.
        /// </summary>
        protected const int Second = 0;

        /// <summary>
        /// Field specification for minute.
        /// </summary>
        protected const int Minute = 1;

        /// <summary>
        /// Field specification for hour.
        /// </summary>
        protected const int Hour = 2;

        /// <summary>
        /// Field specification for day of month.
        /// </summary>
        protected const int DayOfMonth = 3;

        /// <summary>
        /// Field specification for month.
        /// </summary>
        protected const int Month = 4;

        /// <summary>
        /// Field specification for day of week.
        /// </summary>
        protected const int DayOfWeek = 5;

        /// <summary>
        /// Field specification for year.
        /// </summary>
        protected const int Year = 6;

        /// <summary>
        /// Field specification for all wildcard value '*'.
        /// </summary>
        protected const int AllSpecInt = 99; // '*'

        /// <summary>
        /// Field specification for not specified value '?'.
        /// </summary>
        protected const int NoSpecInt = 98; // '?'

        /// <summary>
        /// Field specification for wildcard '*'.
        /// </summary>
        protected const int AllSpec = AllSpecInt;

        /// <summary>
        /// Field specification for no specification at all '?'.
        /// </summary>
        protected const int NoSpec = NoSpecInt;

        private static readonly Dictionary<string, int> monthMap = new Dictionary<string, int>(20);
        private static readonly Dictionary<string, int> dayMap = new Dictionary<string, int>(60);

        private TimeZoneInfo timeZone;

        /// <summary>
        /// Seconds.
        /// </summary>
        [NonSerialized] protected SortedSet<int> seconds = null;

        /// <summary>
        /// minutes.
        /// </summary>
        [NonSerialized] protected SortedSet<int> minutes = null;

        /// <summary>
        /// Hours.
        /// </summary>
        [NonSerialized] protected SortedSet<int> hours = null;

        /// <summary>
        /// Days of month.
        /// </summary>
        [NonSerialized] protected SortedSet<int> daysOfMonth = null;

        /// <summary>
        /// Months.
        /// </summary>
        [NonSerialized] protected SortedSet<int> months = null;

        /// <summary>
        /// Days of week.
        /// </summary>
        [NonSerialized] protected SortedSet<int> daysOfWeek = null;

        /// <summary>
        /// Years.
        /// </summary>
        [NonSerialized] protected SortedSet<int> years = null;

        /// <summary>
        /// Last day of week.
        /// </summary>
        [NonSerialized] protected bool lastdayOfWeek;

        /// <summary>
        /// N number of weeks.
        /// </summary>
        [NonSerialized] protected int everyNthWeek;

        /// <summary>
        /// Nth day of week.
        /// </summary>
        [NonSerialized] protected int nthdayOfWeek;

        /// <summary>
        /// Last day of month.
        /// </summary>
        [NonSerialized] protected bool lastdayOfMonth;

        /// <summary>
        /// Nearest weekday.
        /// </summary>
        [NonSerialized] protected bool nearestWeekday;

        [NonSerialized] protected int lastdayOffset;

        /// <summary>
        /// Calendar day of week.
        /// </summary>
        [NonSerialized] protected bool calendardayOfWeek;

        /// <summary>
        /// Calendar day of month.
        /// </summary>
        [NonSerialized] protected bool calendardayOfMonth;

        /// <summary>
        /// Expression parsed.
        /// </summary>
        [NonSerialized] protected bool expressionParsed;

        private static readonly int MaxYear = DateTime.Now.Year + 100;

        private static readonly char[] splitSeparators = { ' ', '\t', '\r', '\n' };
        private static readonly char[] commaSeparator = { ',' };
        private static readonly Regex regex = new Regex("^L-[0-9]*[W]?", RegexOptions.Compiled);

        static CronExpression()
        {
            monthMap.Add("JAN", 0);
            monthMap.Add("FEB", 1);
            monthMap.Add("MAR", 2);
            monthMap.Add("APR", 3);
            monthMap.Add("MAY", 4);
            monthMap.Add("JUN", 5);
            monthMap.Add("JUL", 6);
            monthMap.Add("AUG", 7);
            monthMap.Add("SEP", 8);
            monthMap.Add("OCT", 9);
            monthMap.Add("NOV", 10);
            monthMap.Add("DEC", 11);

            dayMap.Add("SUN", 1);
            dayMap.Add("MON", 2);
            dayMap.Add("TUE", 3);
            dayMap.Add("WED", 4);
            dayMap.Add("THU", 5);
            dayMap.Add("FRI", 6);
            dayMap.Add("SAT", 7);
        }

        ///<summary>
        /// 构造解析cron表达式对象
        /// </summary>
        /// <param name="cronExpression">cron表达式</param>
        public CronExpression(string cronExpression)
        {
            if (cronExpression == null)
            {
                throw new ArgumentException("cronExpression cannot be null", nameof(cronExpression));
            }

            CronExpressionString = CultureInfo.InvariantCulture.TextInfo.ToUpper(cronExpression);
            BuildExpression(CronExpressionString);
        }

        /// <summary>
        /// Sets or gets the time zone for which the <see cref="CronExpression" /> of this
        /// </summary>
        private TimeZoneInfo TimeZone
        {
            set => timeZone = value;
            get
            {
                if (timeZone == null)
                {
                    timeZone = TimeZoneInfo.Local;
                }

                return timeZone;
            }
        }

        /// <summary>
        /// Returns the string representation of the <see cref="CronExpression" />
        /// </summary>
        /// <returns>The string representation of the <see cref="CronExpression" /></returns>
        public override string ToString()
        {
            return CronExpressionString;
        }

        /// <summary>
        /// 验证表达式是否有效
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public static bool IsValidExpression(string cronExpression)
        {
            try
            {
                new CronExpression(cronExpression);
            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////
        //
        // Expression Parsing Functions
        //
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Builds the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected void BuildExpression(string expression)
        {
            expressionParsed = true;

            try
            {
                seconds = seconds ?? new SortedSet<int>();
                minutes = minutes ?? new SortedSet<int>();
                hours = hours ?? new SortedSet<int>();
                daysOfMonth = daysOfMonth ?? new SortedSet<int>();
                months = months ?? new SortedSet<int>();
                daysOfWeek = daysOfWeek ?? new SortedSet<int>();
                years = years ?? new SortedSet<int>();

                int exprOn = Second;

                string[] exprsTok = expression.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string exprTok in exprsTok)
                {
                    string expr = exprTok.Trim();

                    if (expr.Length == 0)
                    {
                        continue;
                    }
                    if (exprOn > Year)
                    {
                        break;
                    }

                    // throw an exception if L is used with other days of the month
                    if (exprOn == DayOfMonth && expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(",", StringComparison.Ordinal) >= 0)
                    {
                        throw new FormatException("Support for specifying 'L' and 'LW' with other days of the month is not implemented");
                    }
                    // throw an exception if L is used with other days of the week
                    if (exprOn == DayOfWeek && expr.IndexOf('L') != -1 && expr.Length > 1 && expr.IndexOf(",", StringComparison.Ordinal) >= 0)
                    {
                        throw new FormatException("Support for specifying 'L' with other days of the week is not implemented");
                    }
                    if (exprOn == DayOfWeek && expr.IndexOf('#') != -1 && expr.IndexOf('#', expr.IndexOf('#') + 1) != -1)
                    {
                        throw new FormatException("Support for specifying multiple \"nth\" days is not implemented.");
                    }

                    string[] vTok = expr.Split(commaSeparator);
                    foreach (string v in vTok)
                    {
                        StoreExpressionVals(0, v, exprOn);
                    }

                    exprOn++;
                }

                if (exprOn <= DayOfWeek)
                {
                    throw new FormatException("Unexpected end of expression.");
                }

                if (exprOn <= Year)
                {
                    StoreExpressionVals(0, "*", Year);
                }

                ISet<int> dow = GetSet(DayOfWeek);
                ISet<int> dom = GetSet(DayOfMonth);

                // Copying the logic from the UnsupportedOperationException below
                bool dayOfMSpec = !dom.Contains(NoSpec);
                bool dayOfWSpec = !dow.Contains(NoSpec);

                if (dayOfMSpec && !dayOfWSpec)
                {
                    // skip
                }
                else if (dayOfWSpec && !dayOfMSpec)
                {
                    // skip
                }
                else
                {
                    throw new FormatException("Support for specifying both a day-of-week AND a day-of-month parameter is not implemented.");
                }
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new FormatException($"Illegal cron expression format ({e.Message})", e);
            }
        }

        /// <summary>
        /// Stores the expression values.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="s">The string to traverse.</param>
        /// <param name="type">The type of value.</param>
        /// <returns></returns>
        protected int StoreExpressionVals(int pos, string s, int type)
        {
            int incr = 0;
            int i = SkipWhiteSpace(pos, s);
            if (i >= s.Length)
            {
                return i;
            }
            char c = s[i];
            if (c >= 'A' && c <= 'Z' && !s.Equals("L") && !s.Equals("LW") && !regex.IsMatch(s))
            {
                string sub = s.Substring(i, 3);
                int sval;
                int eval = -1;
                if (type == Month)
                {
                    sval = GetMonthNumber(sub) + 1;
                    if (sval <= 0)
                    {
                        throw new FormatException($"Invalid Month value: '{sub}'");
                    }
                    if (s.Length > i + 3)
                    {
                        c = s[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = s.Substring(i, 3);
                            eval = GetMonthNumber(sub) + 1;
                            if (eval <= 0)
                            {
                                throw new FormatException(
                                    $"Invalid Month value: '{sub}'");
                            }
                        }
                    }
                }
                else if (type == DayOfWeek)
                {
                    sval = GetDayOfWeekNumber(sub);
                    if (sval < 0)
                    {
                        throw new FormatException($"Invalid Day-of-Week value: '{sub}'");
                    }
                    if (s.Length > i + 3)
                    {
                        c = s[i + 3];
                        if (c == '-')
                        {
                            i += 4;
                            sub = s.Substring(i, 3);
                            eval = GetDayOfWeekNumber(sub);
                            if (eval < 0)
                            {
                                throw new FormatException(
                                    $"Invalid Day-of-Week value: '{sub}'");
                            }
                        }
                        else if (c == '#')
                        {
                            try
                            {
                                i += 4;
                                nthdayOfWeek = Convert.ToInt32(s.Substring(i), CultureInfo.InvariantCulture);
                                if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception)
                            {
                                throw new FormatException(
                                    "A numeric value between 1 and 5 must follow the '#' option");
                            }
                        }
                        else if (c == '/')
                        {
                            try
                            {
                                i += 4;
                                everyNthWeek = Convert.ToInt32(s.Substring(i), CultureInfo.InvariantCulture);
                                if (everyNthWeek < 1 || everyNthWeek > 5)
                                {
                                    throw new Exception();
                                }
                            }
                            catch (Exception)
                            {
                                throw new FormatException(
                                    "A numeric value between 1 and 5 must follow the '/' option");
                            }
                        }
                        else if (c == 'L')
                        {
                            lastdayOfWeek = true;
                            i++;
                        }
                        else
                        {
                            throw new FormatException($"Illegal characters for this position: '{sub}'");
                        }
                    }
                }
                else
                {
                    throw new FormatException($"Illegal characters for this position: '{sub}'");
                }
                if (eval != -1)
                {
                    incr = 1;
                }
                AddToSet(sval, eval, incr, type);
                return i + 3;
            }

            if (c == '?')
            {
                i++;
                if (i + 1 <= s.Length && s[i] != ' ' && s[i] != '\t')
                {
                    throw new FormatException("Illegal character after '?': "
                                              + s[i]);
                }
                if (type != DayOfWeek && type != DayOfMonth)
                {
                    throw new FormatException(
                        "'?' can only be specified for Day-of-Month or Day-of-Week.");
                }
                if (type == DayOfWeek && !lastdayOfMonth)
                {
                    int val = daysOfMonth.LastOrDefault();
                    if (val == NoSpecInt)
                    {
                        throw new FormatException(
                            "'?' can only be specified for Day-of-Month -OR- Day-of-Week.");
                    }
                }

                AddToSet(NoSpecInt, -1, 0, type);
                return i;
            }

            var startsWithAsterisk = c == '*';
            if (startsWithAsterisk || c == '/')
            {
                if (startsWithAsterisk && i + 1 >= s.Length)
                {
                    AddToSet(AllSpecInt, -1, incr, type);
                    return i + 1;
                }
                if (c == '/' && (i + 1 >= s.Length || s[i + 1] == ' ' || s[i + 1] == '\t'))
                {
                    throw new FormatException("'/' must be followed by an integer.");
                }
                if (startsWithAsterisk)
                {
                    i++;
                }
                c = s[i];
                if (c == '/')
                {
                    // is an increment specified?
                    i++;
                    if (i >= s.Length)
                    {
                        throw new FormatException("Unexpected end of string.");
                    }

                    incr = GetNumericValue(s, i);

                    i++;
                    if (incr > 10)
                    {
                        i++;
                    }
                    CheckIncrementRange(incr, type);
                }
                else
                {
                    if (startsWithAsterisk)
                    {
                        // invalid value s
                        throw new FormatException("Illegal characters after asterisk: " + s);
                    }
                    incr = 1;
                }

                AddToSet(AllSpecInt, -1, incr, type);
                return i;
            }
            if (c == 'L')
            {
                i++;
                if (type == DayOfMonth)
                {
                    lastdayOfMonth = true;
                }
                if (type == DayOfWeek)
                {
                    AddToSet(7, 7, 0, type);
                }
                if (type == DayOfMonth && s.Length > i)
                {
                    c = s[i];
                    if (c == '-')
                    {
                        ValueSet vs = GetValue(0, s, i + 1);
                        lastdayOffset = vs.theValue;
                        if (lastdayOffset > 30)
                        {
                            throw new FormatException("Offset from last day must be <= 30");
                        }
                        i = vs.pos;
                    }
                    if (s.Length > i)
                    {
                        c = s[i];
                        if (c == 'W')
                        {
                            nearestWeekday = true;
                            i++;
                        }
                    }
                }
                return i;
            }
            if (c >= '0' && c <= '9')
            {
                int val = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= s.Length)
                {
                    AddToSet(val, -1, -1, type);
                }
                else
                {
                    c = s[i];
                    if (c >= '0' && c <= '9')
                    {
                        ValueSet vs = GetValue(val, s, i);
                        val = vs.theValue;
                        i = vs.pos;
                    }
                    i = CheckNext(i, s, val, type);
                    return i;
                }
            }
            else
            {
                throw new FormatException($"Unexpected character: {c}");
            }

            return i;
        }

        private void CheckIncrementRange(int incr, int type)
        {
            if (incr > 59 && (type == Second || type == Minute))
            {
                throw new FormatException($"Increment > 60 : {incr}");
            }
            if (incr > 23 && type == Hour)
            {
                throw new FormatException($"Increment > 24 : {incr}");
            }
            if (incr > 31 && type == DayOfMonth)
            {
                throw new FormatException($"Increment > 31 : {incr}");
            }
            if (incr > 7 && type == DayOfWeek)
            {
                throw new FormatException($"Increment > 7 : {incr}");
            }
            if (incr > 12 && type == Month)
            {
                throw new FormatException($"Increment > 12 : {incr}");
            }
        }

        /// <summary>
        /// Checks the next value.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="s">The string to check.</param>
        /// <param name="val">The value.</param>
        /// <param name="type">The type to search.</param>
        /// <returns></returns>
        protected int CheckNext(int pos, string s, int val, int type)
        {
            int end = -1;
            int i = pos;

            if (i >= s.Length)
            {
                AddToSet(val, end, -1, type);
                return i;
            }

            char c = s[pos];

            if (c == 'L')
            {
                if (type == DayOfWeek)
                {
                    if (val < 1 || val > 7)
                    {
                        throw new FormatException("Day-of-Week values must be between 1 and 7");
                    }
                    lastdayOfWeek = true;
                }
                else
                {
                    throw new FormatException($"'L' option is not valid here. (pos={i})");
                }
                ISet<int> data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == 'W')
            {
                if (type == DayOfMonth)
                {
                    nearestWeekday = true;
                }
                else
                {
                    throw new FormatException($"'W' option is not valid here. (pos={i})");
                }
                if (val > 31)
                {
                    throw new FormatException("The 'W' option does not make sense with values larger than 31 (max number of days in a month)");
                }

                ISet<int> data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == '#')
            {
                if (type != DayOfWeek)
                {
                    throw new FormatException(
                        $"'#' option is not valid here. (pos={i})");
                }
                i++;
                try
                {
                    nthdayOfWeek = Convert.ToInt32(s.Substring(i), CultureInfo.InvariantCulture);
                    if (nthdayOfWeek < 1 || nthdayOfWeek > 5)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new FormatException(
                        "A numeric value between 1 and 5 must follow the '#' option");
                }

                ISet<int> data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == 'C')
            {
                if (type == DayOfWeek)
                {
                    calendardayOfWeek = true;
                }
                else if (type == DayOfMonth)
                {
                    calendardayOfMonth = true;
                }
                else
                {
                    throw new FormatException($"'C' option is not valid here. (pos={i})");
                }
                ISet<int> data = GetSet(type);
                data.Add(val);
                i++;
                return i;
            }

            if (c == '-')
            {
                i++;
                c = s[i];
                int v = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                end = v;
                i++;
                if (i >= s.Length)
                {
                    AddToSet(val, end, 1, type);
                    return i;
                }
                c = s[i];
                if (c >= '0' && c <= '9')
                {
                    ValueSet vs = GetValue(v, s, i);
                    int v1 = vs.theValue;
                    end = v1;
                    i = vs.pos;
                }
                if (i < s.Length && s[i] == '/')
                {
                    i++;
                    c = s[i];
                    int v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                    i++;
                    if (i >= s.Length)
                    {
                        AddToSet(val, end, v2, type);
                        return i;
                    }
                    c = s[i];
                    if (c >= '0' && c <= '9')
                    {
                        ValueSet vs = GetValue(v2, s, i);
                        int v3 = vs.theValue;
                        AddToSet(val, end, v3, type);
                        i = vs.pos;
                        return i;
                    }
                    AddToSet(val, end, v2, type);
                    return i;
                }
                AddToSet(val, end, 1, type);
                return i;
            }

            if (c == '/')
            {
                if (i + 1 >= s.Length || s[i + 1] == ' ' || s[i + 1] == '\t')
                {
                    throw new FormatException("\'/\' must be followed by an integer.");
                }

                i++;
                c = s[i];
                int v2 = Convert.ToInt32(c.ToString(), CultureInfo.InvariantCulture);
                i++;
                if (i >= s.Length)
                {
                    CheckIncrementRange(v2, type);
                    AddToSet(val, end, v2, type);
                    return i;
                }
                c = s[i];
                if (c >= '0' && c <= '9')
                {
                    ValueSet vs = GetValue(v2, s, i);
                    int v3 = vs.theValue;
                    CheckIncrementRange(v3, type);
                    AddToSet(val, end, v3, type);
                    i = vs.pos;
                    return i;
                }
                throw new FormatException($"Unexpected character '{c}' after '/'");
            }

            AddToSet(val, end, 0, type);
            i++;
            return i;
        }

        /// <summary>
        /// 表达式字符串
        /// </summary>
        /// <value>The cron expression string.</value>
        public string CronExpressionString { get; }

        /// <summary>
        /// 获取表达式英文描述
        /// </summary>
        /// <returns></returns>
        public string GetExpressionSummary()
        {
            StringBuilder buf = new StringBuilder();

            buf.Append("seconds: ");
            buf.Append(GetExpressionSetSummary(seconds));
            buf.Append("\n");
            buf.Append("minutes: ");
            buf.Append(GetExpressionSetSummary(minutes));
            buf.Append("\n");
            buf.Append("hours: ");
            buf.Append(GetExpressionSetSummary(hours));
            buf.Append("\n");
            buf.Append("daysOfMonth: ");
            buf.Append(GetExpressionSetSummary(daysOfMonth));
            buf.Append("\n");
            buf.Append("months: ");
            buf.Append(GetExpressionSetSummary(months));
            buf.Append("\n");
            buf.Append("daysOfWeek: ");
            buf.Append(GetExpressionSetSummary(daysOfWeek));
            buf.Append("\n");
            buf.Append("lastdayOfWeek: ");
            buf.Append(lastdayOfWeek);
            buf.Append("\n");
            buf.Append("nearestWeekday: ");
            buf.Append(nearestWeekday);
            buf.Append("\n");
            buf.Append("NthDayOfWeek: ");
            buf.Append(nthdayOfWeek);
            buf.Append("\n");
            buf.Append("lastdayOfMonth: ");
            buf.Append(lastdayOfMonth);
            buf.Append("\n");
            buf.Append("calendardayOfWeek: ");
            buf.Append(calendardayOfWeek);
            buf.Append("\n");
            buf.Append("calendardayOfMonth: ");
            buf.Append(calendardayOfMonth);
            buf.Append("\n");
            buf.Append("years: ");
            buf.Append(GetExpressionSetSummary(years));
            buf.Append("\n");

            return buf.ToString();
        }

        /// <summary>
        /// Gets the expression set summary.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        protected string GetExpressionSetSummary(ICollection<int> data)
        {
            if (data.Contains(NoSpec))
            {
                return "?";
            }
            if (data.Contains(AllSpec))
            {
                return "*";
            }

            StringBuilder buf = new StringBuilder();

            bool first = true;
            foreach (int iVal in data)
            {
                string val = iVal.ToString(CultureInfo.InvariantCulture);
                if (!first)
                {
                    buf.Append(",");
                }
                buf.Append(val);
                first = false;
            }

            return buf.ToString();
        }

        /// <summary>
        /// Skips the white space.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        protected int SkipWhiteSpace(int i, string s)
        {
            for (; i < s.Length && (s[i] == ' ' || s[i] == '\t'); i++)
            {
            }

            return i;
        }

        /// <summary>
        /// Finds the next white space.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        protected int FindNextWhiteSpace(int i, string s)
        {
            for (; i < s.Length && (s[i] != ' ' || s[i] != '\t'); i++)
            {
            }

            return i;
        }

        /// <summary>
        /// Adds to set.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="end">The end.</param>
        /// <param name="incr">The incr.</param>
        /// <param name="type">The type.</param>
        protected void AddToSet(int val, int end, int incr, int type)
        {
            ISet<int> data = GetSet(type);

            if (type == Second || type == Minute)
            {
                if ((val < 0 || val > 59 || end > 59) && val != AllSpecInt)
                {
                    throw new FormatException(
                        "Minute and Second values must be between 0 and 59");
                }
            }
            else if (type == Hour)
            {
                if ((val < 0 || val > 23 || end > 23) && val != AllSpecInt)
                {
                    throw new FormatException(
                        "Hour values must be between 0 and 23");
                }
            }
            else if (type == DayOfMonth)
            {
                if ((val < 1 || val > 31 || end > 31) && val != AllSpecInt
                    && val != NoSpecInt)
                {
                    throw new FormatException(
                        "Day of month values must be between 1 and 31");
                }
            }
            else if (type == Month)
            {
                if ((val < 1 || val > 12 || end > 12) && val != AllSpecInt)
                {
                    throw new FormatException(
                        "Month values must be between 1 and 12");
                }
            }
            else if (type == DayOfWeek)
            {
                if ((val == 0 || val > 7 || end > 7) && val != AllSpecInt
                    && val != NoSpecInt)
                {
                    throw new FormatException(
                        "Day-of-Week values must be between 1 and 7");
                }
            }

            if ((incr == 0 || incr == -1) && val != AllSpecInt)
            {
                if (val != -1)
                {
                    data.Add(val);
                }
                else
                {
                    data.Add(NoSpec);
                }
                return;
            }

            int startAt = val;
            int stopAt = end;

            if (val == AllSpecInt && incr <= 0)
            {
                incr = 1;
                data.Add(AllSpec); // put in a marker, but also fill values
            }

            if (type == Second || type == Minute)
            {
                if (stopAt == -1)
                {
                    stopAt = 59;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 0;
                }
            }
            else if (type == Hour)
            {
                if (stopAt == -1)
                {
                    stopAt = 23;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 0;
                }
            }
            else if (type == DayOfMonth)
            {
                if (stopAt == -1)
                {
                    stopAt = 31;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == Month)
            {
                if (stopAt == -1)
                {
                    stopAt = 12;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == DayOfWeek)
            {
                if (stopAt == -1)
                {
                    stopAt = 7;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1;
                }
            }
            else if (type == Year)
            {
                if (stopAt == -1)
                {
                    stopAt = MaxYear;
                }
                if (startAt == -1 || startAt == AllSpecInt)
                {
                    startAt = 1970;
                }
            }

            // if the end of the range is before the start, then we need to overflow into
            // the next day, month etc. This is done by adding the maximum amount for that
            // type, and using modulus max to determine the value being added.
            int max = -1;
            if (stopAt < startAt)
            {
                switch (type)
                {
                    case Second:
                        max = 60;
                        break;

                    case Minute:
                        max = 60;
                        break;

                    case Hour:
                        max = 24;
                        break;

                    case Month:
                        max = 12;
                        break;

                    case DayOfWeek:
                        max = 7;
                        break;

                    case DayOfMonth:
                        max = 31;
                        break;

                    case Year: throw new ArgumentException("Start year must be less than stop year");
                    default: throw new ArgumentException("Unexpected type encountered");
                }
                stopAt += max;
            }

            for (int i = startAt; i <= stopAt; i += incr)
            {
                if (max == -1)
                {
                    // ie: there's no max to overflow over
                    data.Add(i);
                }
                else
                {
                    // take the modulus to get the real value
                    int i2 = i % max;

                    // 1-indexed ranges should not include 0, and should include their max
                    if (i2 == 0 && (type == Month || type == DayOfWeek || type == DayOfMonth))
                    {
                        i2 = max;
                    }

                    data.Add(i2);
                }
            }
        }

        /// <summary>
        /// Gets the set of given type.
        /// </summary>
        /// <param name="type">The type of set to get.</param>
        /// <returns></returns>
        protected ISet<int> GetSet(int type)
        {
            switch (type)
            {
                case Second:
                    return seconds;

                case Minute:
                    return minutes;

                case Hour:
                    return hours;

                case DayOfMonth:
                    return daysOfMonth;

                case Month:
                    return months;

                case DayOfWeek:
                    return daysOfWeek;

                case Year:
                    return years;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <param name="s">The s.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        protected ValueSet GetValue(int v, string s, int i)
        {
            char c = s[i];
            StringBuilder s1 = new StringBuilder(v.ToString(CultureInfo.InvariantCulture));
            while (c >= '0' && c <= '9')
            {
                s1.Append(c);
                i++;
                if (i >= s.Length)
                {
                    break;
                }
                c = s[i];
            }
            ValueSet val = new ValueSet();
            if (i < s.Length)
            {
                val.pos = i;
            }
            else
            {
                val.pos = i + 1;
            }
            val.theValue = Convert.ToInt32(s1.ToString(), CultureInfo.InvariantCulture);
            return val;
        }

        /// <summary>
        /// Gets the numeric value from string.
        /// </summary>
        /// <param name="s">The string to parse from.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        protected int GetNumericValue(string s, int i)
        {
            int endOfVal = FindNextWhiteSpace(i, s);
            string val = s.Substring(i, endOfVal - i);
            return Convert.ToInt32(val, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the month number.
        /// </summary>
        /// <param name="s">The string to map with.</param>
        /// <returns></returns>
        protected int GetMonthNumber(string s)
        {
            if (monthMap.ContainsKey(s))
            {
                return monthMap[s];
            }

            return -1;
        }

        /// <summary>
        /// Gets the day of week number.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        protected int GetDayOfWeekNumber(string s)
        {
            if (dayMap.ContainsKey(s))
            {
                return dayMap[s];
            }

            return -1;
        }

        /// <summary>
        /// 根据给定的时间，获取下一次触发时间
        /// </summary>
        /// <param name="afterTimeUtc"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="LgyUtilException"></exception>
        public DateTimeOffset GetTimeAfter(DateTimeOffset afterTimeUtc)
        {
            // move ahead one second, since we're computing the time *after* the
            // given time
            afterTimeUtc = afterTimeUtc.AddSeconds(1);

            // CronTrigger does not deal with milliseconds
            DateTimeOffset d = CreateDateTimeWithoutMillis(afterTimeUtc);

            // change to specified time zone
            d = TimeZoneInfo.ConvertTime(d, TimeZone);

            bool gotOne = false;
            // loop until we've computed the next time, or we've past the endTime
            while (!gotOne)
            {
                SortedSet<int> st;
                int t;
                int sec = d.Second;

                // get second.................................................
                st = seconds.TailSet(sec);
                if (st.Count > 0)
                {
                    sec = st.First();
                }
                else
                {
                    sec = seconds.First();
                    d = d.AddMinutes(1);
                }
                d = new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, d.Minute, sec, d.Millisecond, d.Offset);

                int min = d.Minute;
                int hr = d.Hour;
                t = -1;

                // get minute.................................................
                st = minutes.TailSet(min);
                if (st.Count > 0)
                {
                    t = min;
                    min = st.First();
                }
                else
                {
                    min = minutes.First();
                    hr++;
                }
                if (min != t)
                {
                    d = new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, min, 0, d.Millisecond, d.Offset);
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTimeOffset(d.Year, d.Month, d.Day, d.Hour, min, d.Second, d.Millisecond, d.Offset);

                hr = d.Hour;
                int day = d.Day;
                t = -1;

                // get hour...................................................
                st = hours.TailSet(hr);
                if (st.Count > 0)
                {
                    t = hr;
                    hr = st.First();
                }
                else
                {
                    hr = hours.First();
                    day++;
                }
                if (hr != t)
                {
                    int daysInMonth = DateTime.DaysInMonth(d.Year, d.Month);
                    if (day > daysInMonth)
                    {
                        d = new DateTimeOffset(d.Year, d.Month, daysInMonth, d.Hour, 0, 0, d.Millisecond, d.Offset).AddDays(day - daysInMonth);
                    }
                    else
                    {
                        d = new DateTimeOffset(d.Year, d.Month, day, d.Hour, 0, 0, d.Millisecond, d.Offset);
                    }
                    d = SetCalendarHour(d, hr);
                    continue;
                }
                d = new DateTimeOffset(d.Year, d.Month, d.Day, hr, d.Minute, d.Second, d.Millisecond, d.Offset);

                day = d.Day;
                int mon = d.Month;
                t = -1;
                int tmon = mon;

                // get day...................................................
                bool dayOfMSpec = !daysOfMonth.Contains(NoSpec);
                bool dayOfWSpec = !daysOfWeek.Contains(NoSpec);
                if (dayOfMSpec && !dayOfWSpec)
                {
                    // get day by day of month rule
                    st = daysOfMonth.TailSet(day);
                    bool found = st.Any();
                    if (lastdayOfMonth)
                    {
                        if (!nearestWeekday)
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);
                            day -= lastdayOffset;

                            if (t > day)
                            {
                                mon++;
                                if (mon > 12)
                                {
                                    mon = 1;
                                    tmon = 3333; // ensure test of mon != tmon further below fails
                                    d = d.AddYears(1);
                                }
                                day = 1;
                            }
                        }
                        else
                        {
                            t = day;
                            day = GetLastDayOfMonth(mon, d.Year);
                            day -= lastdayOffset;

                            DateTimeOffset tcal = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);

                            int ldom = GetLastDayOfMonth(mon, d.Year);
                            DayOfWeek dow = tcal.DayOfWeek;

                            if (dow == System.DayOfWeek.Saturday && day == 1)
                            {
                                day += 2;
                            }
                            else if (dow == System.DayOfWeek.Saturday)
                            {
                                day -= 1;
                            }
                            else if (dow == System.DayOfWeek.Sunday && day == ldom)
                            {
                                day -= 2;
                            }
                            else if (dow == System.DayOfWeek.Sunday)
                            {
                                day += 1;
                            }

                            DateTimeOffset nTime = new DateTimeOffset(tcal.Year, mon, day, hr, min, sec, d.Millisecond, d.Offset);
                            if (nTime.ToUniversalTime() < afterTimeUtc)
                            {
                                day = 1;
                                mon++;
                            }
                        }
                    }
                    else if (nearestWeekday)
                    {
                        t = day;
                        day = daysOfMonth.First();

                        DateTimeOffset tcal = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);

                        int ldom = GetLastDayOfMonth(mon, d.Year);
                        DayOfWeek dow = tcal.DayOfWeek;

                        if (dow == System.DayOfWeek.Saturday && day == 1)
                        {
                            day += 2;
                        }
                        else if (dow == System.DayOfWeek.Saturday)
                        {
                            day -= 1;
                        }
                        else if (dow == System.DayOfWeek.Sunday && day == ldom)
                        {
                            day -= 2;
                        }
                        else if (dow == System.DayOfWeek.Sunday)
                        {
                            day += 1;
                        }

                        tcal = new DateTimeOffset(tcal.Year, mon, day, hr, min, sec, d.Offset);
                        if (tcal.ToUniversalTime() < afterTimeUtc)
                        {
                            day = daysOfMonth.First();
                            mon++;
                        }
                    }
                    else if (found)
                    {
                        t = day;
                        day = st.First();

                        // make sure we don't over-run a short month, such as february
                        int lastDay = GetLastDayOfMonth(mon, d.Year);
                        if (day > lastDay)
                        {
                            day = daysOfMonth.First();
                            mon++;
                        }
                    }
                    else
                    {
                        day = daysOfMonth.First();
                        mon++;
                    }

                    if (day != t || mon != tmon)
                    {
                        if (mon > 12)
                        {
                            d = new DateTimeOffset(d.Year, 12, day, 0, 0, 0, d.Offset).AddMonths(mon - 12);
                        }
                        else
                        {
                            // This is to avoid a bug when moving from a month
                            //with 30 or 31 days to a month with less. Causes an invalid datetime to be instantiated.
                            // ex. 0 29 0 30 1 ? 2009 with clock set to 1/30/2009
                            int lDay = DateTime.DaysInMonth(d.Year, mon);
                            if (day <= lDay)
                            {
                                d = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);
                            }
                            else
                            {
                                d = new DateTimeOffset(d.Year, mon, lDay, 0, 0, 0, d.Offset).AddDays(day - lDay);
                            }
                        }
                        continue;
                    }
                }
                else if (dayOfWSpec && !dayOfMSpec)
                {
                    // get day by day of week rule
                    if (lastdayOfWeek)
                    {
                        // are we looking for the last XXX day of
                        // the month?
                        int dow = daysOfWeek.First(); // desired
                        // d-o-w
                        int cDow = (int)d.DayOfWeek + 1; // current d-o-w
                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        int lDay = GetLastDayOfMonth(mon, d.Year);

                        if (day + daysToAdd > lDay)
                        {
                            // did we already miss the
                            // last one?
                            if (mon == 12)
                            {
                                //will we pass the end of the year?
                                d = new DateTimeOffset(d.Year, mon - 11, 1, 0, 0, 0, d.Offset).AddYears(1);
                            }
                            else
                            {
                                d = new DateTimeOffset(d.Year, mon + 1, 1, 0, 0, 0, d.Offset);
                            }
                            // we are promoting the month
                            continue;
                        }

                        // find date of last occurrence of this day in this month...
                        while (day + daysToAdd + 7 <= lDay)
                        {
                            daysToAdd += 7;
                        }

                        day += daysToAdd;

                        if (daysToAdd > 0)
                        {
                            d = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);
                            // we are not promoting the month
                            continue;
                        }
                    }
                    else if (nthdayOfWeek != 0)
                    {
                        // are we looking for the Nth XXX day in the month?
                        int dow = daysOfWeek.First(); // desired
                        // d-o-w
                        int cDow = (int)d.DayOfWeek + 1; // current d-o-w
                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        else if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        bool dayShifted = daysToAdd > 0;

                        day += daysToAdd;
                        int weekOfMonth = day / 7;
                        if (day % 7 > 0)
                        {
                            weekOfMonth++;
                        }

                        daysToAdd = (nthdayOfWeek - weekOfMonth) * 7;
                        day += daysToAdd;
                        if (daysToAdd < 0 || day > GetLastDayOfMonth(mon, d.Year))
                        {
                            if (mon == 12)
                            {
                                d = new DateTimeOffset(d.Year, mon - 11, 1, 0, 0, 0, d.Offset).AddYears(1);
                            }
                            else
                            {
                                d = new DateTimeOffset(d.Year, mon + 1, 1, 0, 0, 0, d.Offset);
                            }

                            // we are promoting the month
                            continue;
                        }
                        if (daysToAdd > 0 || dayShifted)
                        {
                            d = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);
                            // we are NOT promoting the month
                            continue;
                        }
                    }
                    else if (everyNthWeek != 0)
                    {
                        int cDow = (int)d.DayOfWeek + 1; // current d-o-w
                        int dow = daysOfWeek.First(); // desired
                        // d-o-w
                        st = daysOfWeek.TailSet(cDow);
                        if (st.Count > 0)
                        {
                            dow = st.First();
                        }

                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow + 7 * (everyNthWeek - 1);
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow) + 7 * (everyNthWeek - 1);
                        }

                        int lDay = GetLastDayOfMonth(mon, d.Year);

                        //if (day + daysToAdd > lDay)
                        //{
                        //    // will we pass the end of the month?

                        //    if (mon == 12)
                        //    {
                        //        //will we pass the end of the year?
                        //        d = new DateTimeOffset(d.Year, mon - 11, 1, 0, 0, 0, d.Offset).AddYears(1);
                        //    }
                        //    else
                        //    {
                        //        d = new DateTimeOffset(d.Year, mon + 1, 1, 0, 0, 0, d.Offset);
                        //    }
                        //    // we are promoting the month
                        //    continue;
                        //}
                        if (daysToAdd > 0)
                        {
                            // are we switching days?
                            d = new DateTimeOffset(d.Year, mon, day, 0, 0, 0, d.Offset);
                            d = d.AddDays(daysToAdd);
                            continue;
                        }
                    }
                    else
                    {
                        int cDow = (int)d.DayOfWeek + 1; // current d-o-w
                        int dow = daysOfWeek.First(); // desired
                        // d-o-w
                        st = daysOfWeek.TailSet(cDow);
                        if (st.Count > 0)
                        {
                            dow = st.First();
                        }

                        int daysToAdd = 0;
                        if (cDow < dow)
                        {
                            daysToAdd = dow - cDow;
                        }
                        if (cDow > dow)
                        {
                            daysToAdd = dow + (7 - cDow);
                        }

                        int lDay = GetLastDayOfMonth(mon, d.Year);

                        if (day + daysToAdd > lDay)
                        {
                            // will we pass the end of the month?

                            if (mon == 12)
                            {
                                //will we pass the end of the year?
                                d = new DateTimeOffset(d.Year, mon - 11, 1, 0, 0, 0, d.Offset).AddYears(1);
                            }
                            else
                            {
                                d = new DateTimeOffset(d.Year, mon + 1, 1, 0, 0, 0, d.Offset);
                            }
                            // we are promoting the month
                            continue;
                        }
                        if (daysToAdd > 0)
                        {
                            // are we switching days?
                            d = new DateTimeOffset(d.Year, mon, day + daysToAdd, 0, 0, 0, d.Offset);
                            continue;
                        }
                    }
                }
                else
                {
                    // dayOfWSpec && !dayOfMSpec
                    throw new Exception(
                        "Support for specifying both a day-of-week AND a day-of-month parameter is not implemented.");
                }

                d = new DateTimeOffset(d.Year, d.Month, day, d.Hour, d.Minute, d.Second, d.Offset);
                mon = d.Month;
                int year = d.Year;
                t = -1;

                // test for expressions that never generate a valid fire date,
                // but keep looping...
                if (year > MaxYear)
                {
                    throw new LgyUtilException("最多计算100年");
                }

                // get month...................................................
                st = months.TailSet(mon);
                if (st.Count > 0)
                {
                    t = mon;
                    mon = st.First();
                }
                else
                {
                    mon = months.First();
                    year++;
                }
                if (mon != t)
                {
                    d = new DateTimeOffset(year, mon, 1, 0, 0, 0, d.Offset);
                    continue;
                }
                d = new DateTimeOffset(d.Year, mon, d.Day, d.Hour, d.Minute, d.Second, d.Offset);
                year = d.Year;
                t = -1;

                // get year...................................................
                st = years.TailSet(year);
                if (st.Count > 0)
                {
                    t = year;
                    year = st.First();
                }
                else
                {
                    throw new LgyUtilException("最多计算100年");
                } // ran out of years...

                if (year != t)
                {
                    d = new DateTimeOffset(year, 1, 1, 0, 0, 0, d.Offset);
                    continue;
                }
                d = new DateTimeOffset(year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Offset);

                //apply the proper offset for this date
                var temp = TimeZone.IsAmbiguousTime(d.DateTime) ? TimeZone.GetAmbiguousTimeOffsets(d.DateTime).Max() : TimeZone.GetUtcOffset(d.DateTime);
                d = new DateTimeOffset(d.DateTime, temp);

                gotOne = true;
            } // while( !done )

            return d.ToUniversalTime();
        }

        /// <summary>
        /// Creates the date time without milliseconds.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        protected static DateTimeOffset CreateDateTimeWithoutMillis(DateTimeOffset time)
        {
            return new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Offset);
        }

        /// <summary>
        /// Advance the calendar to the particular hour paying particular attention
        /// to daylight saving problems.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="hour">The hour.</param>
        /// <returns></returns>
        protected static DateTimeOffset SetCalendarHour(DateTimeOffset date, int hour)
        {
            // Java version of Quartz uses lenient calendar
            // so hour 24 creates day increment and zeroes hour
            int hourToSet = hour;
            if (hourToSet == 24)
            {
                hourToSet = 0;
            }
            DateTimeOffset d = new DateTimeOffset(date.Year, date.Month, date.Day, hourToSet, date.Minute, date.Second, date.Millisecond, date.Offset);
            if (hour == 24)
            {
                // increment day
                d = d.AddDays(1);
            }
            return d;
        }

        /// <summary>
        /// Determines whether given year is a leap year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        /// 	<c>true</c> if the specified year is a leap year; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        /// <summary>
        /// Gets the last day of month.
        /// </summary>
        /// <param name="monthNum">The month num.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        protected int GetLastDayOfMonth(int monthNum, int year)
        {
            return DateTime.DaysInMonth(year, monthNum);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var copy = new CronExpression(CronExpressionString);
            copy.TimeZone = TimeZone;
            return copy;
        }
    }

    /// <summary>
    /// Helper class for cron expression handling.
    /// </summary>
    internal class ValueSet
    {
        /// <summary>
        /// The value.
        /// </summary>
        public int theValue;

        /// <summary>
        /// The position.
        /// </summary>
        public int pos;
    }
}