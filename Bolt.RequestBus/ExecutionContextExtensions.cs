namespace Bolt.RequestBus
{
    public static class ExecutionContextExtensions
    {
        public static T Get<T>(this IExecutionContextReader source, string key)
        {
            return (T)(source.Get(key));
        }
    }
}
