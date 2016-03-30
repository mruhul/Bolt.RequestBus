namespace Sample.CreateBook
{
    public interface IEventSourceRepo
    {
        void Save<TEvent>(TEvent evnt);
    }
}