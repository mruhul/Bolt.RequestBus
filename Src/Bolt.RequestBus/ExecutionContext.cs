using System.Collections.Concurrent;
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

    internal class ExecutionContext : IExecutionContextReader, IExecutionContextWriter
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
}
