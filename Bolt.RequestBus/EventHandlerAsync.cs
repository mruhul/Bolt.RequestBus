using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IEventHandlerAsync<TEvent> : IApplicable<TEvent>
    {
        Task Handle(IExecutionContextReader context, TEvent evnt);
    }

    public abstract class EventHandlerAsync<TEvent> : IEventHandlerAsync<TEvent>
    {
        public abstract Task Handle(IExecutionContextReader context, TEvent evnt);

        public virtual bool IsApplicable(IExecutionContextReader context, TEvent request) => true;
    }

    internal static class EventHandlerBus
    {
        public static Task PublishAsync<TEvent>(IServiceProvider sp, IExecutionContextReader context, ILogger logger, TEvent evnt, bool failSafe)
        {
            var handlers = sp.GetServices<IEventHandlerAsync<TEvent>>()
                            ?.Where(h => h.IsApplicable(context, evnt));

            if (handlers == null) return Task.FromResult(0);

            var tasks = handlers.Select(async h => {

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
                        logger.LogError(0, e, e.Message);
                    }
                }
            });

            return Task.WhenAll(tasks);
        }
    }
}
