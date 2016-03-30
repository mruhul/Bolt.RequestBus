using System.Collections.Generic;

namespace Bolt.RequestBus
{
    public interface IDependencyResolver
    {
        T Resolve<T>();
        IEnumerable<T> ResolveAll<T>();
    }
}