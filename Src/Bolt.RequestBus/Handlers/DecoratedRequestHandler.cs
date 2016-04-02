using System.Collections.Generic;
using System.Linq;
using Bolt.RequestBus.Events;

namespace Bolt.RequestBus.Handlers
{
    internal class DecoratedRequestHandler<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
    {
        private readonly IRequestHandler<TRequest> innerHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly IEnumerable<IRequestFilter<TRequest>> filters;
        public DecoratedRequestHandler(IRequestHandler<TRequest> innerHandler,
            IRequestBus bus,
            IEnumerable<IValidator<TRequest>> validators,
            IEnumerable<IRequestFilter<TRequest>> filters)
        {
            this.innerHandler = innerHandler;
            this.bus = bus;
            this.validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
            this.filters = filters ?? Enumerable.Empty<IRequestFilter<TRequest>>();
        }

        public IResponse Handle(TRequest msg)
        {
            bus.Publish(new RequestInitiated<TRequest>(msg));

            foreach (var filter in filters)
            {
                filter.OnInit(msg);
            }

            var hasValidator = false;

            foreach (var validator in validators.OrderBy(x => x.Order))
            {
                hasValidator = true;

                var errors = validator.Validate(msg)?.ToArray();

                if (errors?.Length > 0)
                {
                    bus.Publish(new RequestValidationFailed<TRequest>(msg, errors));

                    return Response.Failed(errors);
                }
            }

            if (hasValidator)
            {
                bus.Publish(new RequestValidated<TRequest>(msg));
            }

            foreach (var filter in filters)
            {
                filter.OnValidated(msg);
            }

            innerHandler.Handle(msg);

            bus.Publish(new RequestCompleted<TRequest>(msg));

            return Response.Succeed();
        }
    }

    internal class DecoratedRequestHandler<TRequest, TValue> : IRequestHandler<TRequest, TValue>
        where TRequest : IRequest
    {
        private readonly IRequestHandler<TRequest, TValue> internalHandler;
        private readonly IRequestBus bus;
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly IEnumerable<IRequestFilter<TRequest, TValue>> filters;

        public DecoratedRequestHandler(IRequestHandler<TRequest, TValue> internalHandler, IRequestBus bus, IEnumerable<IValidator<TRequest>> validators, IEnumerable<IRequestFilter<TRequest, TValue>> filters)
        {
            this.internalHandler = internalHandler;
            this.bus = bus;
            this.validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
            this.filters = filters ?? Enumerable.Empty<IRequestFilter<TRequest, TValue>>();
        }

        public IResponse<TValue> Handle(TRequest msg)
        {
            bus.Publish(new RequestInitiated<TRequest>(msg));

            foreach (var filter in filters)
            {
                filter.OnInit(msg);
            }

            var hasValidator = false;

            foreach (var validator in validators.OrderBy(x =>  x.Order))
            {
                hasValidator = true;

                var errors = validator.Validate(msg)?.ToArray();

                if (errors?.Length > 0)
                {
                    bus.Publish(new RequestValidationFailed<TRequest>(msg, errors));

                    return Response.Failed<TValue>(errors);
                }
            }

            if (hasValidator)
            {
                bus.Publish(new RequestValidated<TRequest>(msg));
            }

            foreach (var filter in filters)
            {
                filter.OnValidated(msg);
            }

            var response = internalHandler.Handle(msg);
            
            foreach (var filter in filters)
            {
                filter.OnCompleted(msg, response.Value);
            }

            bus.Publish(new RequestCompleted<TRequest,TValue>(msg, response.Value));

            return response;
        }
    }
}
