using Autofac;

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
                    || a.Name.EndsWith("Store")))
                .AsImplementedInterfaces();
        }
    }
}