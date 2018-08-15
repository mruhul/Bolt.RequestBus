using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IExecutionContextProvider
    {
        Task<IExecutionContextReader> GetAsync(IServiceProvider serviceProvider);
    }

    internal class ExecutionContextProvider : IExecutionContextProvider
    {      
        private IExecutionContextReader _context;

        public async Task<IExecutionContextReader> GetAsync(IServiceProvider serviceProvider)
        {
            if (_context != null) return _context;

            var populators = serviceProvider.GetServices<IExecutionContextPopulatorAsync>();

            var context = new ExecutionContext();

            if (populators != null)
            {
                var tasks = populators.Select(p => p.Init(context));

                await Task.WhenAll(tasks);
            }

            _context = context;

            return _context;
        }
    }
}
