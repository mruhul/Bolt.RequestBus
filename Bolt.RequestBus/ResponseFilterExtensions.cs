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
        internal static async Task FilterAsync<TResult>(this IServiceProvider service, IExecutionContextReader context, IResponse<TResult> response, ILogger logger)
        {
            if (response == null || !response.IsSucceed || response.Result == null) return;
            
            var filters = service.GetServices<IResponseFilterAsync<TResult>>()
                            ?.Where(h => h.IsApplicable(context));

            if (filters == null) return;

            foreach(var filter in filters)
            {
                await filter.Filter(context, response);
            }
        }

        internal static async Task FilterAsync<TRequest,TResult>(this IServiceProvider service, IExecutionContextReader context, TRequest request, IResponse<TResult> response, ILogger logger)
        {
            if (response == null || !response.IsSucceed || response.Result == null) return;

            var filters = service.GetServices<IResponseFilterAsync<TRequest,TResult>>()
                            ?.Where(h => h.IsApplicable(context,request));

            if (filters == null) return;

            foreach (var filter in filters)
            {
                await filter.Filter(context, request, response);
            }
        }
    }
}
