using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public static class ResponseProviderBus
    {
        public static async Task<IResponse<TResult>> GetAsync<TRequest, TResult>(IServiceProvider sp, IExecutionContext context, ILogger logger, TRequest request, bool failSafe)
        {
            var errors = await sp.ValidateAsync(context, request, logger);

            if (errors.Any())
            {
                return Response.Failed<TResult>(errors);
            }

            var handler = sp.GetServices<IResponseHandlerAsync<TRequest, TResult>>()
                            ?.Where(h => h.IsApplicable(context, request)).FirstOrDefault();

            if (handler == null)
            {
                if (failSafe)
                {
                    logger.LogWarning($"No handler found that impement IResponseProvider<{typeof(TRequest).FullName},{typeof(TResult).FullName}>");

                    return Response.NoHandler<TResult>();
                }

                throw new Exception($"No response handler defined for type {typeof(IResponseHandlerAsync<TRequest, TResult>)}");
            };


#if DEBUG
            var timer = Timer.Start(logger, handler);
#endif
            var response = await handler.Handle(context, request);
#if DEBUG
            timer.Completed();
#endif

            if (response == null) return Response.Failed<TResult>();

            await sp.FilterAsync(context, response, logger);

            return response;
        }

        public static async Task<TResult> GetAsync<TResult>(IServiceProvider sp, IExecutionContext context, ILogger logger, bool failSafe)
        {
            var handler = sp.GetServices<IResponseHandlerAsync<TResult>>()
                            ?.Where(h => h.IsApplicable(context)).FirstOrDefault();

            if (handler == null)
            {
                if(failSafe)
                {
                    logger.LogWarning($"No handler found that impement IResponseProvider<{typeof(TResult).FullName}>");

                    return default(TResult);
                }

                throw new RequestBusException($"No response handler defined for type {typeof(IResponseHandlerAsync<TResult>)}");
            }

#if DEBUG
            var timer = Timer.Start(logger, handler);
#endif
            var response = await handler.Handle(context);

#if DEBUG
            timer.Completed();
#endif
            if (response == null)
            {
                return default(TResult);
            }

            await sp.FilterAsync(context, response, logger);

            return response.Result;
        }

        private static ResponseUnit<TResult> Convert<TResult>(IResponse<TResult> source, ExecutionHintType hintType)
        {
            if (source == null) return null;

            return new ResponseUnit<TResult>
            {
                Errors = source.Errors,
                Result = source.Result,
                IsMainResponse = hintType == ExecutionHintType.Main,
                IsSucceed = source.IsSucceed
            };
        }
    }
}
