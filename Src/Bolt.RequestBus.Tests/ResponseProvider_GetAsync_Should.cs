using Bolt.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Bolt.RequestBus.Tests
{

    public class ResponseProvider_GetAsync_Should : IClassFixture<ServiceProviderFixture>
    {
        private readonly ServiceProviderFixture _fixture;

        private IServiceProvider _serviceProvider;

        public ResponseProvider_GetAsync_Should(ServiceProviderFixture fixture)
        {
            _fixture = fixture;

            _serviceProvider = _fixture.BuildProvider(collection => {
                collection.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });
        }

        
        [Fact]
        public async Task Return_Correct_Response()
        {
            var response = _serviceProvider.GetService<IResponseProvider>();
            var result = await response.GetAsync<Customer>();
            result.Name.ShouldBe("Customer1Filtered");
        }
    }

    public class Customer
    {
        public string Name { get; set; }
    }

    public class CustomerFilter : ResponseFilterAsync<Customer>
    {
        public override Task Filter(IExecutionContext context, IResponse<Customer> response)
        {
            if(response.IsSucceed)
            {
                response.Result.Name = response.Result.Name + "Filtered";
            }

            return Task.FromResult(0);
        }
    }

    public class GetCustomerHandler : ResponseHandlerAsync<Customer>
    {
        protected override Task<Customer> Handle(IExecutionContext context)
        {
            return new Customer { Name = "Customer1" }.WrapInTask();
        }

        public override bool IsApplicable(IExecutionContext context)
        {
            return true;
        }
    }

    public class GetCustomer2Handler : ResponseHandlerAsync<Customer>
    {
        protected override Task<Customer> Handle(IExecutionContext context)
        {
            return new Customer { Name = "Customer2" }.WrapInTask();
        }
        public override bool IsApplicable(IExecutionContext context)
        {
            return false ;
        }
    }
}
