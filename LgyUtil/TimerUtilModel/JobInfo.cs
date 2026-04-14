using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LgyUtil.TimerUtilModel
{
    public class JobInfo
    {
        /// <summary>
        /// 所有定时调度集合
        /// </summary>
        internal static readonly ConcurrentDictionary<string, JobInfo> AllJobDic = new ConcurrentDictionary<string, JobInfo>();

        /// <summary>
        /// 日志文件名
        /// </summary>
        private const string LogFileName = "Log";

        /// <summary>
        /// job运行记录日志的文件夹名
        /// </summary>
        private const string LogDirName = "TimerUtil";

        /// <summary>
        /// 添加job
        /// </summary>
        /// <param name="jobName">job名称</param>
        /// <param name="trigger">触发器</param>
        /// <param name="doing">执行方法</param>
        /// <param name="options">参数</param>
        /// <exception cref="LgyUtilException"></exception>
        internal static void AddJob(string jobName, ITrigger trigger, Action<JobExecInfo> doing, JobOption options)
        {
            var job = new JobInfo()
            {
                JobName = jobName,
                Doing = doing,
                Options = options ?? new JobOption(),
                Trigger = trigger
            };
            job.CheckJobInfo();
            if (!AllJobDic.TryAdd(job.JobName, job))
                throw new LgyUtilException("job添加失败，job名称已存在：" + job.JobName);
            if (options?.EndTime.HasValue==true && options.EndTime.Value < DateTime.Now)
                throw new LgyUtilException("job添加失败，结束时间不能小于当前时间：" + job.JobName);
            job.StartJob();
        }

        /// <summary>
        /// 根据名称获取job
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        /// <exception cref="LgyUtilException"></exception>
        internal static JobInfo GetJob(string jobName)
        {
            return AllJobDic.TryGetValue(jobName, out var job) ? job : null;
        }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 触发器
        /// </summary>
        internal ITrigger Trigger { get; set; }

        /// <summary>
        /// 执行的任务
        /// </summary>
        private Action<JobExecInfo> Doing { get; set; }

        /// <summary>
        /// 任务选项
        /// </summary>
        public JobOption Options { get; set; }

        /// <summary>
        /// job最终调度
        /// </summary>
        private Timer JobTimer { get; set; }

        private int _ExecCount;

        /// <summary>
        /// 已执行次数
        /// </summary>
        public int ExecCount => _ExecCount;

        /// <summary>
        /// 本次是否执行完成
        /// </summary>
        private bool IsFinish { get; set; } = true;

        /// <summary>
        /// 是否已被删除
        /// </summary>
        private bool IsDelete { get; set; }

        /// <summary>
        /// 验证job信息
        /// </summary>
        /// <exception cref="LgyUtilException"></exception>
        internal void CheckJobInfo()
        {
            if (AllJobDic.ContainsKey(JobName))
                throw new LgyUtilException("job名称已存在：" + JobName);
            if (Doing is null)
                throw new LgyUtilException(JobName + ":doing参数不能为空");
            Trigger.IsValid();
        }

        /// <summary>
        /// 获取已添加的任务未来5次触发时间(
        /// </summary>
        /// <returns></returns>
        public IList<DateTime> GetJobNext5TriggerTimes()
        {
            return Trigger.ComputedNext5Times();
        }

        /// <summary>
        /// 停止job
        /// </summary>
        public void StopJob()
        {
            if (IsDelete)
                return;

            IsDelete = true; //标记删除

            JobTimer?.Dispose();
            AllJobDic.TryRemove(JobName, out _);
            if (Options.AfterStopDoing != null)
            {
                JobExecInfo execInfo = new JobExecInfo(JobName, ExecCount);
                Options.AfterStopDoing(execInfo);
            }
        }

        /// <summary>
        /// 开始执行job
        /// </summary>
        public void StartJob()
        {
            var firstSpan = GetFirstExecSpan();
            JobTimer = new Timer((obj) =>
            {
                try
                {
                    //超过结束时间，停止任务
                    if (Options.EndTime != null && Options.EndTime.Value <= DateTime.Now)
                    {
                        StopJob();
                        return;
                    }
                    
                    //下次执行时间
                    DateTime nextExecDate = Trigger.GetNextTime(DateTime.Now); //防止服务器时间错误，用当前执行时间来计算下次触发时间
                    if (ExecCount == 0 && Options.RunNow) //立即执行，不修改下次执行时间
                        nextExecDate = Trigger.NextTime;

                    //本次是否执行
                    bool isExec = true;
                    if (!Options.ContinueNotFinish)
                        isExec = IsFinish;

                    var nextDueTime = nextExecDate - DateTime.Now; //下次执行时间间隔
                    var findTimes = 1; //查找次数
                    //防止触发时间间隔小于0，重新计算,20次后停止
                    while (nextDueTime.TotalMilliseconds < 0)
                    {
                        if (findTimes > 20)
                        {
                            LogUtil.AddErrorLog($"\"{JobName}\"计算下次触发时间失败，已停止任务", LogFileName, LogDirName);
                            StopJob();
                            return;
                        }
                        LogUtil.AddErrorLog($"\"{JobName}\"执行异常：计算下次触发时间异常，200毫秒后，重新计算下次触发时间，当前次数{findTimes}，失败20次后，停止任务", LogFileName, LogDirName);
                        Thread.Sleep(200);
                        nextExecDate = Trigger.GetNextTime(DateTime.Now);
                        nextDueTime = nextExecDate - DateTime.Now;
                        findTimes++;
                    }

                    //设置下次执行时间
                    JobTimer.Change(nextDueTime, TimeSpan.FromMilliseconds(-1));

                    //返给用户的信息
                    JobExecInfo execInfo = new JobExecInfo(JobName, ExecCount, nextExecDate, DateTime.Now);

                    //标记未完成
                    IsFinish = false;

                    Task.Run(() =>
                    {
                        try
                        {
                            if (isExec)
                                Doing(execInfo);
                            IsFinish = true;
                        }
                        catch (Exception e)
                        {
                            Options.ErrorDoing?.Invoke(e, execInfo);
                            //遇到错误，停止job
                            if (!Options.ErrorContinue)
                            {
                                StopJob();
                            }
                        }
                    });
                    //执行总次数+1
                    if (isExec)
                        Interlocked.Increment(ref _ExecCount);
                    Trigger.NextTime = nextExecDate;
                    //到结束时间，停止job，或者到最大次数，结束job
                    if ((Options.EndTime != null && Options.EndTime.Value <= DateTime.Now)
                        || (Options.MaxExecTimes > 0 && ExecCount >= Options.MaxExecTimes))
                        StopJob();
                }
                catch (Exception e)
                {
                    LogUtil.AddErrorLog($"\"{JobName}\"执行异常，任务已停止：{e.Message}", LogFileName, LogDirName);
                }
            }, null, firstSpan, TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// 获取任务开始执行距今的时间间隔
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetFirstExecSpan()
        {
            TimeSpan firstSpan;
            var nowDate = DateTime.Now;
            //立即执行
            if (Options.RunNow)
            {
                //既有立即执行，又有开始时间
                if (Options.StartTime.HasValue && Options.StartTime.Value > nowDate)
                {
                    if (Trigger.Type == EnumTriggerType.Common) //普通类型，开始时间即为下次触发时间
                        Trigger.NextTime = Options.StartTime.Value;
                    else //cron类型，需要以开始时间减1秒，再判断下次执行时间，有可能开始时间就是触发时间
                    {
                        Trigger.NextTime = Options.StartTime.Value.AddSeconds(-1);
                        Trigger.NextTime = Trigger.GetNextTime();
                    }
                }
                //只有立即执行，直接赋值下次执行时间
                else
                    Trigger.NextTime = Trigger.GetNextTime();

                firstSpan = TimeSpan.Zero; //返回立即执行
            }
            //只有开始时间
            else if (Options.StartTime.HasValue && Options.StartTime.Value >= nowDate)
            {
                //立即执行
                if (Options.StartTime == nowDate)
                {
                    Trigger.NextTime = Trigger.GetNextTime();
                    firstSpan = TimeSpan.Zero;
                }
                //计算开始时间
                else
                {
                    Trigger.NextTime = Options.StartTime.Value;
                    Trigger.NextTime = Trigger.GetNextTime();
                    firstSpan = Trigger.NextTime - nowDate; //等待到达开始时间
                }
            }
            //其它，根据时间间隔，计算下次执行时间
            else
            {
                Trigger.NextTime = Trigger.GetNextTime();
                firstSpan = Trigger.NextTime - nowDate;
            }

            return firstSpan;
        }
    }
}