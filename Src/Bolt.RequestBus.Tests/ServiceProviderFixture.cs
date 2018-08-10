using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus.Tests
{
    public class ServiceProviderFixture
    {        
        public IServiceProvider BuildProvider(Action<IServiceCollection> action)
        {
            var sc = new ServiceCollection();
            sc.AddRequestBus();
            sc.AddLogging(configure => configure.AddConsole());

            action.Invoke(sc);

            return sc.BuildServiceProvider();
        }
    }
}
