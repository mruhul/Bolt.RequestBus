using Microsoft.Extensions.DependencyInjection;
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
        Task<IResponse<TResult>> GetAsync<TRequest, TResult>(TRequest request);
        Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TResult>();
        Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TRequest, TResult>(TRequest request);
    }

    public class ResponseProvider : IResponseProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ResponseProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TResult>()
        {
            var result = new List<IResponseUnit<TResult>>();

            var context = await _serviceProvider.BuildContextAsync();

            var allApplicableHandlers = _serviceProvider.GetServices<IResponseHandlerAsync<TResult>>()
                               ?.Where(t => t.IsApplicable(context)) 
                               ?? Enumerable.Empty<IResponseHandlerAsync<TResult>>();

            var mainHandlers = allApplicableHandlers.Where(t => t.ExecutionHint == ExecutionHintType.Independent 
                                                    || t.ExecutionHint == ExecutionHintType.Main);

            var mainHandlerTasks = mainHandlers
                                        .Select(h => h.Handle(context)
                                                    .ContinueWith(t => Convert(t.Result, h.ExecutionHint)));

            await Task.WhenAll(mainHandlerTasks);

            var isMainFailed = false;

            foreach(var task in mainHandlerTasks)
            {
                if (task.Result == null) continue;

                if (task.Result.IsMainResponse)
                {
                    isMainFailed = task.Result.IsSucceed;
                }

                result.Add(task.Result);
            }

            if (isMainFailed) return result;

            var dependentHandlers = allApplicableHandlers.Where(h => h.ExecutionHint == ExecutionHintType.Dependent);

            var dependentHandlerTasks = dependentHandlers
                                        .Select(h => h.Handle(context)
                                                    .ContinueWith(t => Convert(t.Result, h.ExecutionHint)));

            await Task.WhenAll(dependentHandlerTasks);

            foreach(var task in dependentHandlerTasks)
            {
                if (task.Result == null) continue;

                result.Add(task.Result);
            }

            var filters = _serviceProvider.GetServices<IResponseCollectionFilterAsync<TResult>>()
                            ?.Where(x => x.IsApplicable(context)) 
                            ?? Enumerable.Empty<IResponseCollectionFilterAsync<TResult>>();

            foreach(var filter in filters)
            {
                await filter.Filter(context, result);
            }

            return result;
        }

        public async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            var errors = await _serviceProvider.ValidateAsync(context, request);

            var result = new List<IResponseUnit<TResult>>();

            if (errors.Any())
            {
                result.Add(new ResponseUnit<TResult> {
                    Errors = errors,
                    IsMainResponse = true,
                    IsSucceed = false
                });

                return result;
            }

            var allApplicableHandlers = _serviceProvider.GetServices<IResponseHandlerAsync<TRequest,TResult>>()
                               ?.Where(t => t.IsApplicable(context, request))
                               ?? Enumerable.Empty<IResponseHandlerAsync<TRequest,TResult>>();

            var mainHandlers = allApplicableHandlers.Where(t => t.ExecutionHint == ExecutionHintType.Independent
                                                    || t.ExecutionHint == ExecutionHintType.Main);

            var mainHandlerTasks = mainHandlers
                                        .Select(h => h.Handle(context, request)
                                                    .ContinueWith(t => Convert(t.Result, h.ExecutionHint)));

            await Task.WhenAll(mainHandlerTasks);

            var isMainFailed = false;

            foreach (var task in mainHandlerTasks)
            {
                if (task.Result == null) continue;

                if (task.Result.IsMainResponse)
                {
                    isMainFailed = task.Result.IsSucceed;
                }

                result.Add(task.Result);
            }

            if (isMainFailed) return result;

            var dependentHandlers = allApplicableHandlers.Where(h => h.ExecutionHint == ExecutionHintType.Dependent);

            var dependentHandlerTasks = dependentHandlers
                                        .Select(h => h.Handle(context, request)
                                                    .ContinueWith(t => Convert(t.Result, h.ExecutionHint)));

            await Task.WhenAll(dependentHandlerTasks);

            foreach (var task in dependentHandlerTasks)
            {
                if (task.Result == null) continue;

                result.Add(task.Result);
            }

            var filters = _serviceProvider.GetServices<IResponseCollectionFilterAsync<TRequest,TResult>>()
                            ?.Where(x => x.IsApplicable(context, request))
                            ?? Enumerable.Empty<IResponseCollectionFilterAsync<TRequest,TResult>>();

            foreach (var filter in filters)
            {
                await filter.Filter(context, request, result);
            }

            return result;
        }

        public async Task<TResult> GetAsync<TResult>()
        {
            var context = await _serviceProvider.BuildContextAsync();

            var handler = _serviceProvider.GetServices<IResponseHandlerAsync<TResult>>()
                            ?.Where(h => h.IsApplicable(context)).FirstOrDefault();

            if (handler == null) throw new Exception($"No response handler defined for type {typeof(IResponseHandlerAsync<TResult>)}");

            var response = await handler.Handle(context);

            if (response == null) return default(TResult);

            var filters = _serviceProvider.GetServices<IResponseFilterAsync<TResult>>()
                                ?.Where(h => h.IsApplicable(context))
                                ?? Enumerable.Empty<IResponseFilterAsync<TResult>>();

            if (filters == null)
            {
                foreach (var filter in filters)
                {
                    await filter.Filter(context, response);
                }
            }

            return response.Result;
        }

        public async Task<IResponse<TResult>> GetAsync<TRequest, TResult>(TRequest request)
        {
            var context = await _serviceProvider.BuildContextAsync();

            var errors = await _serviceProvider.ValidateAsync(context, request);

            if (errors.Any())
            {
                return Response.Failed<TResult>(errors);
            }

            var handler = _serviceProvider.GetServices<IResponseHandlerAsync<TRequest,TResult>>()
                            ?.Where(h => h.IsApplicable(context, request)).FirstOrDefault();

            if (handler == null) throw new Exception($"No response handler defined for type {typeof(IResponseHandlerAsync<TRequest,TResult>)}");

            var response = await handler.Handle(context, request);

            if (response == null) return null;

            var filters = _serviceProvider.GetServices<IResponseFilterAsync<TRequest, TResult>>()
                                ?.Where(h => h.IsApplicable(context, request))
                                ?? Enumerable.Empty<IResponseFilterAsync<TRequest, TResult>>();

            if(filters == null)
            {
                foreach(var filter in filters)
                {
                    await filter.Filter(context, request, response);
                }
            }

            return response;
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
