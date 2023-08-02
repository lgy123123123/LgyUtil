using LgyUtil.TimerUtilModel;
using System;

namespace LgyUtil
{
    /// <summary>
    /// 任务选项
    /// </summary>
    public sealed class JobOption
    {
        /// <summary>
        ///任务开始执行时间
        ///<para>若设置了RunNow，会先执行一次，再按照StartTime执行</para>
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        ///任务结束执行时间(结束后，自动删除任务)
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 立即执行一次，默认false
        /// <para>若设置了StartTime，立即执行后，等到达StartTime后执行第二次</para>
        /// </summary>
        public bool RunNow { get; set; }

        /// <summary>
        /// 上次执行未结束，到下次触发时间，是否继续触发本次任务，默认false
        /// <para>true:不管上次执行是否结束，本次依然执行</para>
        /// <para>false:上次未结束，跳过本次任务，等待下次触发(此任务只会有一个线程在执行)</para>
        /// </summary>
        public bool ContinueNotFinish { get; set; }

        /// <summary>
        /// 最大执行次数，默认无限次数执行，到达最大次数时，停止并删除任务
        /// </summary>
        public int MaxExecTimes { get; set; }

        /// <summary>
        /// 发生错误时，是否继续执行，默认false，发生错误，停止job
        /// <para>业务中添加try catch，可避免触发</para>
        /// </summary>
        public bool ErrorContinue { get; set; }

        /// <summary>
        /// 发生错误时，执行的方法
        /// </summary>
        public Action<Exception, JobExecInfo> ErrorDoing { get; set; }

        /// <summary>
        /// 任务停止之后执行
        /// </summary>
        public Action<JobExecInfo> AfterStopDoing { get; set; }
    }
}