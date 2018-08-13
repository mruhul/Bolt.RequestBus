using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IExecutionContextReader
    {
        object Get(string key);
        bool Exists(string key);
    }

    public interface IExecutionContextWriter
    {
        void Write(string key, object value);
    }

    internal interface IExecutionContext : IExecutionContextReader, IExecutionContextWriter
    {
    }

    public interface IExecutionContextPopulatorAsync
    {
        Task Init(IExecutionContextWriter writer);
    }

    public interface IExecutionContextPopulatorAsync<TRequest>
    {
        Task Init(IExecutionContextWriter writer, TRequest request);
        bool IsApplicable(IExecutionContextReader context, TRequest request);
    }

    public class ExecutionContext : IExecutionContextReader, IExecutionContextWriter
    {
        private readonly ConcurrentDictionary<string, object> _store = new ConcurrentDictionary<string, object>();

        public object Get(string key)
        {
            return _store.TryGetValue(key, out object result) ? result : null;
        }

        public bool Exists(string key)
        {
            return _store.TryGetValue(key, out object _);
        }

        public void Write(string key, object value)
        {
            _store.AddOrUpdate(key, value, (k, v) => value);
        }
    }

    public static class ExecutionContextExtensions
    {
        public static T Get<T>(this IExecutionContextReader source, string key)
        {
            return (T)(source.Get(key));
        }

        
        internal static async Task<IExecutionContextReader> BuildContextAsync(this IServiceProvider serviceProvider)
        {
            var providers = serviceProvider.GetServices<IExecutionContextPopulatorAsync>();

            var context = new ExecutionContext();

            if(providers != null)
            {
                await Task.WhenAll(providers.Select(p => p.Init(context)));
            }

            return context;
        }
    }
}
