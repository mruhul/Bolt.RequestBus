using System;
using System.Collections.Generic;
using System.Linq;
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
        private Task<IEnumerable<IResponseUnit<TestResponse>>> ExecuteResponses(string contextValue, TestRequest request)
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IExecutionContextPopulatorAsync>(c => new TestRequestExecutionContext(contextValue));
                sc.AddTransient<IValidatorAsync<TestRequest>, TestRequestValidator>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestMainHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestFailedMainHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestInDependentHandler>();
                sc.AddTransient<IResponseHandlerAsync<TestRequest, TestResponse>, TestRequestDependentHandler>();
            });
            
            var sut = sp.GetService<IRequestBus>();

            return sut.ResponsesAsync<TestRequest, TestResponse>(request);
        }

        [Fact]
        public async Task Return_Only_IndependentAndMain_Response_When_Main_Failed()
        {
            var responses = await ExecuteResponses("MainFailed", new TestRequest { Id = "1" });

            responses.Count().ShouldBe(2);
            responses.ShouldContain(x => x.IsMainResponse);
            responses.ShouldContain(x => x.Result != null && x.Result.Name.IsSame("Independent"));
        }

        [Fact]
        public async Task Return_MainResponse_As_Failed_With_Errors_When_Validation_Failed()
        {   
            var responses = await ExecuteResponses("ValidationFailed", new TestRequest
            {
                Id = string.Empty
            });

            responses.ShouldNotContain(x => !x.IsMainResponse);
            responses.ShouldContain(x => x.IsMainResponse);
            responses.ShouldContain(x => x.Errors.Any(e => e.Message.IsSame("Id is required")));
        }

        [Fact]
        public async Task Return_All_Responses()
        {
            var responses = await ExecuteResponses("A", new TestRequest
            {
                Id = "1"
            });

            responses.ShouldNotBeNull();
            responses.ShouldContain(x => x.IsMainResponse);
            responses.ShouldContain(x => !x.IsMainResponse);
            responses.ShouldContain(x => x.Result.Name.IsSame("Main"));
            responses.ShouldContain(x => x.Result.Name.IsSame("Independent"));
            responses.ShouldContain(x => x.Result.Name.IsSame("Dependent"));
        }


        public class TestRequestValidator : ValidatorAsync<TestRequest>
        {
            public override Task<IEnumerable<IError>> Validate(IExecutionContextReader context, TestRequest request)
            {
                return RuleChecker.For(request)
                        .Should(x => x.Id.HasValue(),"Id","Id is required")
                        .Done()
                        .WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                return context.Get<string>("control").IsSame("ValidationFailed");
            }
        }

        public class TestRequestMainHandler : MainResponseHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IExecutionContextReader context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Main"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                var control = context.Get<string>("control");
                return control.IsSame("a");
            }
        }

        public class TestRequestFailedMainHandler : IResponseHandlerAsync<TestRequest, TestResponse>
        {
            public ExecutionHintType ExecutionHint => ExecutionHintType.Main;

            public Task<IResponse<TestResponse>> Handle(IExecutionContextReader context, TestRequest request)
            {
                return Response.Failed<TestResponse>().WrapInTask();
            }

            public bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                return context.Get<string>("control").IsSame("mainfailed");
            }
        }

        public class TestRequestInDependentHandler : ResponseHandlerAsync<TestRequest, TestResponse>
        {
            protected override Task<TestResponse> Handle(IExecutionContextReader context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Independent"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                var control = context.Get<string>("control");
                return control.IsSame("a") || control.IsSame("mainfailed");
            }
        }

        public class TestRequestDependentHandler : ResponseHandlerAsync<TestRequest, TestResponse>
        {
            public override ExecutionHintType ExecutionHint => ExecutionHintType.Dependent;
            
            protected override Task<TestResponse> Handle(IExecutionContextReader context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Dependent"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                var control = context.Get<string>("control");
                return control.IsSame("a") || control.IsSame("mainfailed");
            }
        }

        public class TestRequestDependentBHandler : ResponseHandlerAsync<TestRequest, TestResponse>
        {
            public override ExecutionHintType ExecutionHint => ExecutionHintType.Dependent;

            protected override Task<TestResponse> Handle(IExecutionContextReader context, TestRequest request)
            {
                return new TestResponse
                {
                    Name = "Dependent"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestRequest request)
            {
                return context.Get<string>("control").IsSame("a");
            }
        }

        public class TestRequestExecutionContext : IExecutionContextPopulatorAsync
        {
            private readonly string _value;

            public TestRequestExecutionContext(string value)
            {
                _value = value;
            }

            public Task Init(IExecutionContextWriter writer)
            {
                writer.Write("control", _value);
                return Task.CompletedTask;
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
