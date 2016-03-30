using System.Collections.Generic;
using Bolt.RequestBus.Handlers;

namespace Bolt.RequestBus.Factories
{
    public class RequestHandlerFactory<TRequest, TReturn> : IRequestHandlerFactory<TRequest, TReturn> where TRequest : IRequest
    {
        private readonly IRequestHandler<TRequest, TReturn> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly IEnumerable<IRequestFilter<TRequest, TReturn>> filters;

        public RequestHandlerFactory(IRequestHandler<TRequest,TReturn> innerHandler,
            IRequestBus bus,
            IEnumerable<IValidator<TRequest>> validators,
            IEnumerable<IRequestFilter<TRequest,TReturn>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators;
            this.filters = filters;
        }

        public IRequestHandler<TRequest, TReturn> Create()
        {
            return new DecoratedRequestHandler<TRequest, TReturn>(innerHandler, bus, validators,filters);
        }
    }

    public class RequestHandlerFactory<TRequest> : IRequestHandlerFactory<TRequest> where TRequest : IRequest
    {
        private readonly IRequestHandler<TRequest> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly IEnumerable<IRequestFilter<TRequest>> filters;

        public RequestHandlerFactory(IRequestHandler<TRequest> innerHandler,
            IRequestBus bus,
            IEnumerable<IValidator<TRequest>> validators,
            IEnumerable<IRequestFilter<TRequest>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators;
            this.filters = filters;
        }

        public IRequestHandler<TRequest> Create()
        {
            return new DecoratedRequestHandler<TRequest>(innerHandler, bus, validators, filters);
        }
    }
}