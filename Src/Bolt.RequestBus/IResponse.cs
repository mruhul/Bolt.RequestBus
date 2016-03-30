using System.Collections.Generic;

namespace Bolt.RequestBus
{
    public interface IResponse
    {
        bool Succeed { get; }
        IEnumerable<IError> Errors { get; }
    }

    public interface IResponse<out T> : IResponse
    {
        T Value { get; }
    }
}
