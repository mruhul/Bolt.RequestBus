using Autofac;
using AutoMapper;
using Bolt.Logger;

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
        }
    }
}