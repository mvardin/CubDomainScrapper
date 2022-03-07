using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CubDomain.Crawler.WinUI
{
    public static class Tasks
    {
        public static async Task StartAndWaitAllThrottledAsync(IEnumerable<Task> tasksToRun, int maxTasksToRunInParallel, int timeoutInMilliseconds, CancellationToken cancellationToken = new CancellationToken())
        {
            // Convert to a list of tasks so that we don't enumerate over it multiple times needlessly.
            var tasks = tasksToRun.ToList();

            using (var throttler = new SemaphoreSlim(maxTasksToRunInParallel))
            {
                var postTaskTasks = new List<Task>();

                // Have each task notify the throttler when it completes so that it decrements the number of tasks currently running.
                tasks.ForEach(t => postTaskTasks.Add(t.ContinueWith(tsk => throttler.Release())));

                // Start running each task.
                foreach (var task in tasks)
                {
                    // Increment the number of tasks currently running and wait if too many are running.
                    await throttler.WaitAsync(timeoutInMilliseconds, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    Parallel.Invoke(() =>
                    {
                        task.Start();
                    });
                }

                // Wait for all of the provided tasks to complete.
                // We wait on the list of "post" tasks instead of the original tasks, otherwise there is a potential race condition where the throttler&#39;s using block is exited before some Tasks have had their "post" action completed, which references the throttler, resulting in an exception due to accessing a disposed object.
                await Task.WhenAll(postTaskTasks.ToArray());
            }
        }
        public static void Delay(int min, int max)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int waitForSec = rnd.Next(min, max);
            Task.Delay(waitForSec * 1000).Wait();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult">
        /// Key : is tasks completed or not ...
        /// value : is result of task ...
        /// </typeparam>
        /// <param name="task"></param>
        /// <param name="timeout">timeout in miliseconds ...</param>
        /// <returns></returns>
        public static async Task<KeyValuePair<bool, TResult>> RunInTime<TResult>(Task<TResult> task, int timeout)
        {
            try
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                {
                    var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                    if (completedTask == task)
                    {
                        timeoutCancellationTokenSource.Cancel();
                        return new KeyValuePair<bool, TResult>(true, await task);
                    }
                    else
                    {
                        return new KeyValuePair<bool, TResult>(false, default(TResult));
                    }
                }
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, TResult>(false, default(TResult));
            }
        }
        public static async Task<bool> RunInTime(Task task, int timeout)
        {
            try
            {
                using (var timeoutCancellationTokenSource = new CancellationTokenSource())
                {
                    var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                    if (completedTask == task)
                    {
                        timeoutCancellationTokenSource.Cancel();
                        await task;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}