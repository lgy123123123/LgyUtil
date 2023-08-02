using LgyUtil.TimerUtilModel.Cron;
using System;
using System.Collections.Generic;

namespace LgyUtil.TimerUtilModel
{
    /// <summary>
    /// cron表达式触发器
    /// </summary>
    internal class TriggerCron : ITrigger
    {
        public EnumTriggerType Type => EnumTriggerType.Cron;

        public DateTime NextTime { get; set; } = DateTime.Now;

        /// <summary>
        /// cron表达式解析对象
        /// </summary>
        public CronExpression Expression { get; set; }

        public TriggerCron(string cron)
        {
            Expression = new CronExpression(cron);
        }

        public List<DateTime> ComputedNext5Times()
        {
            List<DateTime> result = new List<DateTime>();
            var dtNext = NextTime;
            for (int i = 0; i < 5; i++)
            {
                dtNext = Expression.GetTimeAfter(dtNext).LocalDateTime;
                result.Add(dtNext);
            }
            return result;
        }

        public DateTime GetNextTime()
        {
            return Expression.GetTimeAfter(NextTime).LocalDateTime;
        }

        public void IsValid()
        {
            //上面构造函数中，实例化表达式解析时，已经验证了
        }
    }
}