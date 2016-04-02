using Autofac;
using Bolt.RequestBus;
using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Ioc
{
    public class ConventionBasedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[]
            {typeof (Startup).Assembly, typeof (IPersistentStore).Assembly};

            builder.RegisterAssemblyTypes(assemblies)
                .Where(a => a.IsClass && 
                    (a.Name.EndsWith("Handler") 
                    || a.Name.EndsWith("Filter")
                    || a.Name.EndsWith("Proxy")
                    || a.Name.EndsWith("Validator")
                    || a.Name.EndsWith("Repository")
                    || a.Name.EndsWith("Store")))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IStartUpTask>()
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof (Features.Shared.EventSourceHandler<>)).As(typeof (IEventHandler<>));
        }
    }
}