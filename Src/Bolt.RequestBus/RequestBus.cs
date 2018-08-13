using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IRequestBus
    {
        Task<IResponse> SendAsync<TRequest>(TRequest request);
        Task<IResponse> SendAsync<TRequest>(IExecutionContextReader context, TRequest request);
        Task<IResponse> TrySendAsync<TRequest>(TRequest request);
        Task<IResponse> TrySendAsync<TRequest>(IExecutionContextReader context, TRequest request);
        Task<IResponse<TResult>> SendAsync<TRequest,TResult>(TRequest request);
        Task<IResponse<TResult>> SendAsync<TRequest, TResult>(IExecutionContextReader context, TRequest request);
        Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(TRequest request);
        Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(IExecutionContextReader context, TRequest request);
        Task PublishAsync<TEvent>(TEvent evnt);
        Task TryPublishAsync<TEvent>(TEvent evnt);
        Task PublishAsync<TEvent>(IExecutionContextReader context, TEvent evnt);
        Task TryPublishAsync<TEvent>(IExecutionContextReader context, TEvent evnt);
    }

    public class RequestBusSettings
    {
        public bool EnableProfiler { get; set; }
    }

    public class RequestBus : IRequestBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RequestBus> _logger;

        public RequestBus(IServiceProvider serviceProvider, ILogger<RequestBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task PublishAsync<TEvent>(TEvent evnt)
        {
            return _serviceProvider
                    .BuildContextAsync()
                    .ContinueWith(x => EventHandlerBus
                                        .PublishAsync(_serviceProvider, x.Result, _logger, evnt, false));
        }

        public Task TryPublishAsync<TEvent>(TEvent evnt)
        {
            return _serviceProvider
                    .BuildContextAsync()
                    .ContinueWith(x => EventHandlerBus
                                        .PublishAsync(_serviceProvider, x.Result, _logger, evnt, failSafe: true));
        }



        public Task PublishAsync<TEvent>(IExecutionContextReader context, TEvent evnt)
        {
            return EventHandlerBus.PublishAsync(_serviceProvider, context, _logger, evnt, failSafe: false);
        }

        public Task TryPublishAsync<TEvent>(IExecutionContextReader context, TEvent evnt)
        {
            return EventHandlerBus.PublishAsync(_serviceProvider, context, _logger, evnt, failSafe: true);
        }

        public async Task<IResponse> SendAsync<TRequest>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await SendAsync(context, request);
        }
        
        public Task<IResponse> SendAsync<TRequest>(IExecutionContextReader context, TRequest request)
        {
            return RequestHandlerBus.SendAsync(_serviceProvider, context, _logger, request, failSafe: false);
        }

        public async Task<IResponse<TResult>> SendAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await SendAsync<TRequest,TResult>(context, request);
        }
        
        public Task<IResponse<TResult>> SendAsync<TRequest, TResult>(IExecutionContextReader context, TRequest request)
        {
            return RequestHandlerBus.SendAsync<TRequest, TResult>(_serviceProvider, context, _logger, request, failSafe: false);
        }

        public async Task<IResponse> TrySendAsync<TRequest>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await TrySendAsync(context, request);
        }

        public Task<IResponse> TrySendAsync<TRequest>(IExecutionContextReader context, TRequest request)
        {
            return RequestHandlerBus.SendAsync(_serviceProvider, context, _logger, request, failSafe: true);
        }

        public async Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await TrySendAsync<TRequest, TResult>(context, request);
        }

        public Task<IResponse<TResult>> TrySendAsync<TRequest, TResult>(IExecutionContextReader context, TRequest request)
        {
            return RequestHandlerBus.SendAsync<TRequest, TResult>(_serviceProvider, context, _logger, request, failSafe: true);
        }
    }

    public static class ResponseExtensions
    {
        public static bool IsNoHandlerAvailable(this IResponse source)
        {
            return source.IsSucceed == false && source.Errors.Any(x => x.Code == "NO_HANDLER");
        }
    }

    public interface IResponse
    {
        bool IsSucceed { get; }
        IEnumerable<IError> Errors { get; }
    }

    public interface IResponse<T> : IResponse
    {
        T Result { get; }
    }

    public class Response : IResponse
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }

        public static IResponse Succeed() => new Response { IsSucceed = true };
        public static IResponse Failed() => new Response { IsSucceed = false };
        public static IResponse Failed(IEnumerable<IError> errors) => new Response { IsSucceed = false, Errors = errors };

        public static IResponse<TResult> Succeed<TResult>(TResult result) => new Response<TResult> { IsSucceed = true, Result = result };
        public static IResponse<TResult> Failed<TResult>(IEnumerable<IError> errors) => new Response<TResult> { IsSucceed = false, Errors = errors ?? Enumerable.Empty<IError>() };
        public static IResponse<TResult> Failed<TResult>() => new Response<TResult> { IsSucceed = false, Errors = Enumerable.Empty<IError>() };

        internal static IResponse<TResult> NoHandler<TResult>() => new Response<TResult>() { IsSucceed = false, Errors = new IError[] { Error.Create(string.Empty, "Unable to handle request", "NO_HANDLER") } };
        internal static IResponse NoHandler() => new Response()
        {
            IsSucceed = false,
            Errors = new IError[] { Error.Create(string.Empty, "Unable to handle request", "NO_HANDLER") }
        };
    }

    public class Response<T> : IResponse<T>
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public T Result { get; set; }
    }
    
    public interface IResponseUnit<TResult> : IResponse
    {
        TResult Result { get; }
        bool IsMainResponse { get; }
    }

    public class ResponseUnit<TResult> : IResponseUnit<TResult>
    {
        public bool IsSucceed { get; set; }
        public IEnumerable<IError> Errors { get; set; }
        public TResult Result { get; set; }
        public bool IsMainResponse { get; set; }

        public static IResponseUnit<TResult> Succeed(TResult result) => new ResponseUnit<TResult> { IsSucceed = true, Result = result };
        public static IResponseUnit<TResult> Failed(IEnumerable<IError> errors) => new ResponseUnit<TResult> { IsSucceed = false, Errors = errors ?? Enumerable.Empty<IError>() };
        public static IResponseUnit<TResult> Failed() => new ResponseUnit<TResult> { IsSucceed = false, Errors = Enumerable.Empty<IError>() };
    }

    public enum ExecutionHintType
    {
        Independent,
        Main,
        Dependent
    }
}
