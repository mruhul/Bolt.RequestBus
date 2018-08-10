using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IEventHandlerAsync<TEvent> : IApplicable<TEvent>
    {
        Task Handle(IExecutionContext context, TEvent evnt);
    }

    public abstract class EventHandlerAsync<TEvent> : IEventHandlerAsync<TEvent>
    {
        public abstract Task Handle(IExecutionContext context, TEvent evnt);

        public virtual bool IsApplicable(IExecutionContext context, TEvent request) => true;
    }

    internal static class EventHandlerBus
    {
        public static Task PublishAsync<TEvent>(IServiceProvider sp, IExecutionContext context, ILogger logger, TEvent evnt, bool failSafe)
        {
            var handlers = sp.GetServices<IEventHandlerAsync<TEvent>>()
                            ?.Where(h => h.IsApplicable(context, evnt));

            if (handlers == null) return Task.FromResult(0);

            var tasks = handlers.Select(h => ExecutionHelper.Execute(async () => {

                if (!failSafe)
                {
                    await h.Handle(context, evnt);
                }
                else
                {
                    try
                    {
                        await h.Handle(context, evnt);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, e.Message);
                    }
                }
            }, h, logger));

            return Task.WhenAll(tasks);
        }
    }
}
