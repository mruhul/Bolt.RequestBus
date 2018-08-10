using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IResponseProvider
    {
        Task<TResult> GetAsync<TResult>();
        Task<TResult> TryGetAsync<TResult>();
        Task<IResponse<TResult>> GetAsync<TRequest, TResult>(TRequest request);
        Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TResult>();
        Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TRequest, TResult>(TRequest request);
    }

    public class ResponseProvider : IResponseProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ResponseProvider> _logger;

        public ResponseProvider(IServiceProvider serviceProvider, ILogger<ResponseProvider> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TResult>()
        {
            var context = await _serviceProvider.BuildContextAsync();
            return await ResponseProviderBus.GetAllAsync<TResult>(_serviceProvider, context, _logger);
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await ResponseProviderBus.GetAllAsync<TRequest, TResult>(_serviceProvider, context, _logger, request);
        }

        public async Task<TResult> GetAsync<TResult>()
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await ResponseProviderBus.GetAsync<TResult>(_serviceProvider, context, _logger, failSafe: false);
        }

        public async Task<TResult> TryGetAsync<TResult>()
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await ResponseProviderBus.GetAsync<TResult>(_serviceProvider, context, _logger, failSafe: true);
        }

        public async Task<IResponse<TResult>> GetAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            return await ResponseProviderBus.GetAsync<TRequest, TResult>(_serviceProvider, context, _logger, request, failSafe: false);
        }

        private ResponseUnit<TResult> Convert<TResult>(IResponse<TResult> source, ExecutionHintType hintType)
        {
            if (source == null) return null;

            return new ResponseUnit<TResult>
            {
                Errors = source.Errors,
                Result = source.Result,
                IsMainResponse = hintType == ExecutionHintType.Main,
                IsSucceed = source.IsSucceed
            };
        }
    }
}
