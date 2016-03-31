namespace Sample.EventSource
{
    public interface IEventSourceRepo
    {
        void Save<TEvent>(TEvent evnt);
    }
}