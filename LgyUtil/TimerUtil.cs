using System;
using System.Collections.Generic;
using LgyUtil.TimerUtilModel;

namespace LgyUtil
{
    /// <summary>
    /// 定时任务帮助类(很简易，请注意多线程的参数使用)
    /// </summary>
    public sealed class TimerUtil
    {
        /// <summary>
        /// 添加cron表达式任务(支持6-7位表达式)
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="cron">cron表达式(6-7位)</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddCronJob(string jobName,string cron,Action<JobExecInfo> doing, JobOption options=null)
        {
            JobInfo.AddJob(jobName,new TriggerCron(cron),doing,options);
        }

        /// <summary>
        /// 添加秒任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="second">间隔秒数</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddSecondJob(string jobName,double second,Action<JobExecInfo> doing,JobOption options = null)
        {
            JobInfo.AddJob(jobName, new TriggerCommon(TimeSpan.FromSeconds(second)), doing, options);
        }

        /// <summary>
        /// 添加分钟任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="minute">间隔分钟数</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddMinuteJob(string jobName, double minute, Action<JobExecInfo> doing, JobOption options = null)
        {
            JobInfo.AddJob(jobName, new TriggerCommon(TimeSpan.FromMinutes(minute)), doing, options);
        }

        /// <summary>
        /// 添加小时任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="hour">间隔小时数</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddHourJob(string jobName, double hour, Action<JobExecInfo> doing, JobOption options = null)
        {
            JobInfo.AddJob(jobName, new TriggerCommon(TimeSpan.FromHours(hour)), doing, options);
        }

        /// <summary>
        /// 添加天任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="day">间隔天数</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddDayJob(string jobName, double day, Action<JobExecInfo> doing, JobOption options = null)
        {
            JobInfo.AddJob(jobName, new TriggerCommon(TimeSpan.FromHours(day)), doing, options);
        }

        /// <summary>
        /// 添加自定义时间间隔任务（毫秒不准，尽量不要用）
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="customTimeSpan">自定义间隔时间</param>
        /// <param name="doing">执行的任务</param>
        /// <param name="options">选项</param>
        public static void AddCustomJob(string jobName, TimeSpan customTimeSpan, Action<JobExecInfo> doing, JobOption options = null)
        {
            JobInfo.AddJob(jobName, new TriggerCommon(customTimeSpan), doing, options);
        }

        /// <summary>
        /// 获取已添加的任务未来5次触发时间(任务名称错误时，返回null)
        /// </summary>
        /// <param name="jobName">已添加任务名称</param>
        /// <returns></returns>
        public static IList<DateTime> GetJobNext5TriggerTimes(string jobName)
        {
            return JobInfo.GetJob(jobName)?.Trigger.ComputedNext5Times();
        }
        
        /// <summary>
        /// 获取未来5次触发时间(任务名称错误时，返回null)
        /// </summary>
        /// <param name="cron">cron表达式</param>
        /// <returns></returns>
        public static IList<DateTime> GetNext5TriggerTimes(string cron)
        {
            TriggerCron tc = new TriggerCron(cron);
            return tc.ComputedNext5Times();
        } 

        /// <summary>
        /// 停止并移除job
        /// </summary>
        /// <param name="jobName">任务名称</param>
        public static void StopJob(string jobName)
        {
            JobInfo.GetJob(jobName)?.StopJob();
        }

        /// <summary>
        /// 延迟执行1次，类似js中的setTimeout
        /// </summary>
        /// <param name="delay">等待时长</param>
        /// <param name="action">等待时长结束后，执行的方法</param>
        public static void SetTimeout(TimeSpan delay, Action action)
        {
            AddCustomJob("timeout:" + Guid.NewGuid().ToString("N"), delay, info =>
            {
                action?.Invoke();
            }, new JobOption { EndTime = DateTime.Now.Add(delay) });
        }

        /// <summary>
        /// 延迟执行1次，类似js中的setTimeout
        /// </summary>
        /// <param name="second">等待秒数</param>
        /// <param name="action">等待时长结束后，执行的方法</param>
        /// <returns></returns>
        public static void SetTimeoutSecond(int second, Action action)
        {
            SetTimeout(TimeSpan.FromSeconds(second), action);
        }

        /// <summary>
        /// 延迟执行1次，类似js中的setTimeout
        /// </summary>
        /// <param name="minute">等待分钟数</param>
        /// <param name="action">等待时长结束后，执行的方法</param>
        /// <returns></returns>
        public static void SetTimeoutMinute(int minute, Action action)
        {
            SetTimeout(TimeSpan.FromMinutes(minute), action);
        }
    }
}
