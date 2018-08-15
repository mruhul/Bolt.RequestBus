using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bolt.RequestBus
{
    internal class Timer
    {
        private readonly Stopwatch _sw;
        private readonly ILogger _logger;
        private readonly string _name; 

        public Timer(ILogger logger, string name)
        {
            _sw = Stopwatch.StartNew();
            _logger = logger;
            _name = name;

            _logger.LogDebug($"{_name}: started");
        }

        public void Completed()
        {
            _sw.Stop();
            _logger.LogDebug($"{_name}: completed in {_sw.ElapsedMilliseconds}ms");
        }

        public static Timer Start<T>(ILogger logger, T handler)
        {
            return new Timer(logger, handler.GetType().FullName);
        }
    }
}
