using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus.Impl
{
    public class RequestBus : IRequestBus
    {
        private readonly IDependencyResolver resolver;

        public RequestBus(IDependencyResolver resolver)
        {
            this.resolver = resolver;
        }

        IResponse<T> IRequestBus.Send<TRequest, T>(TRequest request)
        {
            var factory = resolver.Resolve<IRequestHandlerFactory<TRequest, T>>();

            return factory.Create().Handle(request);
        }

        IResponse IRequestBus.Send<TRequest>(TRequest request)
        {
            var factory = resolver.Resolve<IRequestHandlerFactory<TRequest>>();

            return factory.Create().Handle(request);
        }

        Task<IResponse<T>> IRequestBus.SendAsync<TRequest, T>(TRequest request)
        {
            var factory = resolver.Resolve<IAsyncRequestHandlerFactory<TRequest,T>>();

            return factory.Create().HandleAsync(request);
        }

        Task<IResponse> IRequestBus.SendAsync<TRequest>(TRequest request)
        {
            var factory = resolver.Resolve<IAsyncRequestHandlerFactory<TRequest>>();

            return factory.Create().HandleAsync(request);
        }

        void IRequestBus.Publish<TEvent>(TEvent evnt)
        {
            var handlers = resolver.ResolveAll<IEventHandler<TEvent>>();

            if(handlers == null) return;

            foreach (var eventHandler in handlers)
            {
                eventHandler.Handle(evnt);
            }
        }

        Task IRequestBus.PublishAsync<TEvent>(TEvent evnt)
        {
            var handlers = resolver.ResolveAll<IAsyncEventHandler<TEvent>>()?.ToArray();

            if (handlers == null || handlers.Length == 0) return Task.FromResult(0);

            if (handlers.Length == 1) return handlers[0].HandleAsync(evnt);

            var tasks = new Task[handlers.Length];

            for (var i = 0; i < handlers.Length; i++)
            {
                tasks[i] = handlers[i].HandleAsync(evnt);
            }

            return Task.WhenAll(tasks);
        }
    }
}