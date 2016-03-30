using Autofac;
using Bolt.RequestBus.Factories;

namespace Bolt.RequestBus.Autofac
{
    public class RequestBusModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DependencyResolver>().As<IDependencyResolver>();
            builder.RegisterType<Impl.RequestBus>().As<IRequestBus>();

            builder.RegisterGeneric(typeof(RequestHandlerFactory<,>)).As(typeof(IRequestHandlerFactory<,>));
            builder.RegisterGeneric(typeof(AsyncRequestHandlerFactory<,>)).As(typeof(IAsyncRequestHandlerFactory<,>));
            builder.RegisterGeneric(typeof(RequestHandlerFactory<>)).As(typeof(IRequestHandlerFactory<>));
            builder.RegisterGeneric(typeof(AsyncRequestHandlerFactory<>)).As(typeof(IAsyncRequestHandlerFactory<>));
        }
    }
}