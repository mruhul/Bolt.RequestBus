using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus
{
    public interface IRequestHandlerAsync<TRequest> : IApplicable<TRequest>
    {
        Task<IResponse> Handle(IExecutionContext context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest> : IRequestHandlerAsync<TRequest>
    {
        Task<IResponse> IRequestHandlerAsync<TRequest>.Handle(IExecutionContext context, TRequest request)
        {
            return Handle(context, request)
                .ContinueWith(t => Response.Succeed());
        }

        protected abstract Task Handle(IExecutionContext context, TRequest request);

        public virtual bool IsApplicable(IExecutionContext context, TRequest request) => true;
    }

    public interface IRequestHandlerAsync<TRequest,TResult> : IApplicable<TRequest>
    {
        Task<IResponse<TResult>> Handle(IExecutionContext context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest, TResult> : IRequestHandlerAsync<TRequest, TResult>
    {
        protected abstract Task<TResult> Handle(IExecutionContext context, TRequest request);

        public virtual bool IsApplicable(IExecutionContext context, TRequest request) => true;

        Task<IResponse<TResult>> IRequestHandlerAsync<TRequest, TResult>.Handle(IExecutionContext context, TRequest request)
        {
            return Handle(context, request)
                .ContinueWith(t => Response.Succeed(t.Result));
        }
    }


    internal class RequestHandlerBus
    {
        internal static async Task<IResponse> SendAsync<TRequest>(IServiceProvider serviceProvider, IExecutionContext context, ILogger logger, TRequest request, bool failSafe)
        {
            var errors = await serviceProvider.ValidateAsync(context, request, logger);

            if(errors != null && errors.Any())
            {
                return Response.Failed(errors);
            }

            var handler = serviceProvider.GetServices<IRequestHandlerAsync<TRequest>>()
                            ?.Where(h => h.IsApplicable(context, request)).FirstOrDefault();

            if(handler == null)
            {
                if (failSafe) return Response.NoHandler();

                throw new RequestBusException($"No request handler found that implement IRequestHandlerAsync<{typeof(TRequest).FullName}>");
            }

            return await ExecutionHelper.Execute(() => handler.Handle(context, request), handler, logger);
        }

        internal static async Task<IResponse<TResult>> SendAsync<TRequest,TResult>(IServiceProvider serviceProvider, IExecutionContext context, ILogger logger, TRequest request, bool failSafe)
        {
            var errors = await serviceProvider.ValidateAsync(context, request, logger);

            if (errors != null && errors.Any())
            {
                return Response.Failed<TResult>(errors);
            }

            var handler = serviceProvider.GetServices<IRequestHandlerAsync<TRequest,TResult>>()
                            ?.Where(h => h.IsApplicable(context, request)).FirstOrDefault();

            if (handler == null)
            {
                if (failSafe) return Response.NoHandler<TResult>();

                throw new RequestBusException($"No request handler found that implement IRequestHandlerAsync<{typeof(TRequest).FullName},{typeof(TResult).FullName}>");
            }

            var result = await ExecutionHelper.Execute(() => handler.Handle(context, request), handler, logger);

            await serviceProvider.FilterAsync(context, request, result, logger);

            return result;
        }
    }
}
