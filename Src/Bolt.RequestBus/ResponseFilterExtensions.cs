using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    internal static class ResponseFilterExtensions
    {
        internal static async Task FilterAsync<TResult>(this IServiceProvider service, IExecutionContext context, IResponse<TResult> response, ILogger logger)
        {
            var filters = service.GetServices<IResponseFilterAsync<TResult>>()
                            ?.Where(h => h.IsApplicable(context));

            if (filters == null) return;

            foreach(var filter in filters)
            {
#if DEBUG
                var sw = Stopwatch.StartNew();

                logger.LogDebug($"Start executing response filter {filter.GetType().FullName}");
#endif
                await filter.Filter(context, response);

#if DEBUG
                logger.LogDebug($"Finished executing response filter {filter.GetType().FullName}");

                sw.Stop();

                logger.LogInformation($"{filter.GetType().Name}|{sw.ElapsedMilliseconds}ms");
#endif
            }
        }

        internal static async Task FilterAsync<TRequest,TResult>(this IServiceProvider service, IExecutionContext context, TRequest request, IResponse<TResult> response, ILogger logger)
        {
            var filters = service.GetServices<IResponseFilterAsync<TRequest,TResult>>()
                            ?.Where(h => h.IsApplicable(context,request));

            if (filters == null) return;

            foreach (var filter in filters)
            {
#if DEBUG
                var sw = Stopwatch.StartNew();

                logger.LogDebug($"Start executing response filter {filter.GetType().FullName}");
#endif

                await filter.Filter(context, request, response);

#if DEBUG
                logger.LogDebug($"Finished executing response filter {filter.GetType().FullName}");

                sw.Stop();

                logger.LogInformation($"{filter.GetType().Name}|{sw.ElapsedMilliseconds}ms");
#endif  
            }
        }
    }
}
