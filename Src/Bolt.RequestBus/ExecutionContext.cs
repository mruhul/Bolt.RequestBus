using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IExecutionContext
    {
        object Get(string key);
    }

    public interface IExecutionContextWriter
    {
        void Write(string key, object value);
    }

    public interface IExecutionContextInitializerAsync
    {
        Task Init(IExecutionContextWriter writer);
    }

    public class ExecutionContext : IExecutionContext, IExecutionContextWriter
    {
        private readonly ConcurrentDictionary<string, object> _store = new ConcurrentDictionary<string, object>();

        public object Get(string key)
        {
            return _store.TryGetValue(key, out object result) ? result : null;
        }

        public void Write(string key, object value)
        {
            _store.AddOrUpdate(key, value, (k, v) => value);
        }
    }

    public static class ExecutionContextExtensions
    {
        public static T Get<T>(this IExecutionContext source, string key)
        {
            return (T)(source.Get(key));
        }

        internal static async Task<IEnumerable<IError>> ValidateAsync<TRequest>(this IServiceProvider serviceProvider, IExecutionContext context, TRequest request)
        {
            var validators = serviceProvider.GetServices<IValidatorAsync<TRequest>>()
                                ?.Where(s => s.IsApplicable(context, request))
                                ?? Enumerable.Empty<IValidatorAsync<TRequest>>();

            foreach(var val in validators)
            {
                var errors = await val.Validate(context, request);

                if(errors != null && errors.Any())
                {
                    return errors;
                }
            }

            return Enumerable.Empty<IError>();
        }

        internal static async Task<IExecutionContext> BuildContextAsync(this IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetServices<IExecutionContextInitializerAsync>();

            var context = new ExecutionContext();

            if(provider != null)
            {
                foreach(var p in provider)
                {
                    await p.Init(context);
                }
            }

            return context;
        }
    }
}
