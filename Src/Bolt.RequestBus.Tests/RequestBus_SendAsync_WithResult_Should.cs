using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using System.Linq;
using Bolt.Common.Extensions;

namespace Bolt.RequestBus.Tests
{
    public class RequestBus_SendAsync_WithResult_Should
    {
        [Fact]
        public async Task Throw_Exception_When_No_Handler_Available()
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IRequestHandlerAsync<TestQuery, TestResult>, TestQueryHandlerFirst>();
            });

            var sut = sp.GetService<IRequestBus>();
            await Should.ThrowAsync<RequestBusException>(async () => await sut.SendAsync<TestQuery, TestResult>(new TestQuery()));
        }


        [Fact]
        public async Task Throw_No_Exception_When_No_Handler_Available_And_UseTrySend_Instead()
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IRequestHandlerAsync<TestQuery, TestResult>, TestQueryHandlerFirst>();
            });

            var sut = sp.GetService<IRequestBus>();

            var response = await sut.TrySendAsync<TestQuery, TestResult>(new TestQuery());

            response.IsSucceed.ShouldBeFalse();
            response.Errors.Count().ShouldBe(1);
            response.IsNoHandlerAvailable().ShouldBeTrue();
        }

        [Fact]
        public async Task Return_Failed_Response_With_Errors_When_Input_Is_Wrong()
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IExecutionContextPopulatorAsync, ExecutionContextSecond>();
                sc.AddTransient<IValidatorAsync<TestQuery>, TestQueryInputValidatorFirst>();
                sc.AddTransient<IValidatorAsync<TestQuery>, TestQueryInputValidatorSecond>();
            });

            var sut = sp.GetService<IRequestBus>();
            var response = await sut.SendAsync<TestQuery, TestResult>(new TestQuery());

            response.IsSucceed.ShouldBeFalse();
            response.Errors.ShouldNotBeEmpty();
            response.Errors.Count().ShouldBe(2);
            response.Errors.ShouldContain(e => e.Code.IsSame("TESTQUERY_SECOND_ID_REQUIRED"));
            response.Errors.ShouldContain(e => e.Code.IsSame("TESTQUERY_SECOND_NAME_REQUIRED"));
        }

        [Fact]
        public async Task Return_Correct_Response()
        {
            var sp = ServiceProviderBuilder.Build(sc => {
                sc.AddTransient<IExecutionContextPopulatorAsync, ExecutionContextSecond>();
                sc.AddTransient<IValidatorAsync<TestQuery>, TestQueryInputValidatorFirst>();
                sc.AddTransient<IValidatorAsync<TestQuery>, TestQueryInputValidatorSecond>();
                sc.AddTransient<IRequestHandlerAsync<TestQuery, TestResult>, TestQueryHandlerFirst>();
                sc.AddTransient<IRequestHandlerAsync<TestQuery, TestResult>, TestQueryHandlerSecond>();
            });

            var sut = sp.GetService<IRequestBus>();
            var response = await sut.SendAsync<TestQuery, TestResult>(new TestQuery {
                Id = "1",
                Name = "Name"
            });

            response.IsSucceed.ShouldBeTrue();
            response.Result.ShouldNotBeNull();
            response.Result.Desc.ShouldBe("NameFromSecondHandler");
        }

        public class TestQuery
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class TestResult
        {
            public string Desc { get; set; }
        }

        public class TestQueryInputValidatorFirst : ValidatorAsync<TestQuery>
        {
            public override Task<IEnumerable<IError>> Validate(IExecutionContextReader context, TestQuery request)
            {
                return RuleChecker
                        .For(request)
                        .Should(r => !string.IsNullOrWhiteSpace(r.Id), "Id", "Id is required", "TESTQUERY_ID_REQUIRED")
                        .Should(r => !string.IsNullOrWhiteSpace(r.Name), "Name", "Name is required", "TESTQUERY_NAME_REQUIRED")
                        .Done()
                        .WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestQuery request)
            {
                return context.Get<string>("handler").IsSame("first");
            }
        }

        public class TestQueryInputValidatorSecond : ValidatorAsync<TestQuery>
        {
            public override Task<IEnumerable<IError>> Validate(IExecutionContextReader context, TestQuery request)
            {
                return RuleChecker
                        .For(request)
                        .Should(r => !string.IsNullOrWhiteSpace(r.Id), "Id", "Id is required", "TESTQUERY_SECOND_ID_REQUIRED")
                        .Should(r => !string.IsNullOrWhiteSpace(r.Name), "Name", "Name is required", "TESTQUERY_SECOND_NAME_REQUIRED")
                        .Done()
                        .WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestQuery request)
            {
                return context.Get<string>("handler").IsSame("second");
            }
        }

        public class TestQueryHandlerFirst : RequestHandlerAsync<TestQuery, TestResult>
        {
            protected override Task<TestResult> Handle(IExecutionContextReader context, TestQuery request)
            {
                return new TestResult {
                    Desc = $"{request.Name}FromFirstHandler"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestQuery request)
            {
                return context.Get<string>("handler").IsSame("first");
            }
        }



        public class TestQueryHandlerSecond : RequestHandlerAsync<TestQuery, TestResult>
        {
            protected override Task<TestResult> Handle(IExecutionContextReader context, TestQuery request)
            {
                return new TestResult
                {
                    Desc = $"{request.Name}FromSecondHandler"
                }.WrapInTask();
            }

            public override bool IsApplicable(IExecutionContextReader context, TestQuery request)
            {
                return context.Get<string>("handler").IsSame("second");
            }
        }

        public class ExecutionContextFirst : IExecutionContextPopulatorAsync
        {
            public Task Init(IExecutionContextWriter writer)
            {
                writer.Write("handler", "first");
                return Task.CompletedTask;
            }
        }
        public class ExecutionContextSecond : IExecutionContextPopulatorAsync
        {
            public Task Init(IExecutionContextWriter writer)
            {
                writer.Write("handler", "second");
                return Task.CompletedTask;
            }
        }
    }
}
