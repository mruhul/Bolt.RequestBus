namespace Bolt.RequestBus
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        void Handle(TEvent eEvent);
    }
}