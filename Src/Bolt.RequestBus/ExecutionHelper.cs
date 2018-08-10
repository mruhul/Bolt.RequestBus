using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus
{
    internal static class ExecutionHelper
    {
        public static async Task<T> Execute<T>(Func<Task<T>> func, object taskDesc, ILogger logger)
        {
#if DEBUG
            logger.LogDebug("{0}: start executing", taskDesc);

            var sw = Stopwatch.StartNew();
#endif
            var result = await func.Invoke();
#if DEBUG
            sw.Stop();
            logger.LogDebug("{0}: finished executing", taskDesc);
            logger.LogDebug("{0} took {1}ms", taskDesc?.GetType().FullName, sw.ElapsedMilliseconds);
#endif
            return result;
        }

        public static async Task Execute(Func<Task> func, object taskDesc, ILogger logger)
        {
#if DEBUG
            logger.LogDebug("{0}: start executing", taskDesc);
            var sw = Stopwatch.StartNew();
#endif
            await func.Invoke();
#if DEBUG
            sw.Stop();
            logger.LogDebug("{0}: finished executing", taskDesc);
            logger.LogDebug("{0} took {1}ms", taskDesc?.GetType().FullName, sw.ElapsedMilliseconds);
#endif
        }
    }
}
