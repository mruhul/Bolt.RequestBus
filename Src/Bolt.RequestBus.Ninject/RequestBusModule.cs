using Bolt.RequestBus.Factories;
using Ninject.Modules;

namespace Bolt.RequestBus.Ninject
{
    public class RequestBusModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IDependencyResolver>().To<DependencyResolver>();
            Kernel.Bind<IRequestBus>().To<Impl.RequestBus>();
            Kernel.Bind(typeof (IRequestHandlerFactory<,>)).To(typeof (RequestHandlerFactory<,>));
            Kernel.Bind(typeof (IAsyncRequestHandlerFactory<,>)).To(typeof (AsyncRequestHandlerFactory<,>));
            Kernel.Bind(typeof(IRequestHandlerFactory<>)).To(typeof(RequestHandlerFactory<>));
            Kernel.Bind(typeof(IAsyncRequestHandlerFactory<>)).To(typeof(AsyncRequestHandlerFactory<>));
        }
    }
}
