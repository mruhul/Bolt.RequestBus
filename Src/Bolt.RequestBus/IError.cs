namespace Bolt.RequestBus
{
    public interface IError
    {
        string ErrorCode { get; }
        string PropertyName { get; }
        string ErrorMessage { get; }
    }
}
