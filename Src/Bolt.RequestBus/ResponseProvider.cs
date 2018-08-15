using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseProvider
    {
        Task<TResult> ResponseAsync<TResult>();
        Task<TResult> ResponseAsync<TResult>(IExecutionContextReader context);
        Task<TResult> TryResponseAsync<TResult>();
        Task<TResult> TryResponseAsync<TResult>(IExecutionContextReader context);
        Task<IEnumerable<IResponseUnit<TResult>>> ResponsesAsync<TResult>();
        Task<IEnumerable<IResponseUnit<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request);
    }

    public class ResponseProvider : IResponseProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IExecutionContextProvider _contextProvider;
        private readonly ILogger<ResponseProvider> _logger;

        public ResponseProvider(IServiceProvider serviceProvider, IExecutionContextProvider contextProvider, ILogger<ResponseProvider> logger)
        {
            _serviceProvider = serviceProvider;
            _contextProvider = contextProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> ResponsesAsync<TResult>()
        {
            var context = await _contextProvider.GetAsync(_serviceProvider);
            return await ResponseProviderBus.GetAllAsync<TResult>(_serviceProvider, context, _logger);
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> ResponsesAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _contextProvider.GetAsync(_serviceProvider);

            return await ResponseProviderBus.GetAllAsync<TRequest, TResult>(_serviceProvider, context, _logger, request);
        }

        public async Task<TResult> ResponseAsync<TResult>()
        {
            var context = await _contextProvider.GetAsync(_serviceProvider);

            return await ResponseAsync<TResult>(context);
        }

        public Task<TResult> ResponseAsync<TResult>(IExecutionContextReader context)
        {
            return ResponseProviderBus.GetAsync<TResult>(_serviceProvider, context, _logger, failSafe: false);
        }

        public async Task<TResult> TryResponseAsync<TResult>()
        {
            var context = await _contextProvider.GetAsync(_serviceProvider);

            return await TryResponseAsync<TResult>(context);
        }

        public Task<TResult> TryResponseAsync<TResult>(IExecutionContextReader context)
        {
            return ResponseProviderBus.GetAsync<TResult>(_serviceProvider, context, _logger, failSafe: true);
        }
    }
}
