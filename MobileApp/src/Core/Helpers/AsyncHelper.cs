using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            try
            {
                return AsyncHelper._myTaskFactory
                    .StartNew<Task<TResult>>(func)
                    .Unwrap<TResult>()
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {
                throw;
            }
        }

        public static void RunSync(Func<Task> func)
        {
            try
            {
                AsyncHelper._myTaskFactory
                    .StartNew<Task>(func)
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {
                throw;
            }
        }
    }
}


