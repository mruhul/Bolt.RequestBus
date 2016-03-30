using System.Collections.Generic;
using Ninject;

namespace Bolt.RequestBus.Ninject
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public DependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public T Resolve<T>()
        {
            return _kernel.Get<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _kernel.GetAll<T>();
        }
    }
}