using Bolt.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Bolt.RequestBus.Tests
{
    public class ResponseProvider_TryGetAsync_Should
    {
        [Fact]
        public async Task Not_Throw_Exception_When_No_Handler_Available()
        {
            var sp = ServiceProviderBuilder.Build(c => {
                c.AddTransient<IExecutionContextPopulatorAsync, CustomerNoneExecutionContext>();
                c.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                c.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                c.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });

            var response = sp.GetService<IResponseProvider>();
            var rslt = await response.TryResponseAsync<Customer>();
            rslt.ShouldBeNull();
        }
    }
}
