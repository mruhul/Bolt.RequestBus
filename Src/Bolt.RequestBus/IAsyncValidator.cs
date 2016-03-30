using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bolt.RequestBus
{
    public interface IAsyncValidator<in TRequest> where TRequest : IRequest
    {
        Task<IEnumerable<IError>> ValidateAsync(TRequest request);
        int Order { get; }
    }
}