﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus
{
    public interface IRequestHandlerAsync<TRequest> : IApplicable<TRequest>
    {
        Task<IResponse> Handle(IExecutionContextReader context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest> : IRequestHandlerAsync<TRequest>
    {
        async Task<IResponse> IRequestHandlerAsync<TRequest>.Handle(IExecutionContextReader context, TRequest request)
        {
            await Handle(context, request);
            return Response.Succeed();
        }

        protected abstract Task Handle(IExecutionContextReader context, TRequest request);

        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;
    }

    public interface IRequestHandlerAsync<TRequest,TResult> : IApplicable<TRequest>
    {
        Task<IResponse<TResult>> Handle(IExecutionContextReader context, TRequest request);
    }

    public abstract class RequestHandlerAsync<TRequest, TResult> : IRequestHandlerAsync<TRequest, TResult>
    {
        protected abstract Task<TResult> Handle(IExecutionContextReader context, TRequest request);

        public virtual bool IsApplicable(IExecutionContextReader context, TRequest request) => true;

        async Task<IResponse<TResult>> IRequestHandlerAsync<TRequest, TResult>.Handle(IExecutionContextReader context, TRequest request)
        {
            var result = await Handle(context, request);

            return Response.Succeed(result);
        }
    }


    internal class RequestHandlerBus
    {
        internal static async Task<IResponse> SendAsync<TRequest>(IServiceProvider serviceProvider, IExecutionContextReader context, ILogger logger, TRequest request, bool failSafe)
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

            var result = await handler.Handle(context, request);

            return result;
        }

        internal static async Task<IResponse<TResult>> SendAsync<TRequest,TResult>(IServiceProvider serviceProvider, IExecutionContextReader context, ILogger logger, TRequest request, bool failSafe)
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

            var result = await handler.Handle(context, request);

            await serviceProvider.FilterAsync(context, request, result, logger);

            return result;
        }
    }
}
