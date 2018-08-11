using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bolt.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Bolt.RequestBus.Tests
{
    public class ResponseProviver_Responses_With_Request_Should
    {
        [Fact]
        public async Task Return_All_Responses()
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestMainHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestInDependentHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestDependentHandler>();
            });

            var sut = sp.GetService<IResponseProvider>();

            var response = await sut.ResponsesAsync<TestRequest, TestResponse>(new TestRequest {
                Id = "1"
            });

            response.ShouldNotBeNull();
            response.ShouldContain(x => x.IsMainResponse);
            response.ShouldContain(x => !x.IsMainResponse);
            response.ShouldContain(x => x.Result.Name.IsSame("Main"));
            response.ShouldContain(x => x.Result.Name.IsSame("Independent"));
            response.ShouldContain(x => x.Result.Name.IsSame("Dependent"));
        }


        public class TestRequestMainHandler : MainResponseHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IExecutionContext context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Main"
                }.WrapInTask();
            }
        }

        public class TestRequestInDependentHandler : ResponseHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IExecutionContext context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Independent"
                }.WrapInTask();
            }
        }

        public class TestRequestDependentHandler : ResponseHandlerAsync<TestRequest, TestResponse>
        {
            public override ExecutionHintType ExecutionHint => ExecutionHintType.Dependent;
            
            protected override Task<TestResponse> Handle(IExecutionContext context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Dependent"
                }.WrapInTask();
            }
        }

        public class TestRequest
        {
            public string Id { get; set; }
        }

        public class TestResponse
        {
            public string Name { get; set; }
        }
    }
}
