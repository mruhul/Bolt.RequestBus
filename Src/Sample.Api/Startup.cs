using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Bolt.Common.Extensions;
using Bolt.Logger;
using Microsoft.Owin;
using Owin;
using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.PersistentStores;

[assembly: OwinStartup(typeof(Sample.Api.Startup))]

namespace Sample.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyModules(typeof(Startup).Assembly, 
                typeof(IPersistentStore).Assembly,
                typeof(Bolt.RequestBus.Autofac.DependencyResolver).Assembly);

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            RunStartUpTask(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private void RunStartUpTask(IContainer container)
        {
            container.Resolve<IEnumerable<IStartUpTask>>()
                .NullSafe()
                .ForEach(task =>
                {
                    try
                    {
                        task.Run();
                    }
                    catch (Exception e)
                    {
                        container.Resolve<ILogger>().Error(e,e.Message);
                    }
                });
        }
    }
}
