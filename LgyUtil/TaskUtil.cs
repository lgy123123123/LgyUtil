using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LgyUtil
{
    /// <summary>
    /// task线程帮助类
    /// </summary>
    public static class TaskUtil
    {
        /// <summary>
        /// 设置异步超时时间
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("操作已超时");
                }
            }
        }
        /// <summary>
        /// 设置异步超时时间
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task;  // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("操作已超时");
                }
            }
        }
        /// <summary>
        /// 获取一个可以限制最大并行线程数的帮助类，需要using使用！！！！！
        /// </summary>
        /// <param name="maxTaskCount">最大线程数</param>
        /// <returns></returns>
        public static TaskMaxUtil GetTaskMaxUtil(int maxTaskCount = 5)
        {
            return new TaskMaxUtil(maxTaskCount);
        }
        /// <summary>
        /// 限制并行任务数帮助类，使用完，必须释放
        /// </summary>
        public sealed class TaskMaxUtil:IDisposable
        {
            /// <summary>
            /// 信号量
            /// </summary>
            private SemaphoreSlim semaphore { get; set; }
            /// <summary>
            /// 所有任务
            /// </summary>
            public List<Task> TaskAll { get; private set; }=new List<Task>();
            /// <summary>
            /// 初始化帮助类
            /// </summary>
            /// <param name="maxTaskCount">最大并行线程数，默认5</param>
            public TaskMaxUtil(int maxTaskCount = 5)
            {
                semaphore = new SemaphoreSlim(maxTaskCount, maxTaskCount);
            }
            /// <summary>
            /// 添加任务并立即执行
            /// </summary>
            /// <param name="action">执行的代码</param>
            /// <param name="millisecondsTimeout">超时时间，默认永不超时</param>
            /// <returns></returns>
            public Task AddRunTask(Action action, int millisecondsTimeout = -1)
            {
                var task = Task.Run(() =>
                {
                    if (semaphore.Wait(millisecondsTimeout))
                    {
                        action();
                        semaphore.Release();
                    }
                });
                TaskAll.Add(task);
                return task;
            }
            /// <summary>
            /// 等待所有线程执行完毕
            /// </summary>
            /// <param name="millisecondsTimeout">超时时间，默认永不超时</param>
            /// <returns>返回是否全部执行完成</returns>
            public bool WaitAll(int millisecondsTimeout = -1)
            {
                if (TaskAll.IsNullOrEmpty())
                    return true;

                return Task.WaitAll(TaskAll.ToArray(), millisecondsTimeout);
            }

            public void Dispose()
            {
                //手动释放
                semaphore.Dispose();
            }
        }
    }
}
