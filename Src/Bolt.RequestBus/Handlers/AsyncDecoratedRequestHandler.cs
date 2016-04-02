using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolt.RequestBus.Events;

namespace Bolt.RequestBus.Handlers
{
    internal class AsyncDecoratedRequestHandler<TRequest> 
        : IAsyncRequestHandler<TRequest> where TRequest : IRequest
    {
        private readonly IAsyncRequestHandler<TRequest> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IAsyncValidator<TRequest>> validators;
        private readonly IEnumerable<IAsyncRequestFilter<TRequest>> filters;

        public AsyncDecoratedRequestHandler(
            IAsyncRequestHandler<TRequest> innerHandler,
            IRequestBus bus,
            IEnumerable<IAsyncValidator<TRequest>> validators,
            IEnumerable<IAsyncRequestFilter<TRequest>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators ?? Enumerable.Empty<IAsyncValidator<TRequest>>();
            this.filters = filters ?? Enumerable.Empty<IAsyncRequestFilter<TRequest>>();
        }

        public async Task<IResponse> HandleAsync(TRequest msg)
        {
            await bus.PublishAsync(new RequestInitiated<TRequest>(msg));

            foreach (var filter in filters)
            {
                await filter.OnInitAsync(msg);
            }

            foreach (var validator in validators.OrderBy(x => x.Order))
            {
                var errors = (await validator.ValidateAsync(msg))?.ToArray();

                if (errors?.Length > 0)
                {
                    await bus.PublishAsync(new RequestValidationFailed<TRequest>(msg, errors));

                    return Response.Failed(errors);
                }
            }

            await bus.PublishAsync(new RequestValidated<TRequest>(msg));

            foreach (var filter in filters)
            {
                await filter.OnValidatedAsync(msg);
            }

            await innerHandler.HandleAsync(msg);

            await bus.PublishAsync(new RequestCompleted<TRequest>(msg));

            return Response.Succeed();
        }
    }

    internal class AsyncDecoratedRequestHandler<TRequest, TValue> 
        : IAsyncRequestHandler<TRequest, TValue> where TRequest : IRequest
    {
        private readonly IAsyncRequestHandler<TRequest, TValue> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IAsyncValidator<TRequest>> validators;
        private readonly IEnumerable<IAsyncRequestFilter<TRequest, TValue>> filters;

        public AsyncDecoratedRequestHandler(IAsyncRequestHandler<TRequest, TValue> innerHandler,
            IRequestBus bus,
            IEnumerable<IAsyncValidator<TRequest>> validators,
            IEnumerable<IAsyncRequestFilter<TRequest, TValue>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators ?? Enumerable.Empty<IAsyncValidator<TRequest>>();
            this.filters = filters ?? Enumerable.Empty<IAsyncRequestFilter<TRequest, TValue>>();
        }

        public async Task<IResponse<TValue>> HandleAsync(TRequest msg)
        {
            await bus.PublishAsync(new RequestInitiated<TRequest>(msg));

            foreach (var filter in filters)
            {
                await filter.OnInitAsync(msg);
            }

            foreach (var validator in validators.OrderBy(x => x.Order))
            {
                var errors = (await validator.ValidateAsync(msg))?.ToArray();

                if (errors?.Length > 0)
                {
                    await bus.PublishAsync(new RequestValidationFailed<TRequest>(msg, errors));

                    return Response.Failed<TValue>(errors);
                }
            }

            await bus.PublishAsync(new RequestValidated<TRequest>(msg));

            foreach (var filter in filters)
            {
                await filter.OnValidatedAsync(msg);
            }

            var response = await innerHandler.HandleAsync(msg);

            foreach (var filter in filters)
            {
                await filter.OnCompletedAsync(msg, response.Value);
            }

            await bus.PublishAsync(new RequestCompleted<TRequest, TValue>(msg, response.Value));

            return response;
        }
    }
}