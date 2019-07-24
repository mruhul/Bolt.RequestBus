using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    internal static class ResponseProviderBus
    {
        public static async Task<TResult> GetAsync<TResult>(IServiceProvider sp, IExecutionContextReader context, ILogger logger, bool failSafe)
        {
            var handler = sp.GetServices<IResponseHandlerAsync<TResult>>()
                            ?.Where(h => h.IsApplicable(context)).FirstOrDefault();

            if (handler == null)
            {
                if (failSafe)
                {
                    logger.LogWarning($"No handler found that impement IResponseProvider<{typeof(TResult).FullName}>");

                    return default(TResult);
                }

                throw new RequestBusException($"No response handler defined for type {typeof(IResponseHandlerAsync<TResult>)}");
            }

            var response = await handler.Handle(context);

            if (response == null)
            {
                return default(TResult);
            }

            await sp.FilterAsync(context, response, logger);

            return response.Result;
        }

        public static async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TRequest, TResult>(IServiceProvider sp, IExecutionContextReader context, ILogger logger, TRequest request)
        {
            var errors = await sp.ValidateAsync(context, request, logger);

            var result = new List<IResponseUnit<TResult>>();

            if (errors.Any())
            {
                result.Add(new ResponseUnit<TResult>
                {
                    Errors = errors,
                    IsMainResponse = true,
                    IsSucceed = false
                });

                return result;
            }

            var allApplicableHandlers = sp.GetServices<IResponseHandlerAsync<TRequest, TResult>>()
                               ?.Where(t => t.IsApplicable(context, request))
                               ?? Enumerable.Empty<IResponseHandlerAsync<TRequest, TResult>>();

            var mainHandlers = allApplicableHandlers.Where(t => t.ExecutionHint == ExecutionHintType.Independent
                                                    || t.ExecutionHint == ExecutionHintType.Main);

            var mainHandlerTasks = mainHandlers
                                        .Select(h => HandleResponseHandler(context, h, request, logger));

            await Task.WhenAll(mainHandlerTasks);

            var isMainFailed = false;

            foreach (var task in mainHandlerTasks)
            {
                if (task.Result == null) continue;

                if (task.Result.IsMainResponse)
                {
                    isMainFailed = !task.Result.IsSucceed;
                }

                result.Add(task.Result);
            }

            if (isMainFailed) return result;

            var dependentHandlers = allApplicableHandlers.Where(h => h.ExecutionHint == ExecutionHintType.Dependent);

            var dependentHandlerTasks = dependentHandlers
                                        .Select(h => HandleResponseHandler(context, h, request, logger));

            await Task.WhenAll(dependentHandlerTasks);

            foreach (var task in dependentHandlerTasks)
            {
                if (task.Result == null) continue;

                result.Add(task.Result);
            }

            var filters = sp.GetServices<IResponseCollectionFilterAsync<TRequest, TResult>>()
                            ?.Where(x => x.IsApplicable(context, request))
                            ?? Enumerable.Empty<IResponseCollectionFilterAsync<TRequest, TResult>>();

            foreach (var filter in filters)
            {
                await filter.Filter(context, request, result);
            }

            return result;
        }

        public static async Task<IEnumerable<IResponseUnit<TResult>>> GetAllAsync<TResult>(IServiceProvider sp, IExecutionContextReader context, ILogger logger)
        {
            var result = new List<IResponseUnit<TResult>>();

            var allApplicableHandlers = sp.GetServices<IResponseHandlerAsync<TResult>>()
                               ?.Where(t => t.IsApplicable(context))
                               ?? Enumerable.Empty<IResponseHandlerAsync<TResult>>();

            var mainHandlers = allApplicableHandlers.Where(t => t.ExecutionHint == ExecutionHintType.Independent
                                                    || t.ExecutionHint == ExecutionHintType.Main);

            var mainHandlerTasks = mainHandlers
                                        .Select(h => HandleResponseHandler(h, context, logger));

            await Task.WhenAll(mainHandlerTasks);

            var isMainFailed = false;

            foreach (var task in mainHandlerTasks)
            {
                if (task.Result == null) continue;

                if (task.Result.IsMainResponse)
                {
                    isMainFailed = !task.Result.IsSucceed;
                }

                result.Add(task.Result);
            }

            if (isMainFailed) return result;

            var dependentHandlers = allApplicableHandlers.Where(h => h.ExecutionHint == ExecutionHintType.Dependent);

            var dependentHandlerTasks = dependentHandlers
                                        .Select(h => HandleResponseHandler(h, context, logger));

            await Task.WhenAll(dependentHandlerTasks);

            foreach (var task in dependentHandlerTasks)
            {
                if (task.Result == null) continue;

                result.Add(task.Result);
            }

            var filters = sp.GetServices<IResponseCollectionFilterAsync<TResult>>()
                            ?.Where(x => x.IsApplicable(context))
                            ?? Enumerable.Empty<IResponseCollectionFilterAsync<TResult>>();

            foreach (var filter in filters)
            {
                await filter.Filter(context, result);
            }

            return result;
        }

        private static async Task<IResponseUnit<TResult>> HandleResponseHandler<TResult>(IResponseHandlerAsync<TResult> h, IExecutionContextReader context, ILogger logger)
        {
            IResponse<TResult> rsp;

            if (h.ExecutionHint == ExecutionHintType.Main)
            {
                rsp = await h.Handle(context);
            }
            else
            {
                try
                {
                    rsp = await h.Handle(context);
                }
                catch (Exception e)
                {
                    rsp = Response.Failed<TResult>();
                    logger.LogError(0, e, e.Message);
                }
            }

            return Convert(rsp, h.ExecutionHint);
        }

        private static async Task<IResponseUnit<TResult>> HandleResponseHandler<TRequest, TResult>(IExecutionContextReader context, IResponseHandlerAsync<TRequest, TResult> h, TRequest request, ILogger logger)
        {
            IResponse<TResult> rsp;
            if (h.ExecutionHint != ExecutionHintType.Main)
            {
                rsp = await h.Handle(context, request);
            }
            else
            {
                try
                {
                    rsp = await h.Handle(context, request);
                }
                catch (Exception e)
                {
                    rsp = Response.Failed<TResult>();

                    logger.LogError(0, e, $"ResponseHandler {h.GetType().FullName} failed with message {e.Message}");
                }
            }

            return Convert(rsp, h.ExecutionHint);
        }

        private static ResponseUnit<TResult> Convert<TResult>(IResponse<TResult> source, ExecutionHintType hintType)
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
