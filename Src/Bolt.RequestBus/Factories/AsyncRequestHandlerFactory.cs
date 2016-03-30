using System.Collections.Generic;
using Bolt.RequestBus.Handlers;

namespace Bolt.RequestBus.Factories
{
    public class AsyncRequestHandlerFactory<TRequest> 
        : IAsyncRequestHandlerFactory<TRequest> where TRequest : IRequest
    {
        private readonly IAsyncRequestHandler<TRequest> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IAsyncValidator<TRequest>> validators;
        private readonly IEnumerable<IAsyncRequestFilter<TRequest>> filters;

        public AsyncRequestHandlerFactory(
            IAsyncRequestHandler<TRequest> innerHandler,
            IRequestBus bus,
            IEnumerable<IAsyncValidator<TRequest>> validators,
            IEnumerable<IAsyncRequestFilter<TRequest>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators;
            this.filters = filters;
        }

        public IAsyncRequestHandler<TRequest> Create()
        {
            return new AsyncDecoratedRequestHandler<TRequest>(innerHandler, bus, validators, filters);
        }
    }

    public class AsyncRequestHandlerFactory<TRequest, TValue> 
        : IAsyncRequestHandlerFactory<TRequest, TValue> where TRequest : IRequest
    {
        private readonly IAsyncRequestHandler<TRequest, TValue> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IAsyncValidator<TRequest>> validators;
        private readonly IEnumerable<IAsyncRequestFilter<TRequest, TValue>> filters;

        public AsyncRequestHandlerFactory(
            IAsyncRequestHandler<TRequest, TValue> innerHandler,
            IRequestBus bus,
            IEnumerable<IAsyncValidator<TRequest>> validators,
            IEnumerable<IAsyncRequestFilter<TRequest, TValue>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators;
            this.filters = filters;
        }

        public IAsyncRequestHandler<TRequest, TValue> Create()
        {
            return new AsyncDecoratedRequestHandler<TRequest, TValue>(innerHandler, bus, validators, filters);
        }
    }
}
