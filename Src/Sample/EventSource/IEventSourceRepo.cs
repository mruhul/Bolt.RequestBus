using System.Runtime.InteropServices;
using Bolt.Logger;
using Bolt.Serializer;

namespace Sample.CreateBook
{
    public interface IEventSourceRepo
    {
        void Save<TEvent>(TEvent evnt);
    }

    public class LoggerBasedEventSourceRepo : IEventSourceRepo
    {
        private readonly ILogger logger;
        private readonly ISerializer serializer;

        public LoggerBasedEventSourceRepo(ILogger logger, ISerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
        }

        public void Save<TEvent>(TEvent evnt)
        {
            logger.Info($"Event {evnt.GetType().FullName} : {serializer.Serialize(evnt)}");
        }
    }
}