using System;

namespace LgyUtil.TimerUtilModel
{
    /// <summary>
    /// job执行信息
    /// </summary>
    public sealed class JobExecInfo
    {
        internal JobExecInfo(string jobName, int execCount)
        {
            JobName = jobName;
            ExecCount = execCount;
        }

        internal JobExecInfo(string jobName, int execCount, DateTime nextTime, DateTime thisTime)
        {
            JobName = jobName;
            ExecCount = execCount;
            NextTime = nextTime;
            ThisTime = thisTime;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; }

        /// <summary>
        /// 已执行次数(不算本次执行)
        /// </summary>
        public int ExecCount { get; }

        /// <summary>
        /// 下次触发时间
        /// </summary>
        public DateTime NextTime { get; }

        /// <summary>
        /// 本次执行时间
        /// </summary>
        public DateTime ThisTime { get; }
    }
}