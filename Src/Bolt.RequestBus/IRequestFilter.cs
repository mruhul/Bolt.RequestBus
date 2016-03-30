namespace Bolt.RequestBus
{
    public interface IRequestFilter<in TRequest> where TRequest : IRequest
    {
        void OnInit(TRequest request);
        void OnValidated(TRequest request);
    }


    public interface IRequestFilter<in TRequest, in TValue> where TRequest : IRequest
    {
        void OnInit(TRequest request);
        void OnValidated(TRequest request);
        void OnCompleted(TRequest request, TValue value);
    }
}