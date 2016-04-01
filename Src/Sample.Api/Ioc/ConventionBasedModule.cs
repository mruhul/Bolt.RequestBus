using Autofac;
using Bolt.Logger;
using Bolt.RequestBus;
using Sample.Features.Shared;

namespace Sample.Api.Ioc
{
    public class ConventionBasedModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[]
            {typeof (Startup).Assembly, typeof (Sample.Infrastructure.PersistentStores.IPersistentStore).Assembly};

            builder.RegisterAssemblyTypes(assemblies)
                .Where(a => a.IsClass && 
                    (a.Name.EndsWith("Handler") 
                    || a.Name.EndsWith("Filter")
                    || a.Name.EndsWith("Proxy")
                    || a.Name.EndsWith("Validator")
                    || a.Name.EndsWith("Store")))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof (EventSourceHandler<>)).As(typeof (IEventHandler<>));
        }
    }

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => Bolt.Logger.NLog.LoggerFactory.Create(typeof (Startup)))
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<Bolt.Serializer.Json.JsonSerializer>()
                .As<Bolt.Serializer.ISerializer>()
                .SingleInstance();
        }
    }
}