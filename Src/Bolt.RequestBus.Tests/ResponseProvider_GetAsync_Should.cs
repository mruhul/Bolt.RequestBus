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

        public ResponseProvider_GetAsync_Should(ServiceProviderFixture fixture)
        {
            _fixture = fixture;   
        }

        [Fact]
        public async Task Return_Correct_Response()
        {
            var sp = _fixture.BuildProvider(collection => {
                collection.AddTransient<IExecutionContextInitializerAsync, Customer1ExecutionContext>();
                collection.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });

            var response = sp.GetService<IResponseProvider>();
            var result = await response.GetAsync<Customer>();
            result.Name.ShouldBe("Customer1Filtered");
        }

        [Fact]
        public async Task Return_Pick_Correct_Handler()
        {
            var sp = _fixture.BuildProvider(collection => {
                collection.AddTransient<IExecutionContextInitializerAsync, Customer2ExecutionContext>();
                collection.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });

            var response = sp.GetService<IResponseProvider>();
            var result = await response.GetAsync<Customer>();
            result.Name.ShouldBe("Customer2Filtered");
        }

        [Fact]
        public async Task Throw_Exception_When_No_Handler_Available()
        {
            var sp = _fixture.BuildProvider(collection => {
                collection.AddTransient<IExecutionContextInitializerAsync, CustomerNoneExecutionContext>();
                collection.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });

            var response = sp.GetService<IResponseProvider>();
            await Should.ThrowAsync<RequestBusException>(response.GetAsync<Customer>());
        }

        [Fact]
        public async Task Not_Throw_Exception_When_No_Handler_Available()
        {
            var sp = _fixture.BuildProvider(collection => {
                collection.AddTransient<IExecutionContextInitializerAsync, CustomerNoneExecutionContext>();
                collection.AddTransient<IResponseFilterAsync<Customer>, CustomerFilter>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
                collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            });

            var response = sp.GetService<IResponseProvider>();
            var rslt = await response.TryGetAsync<Customer>();
            rslt.ShouldBeNull();
        }
    }

    public class Customer
    {
        public string Name { get; set; }
    }
    public class CustomerNoneExecutionContext : IExecutionContextInitializerAsync
    {
        public Task Init(IExecutionContextWriter writer)
        {
            writer.Write("handler", "");
            return Task.CompletedTask;
        }
    }

    public class Customer1ExecutionContext : IExecutionContextInitializerAsync
    {
        public Task Init(IExecutionContextWriter writer)
        {
            writer.Write("handler", "customer1");
            return Task.CompletedTask;
        }
    }
    public class Customer2ExecutionContext : IExecutionContextInitializerAsync
    {
        public Task Init(IExecutionContextWriter writer)
        {
            writer.Write("handler", "customer2");
            return Task.CompletedTask;
        }
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
            return context.Get<string>("handler").IsSame("customer1");
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
            return context.Get<string>("handler").IsSame("customer2");
        }
    }
}
