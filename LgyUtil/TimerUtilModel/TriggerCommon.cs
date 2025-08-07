using System;
using System.Collections.Generic;

namespace LgyUtil.TimerUtilModel
{
    /// <summary>
    /// 普通时间类型触发器
    /// </summary>
    internal class TriggerCommon : ITrigger
    {
        public EnumTriggerType Type => EnumTriggerType.Common;

        public DateTime NextTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 执行时间间隔
        /// </summary>
        public TimeSpan Interval { get; }

        public TriggerCommon(TimeSpan interval)
        {
            this.Interval = interval;
        }

        public List<DateTime> ComputedNext5Times()
        {
            List<DateTime> listDt = new List<DateTime>(5);
            DateTime dtNext = NextTime;
            for (int i = 0; i < 5; i++)
            {
                dtNext = dtNext.Add(Interval);
                listDt.Add(dtNext);
            }
            return listDt;
        }

        public DateTime GetNextTime(DateTime? nowTime = null)
        {
            return (nowTime??NextTime).Add(Interval);
        }

        public void IsValid()
        {
            if (Interval <= TimeSpan.Zero)
                throw new LgyUtilException("时间间隔必须大于0");
        }
    }
}