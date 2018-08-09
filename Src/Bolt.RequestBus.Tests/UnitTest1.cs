using Bolt.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace Bolt.RequestBus.Tests
{
    public class ResponseProvider_GetAsync_Should
    {
        private IServiceProvider sp;

        public ResponseProvider_GetAsync_Should()
        {
            var collection = new ServiceCollection();
            collection.AddRequestBus();
            collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomerHandler>();
            collection.AddTransient<IResponseHandlerAsync<Customer>, GetCustomer2Handler>();
            sp = collection.BuildServiceProvider();
        }

        [Fact]
        public async Task Test1()
        {
            var response = sp.GetService<IResponseProvider>();
            var result = await response.GetAsync<Customer>();
            result.Name.ShouldBe("Customer1");
        }
    }

    public class Customer
    {
        public string Name { get; set; }
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
