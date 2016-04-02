using Autofac;
using AutoMapper;
using Bolt.Logger;
using Sample.Api.Features.Shared;
using Sample.Api.Infrastructure.ContextStores;
using Sample.Api.Infrastructure.Mappers;
using Sample.Api.Infrastructure.RequestWrappers;

namespace Sample.Api.Ioc
{
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

            builder.RegisterType<AutoMappedMapper>().As<Infrastructure.Mappers.IMapper>().SingleInstance();
            builder.RegisterType<HttpContextStore>().As<IContextStore>().SingleInstance();
            builder.RegisterType<HttpRequestContext>().As<IRequestContext>().SingleInstance();
            builder.Register(x => new Settings()).As<ISettings>().SingleInstance();
        }
    }
}