using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus.Tests
{
    public class ServiceProviderFixture : IDisposable
    {
        private IServiceCollection _sc;

        public ServiceProviderFixture()
        {
            _sc = new ServiceCollection();
            _sc.AddRequestBus();
            _sc.AddLogging(configure => configure.AddConsole());
        }

        public IServiceProvider BuildProvider(Action<IServiceCollection> action)
        {
            action.Invoke(_sc);

            return _sc.BuildServiceProvider();
        }

        public void Dispose()
        {
        }
    }
}
