using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bolt.RequestBus
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRequestBus(this IServiceCollection source)
        {
            source.TryAdd(ServiceDescriptor.Transient<IRequestBus, RequestBus>());
            source.TryAdd(ServiceDescriptor.Transient<IResponseProvider, ResponseProvider>());
        }
    }
}
