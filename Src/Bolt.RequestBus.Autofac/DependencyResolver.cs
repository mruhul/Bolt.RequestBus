using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Bolt.RequestBus.Autofac
{
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IComponentContext _context;

        public DependencyResolver(IComponentContext context)
        {
            _context = context;
        }

        public T Resolve<T>()
        {
            return _context.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _context.Resolve<IEnumerable<T>>() ?? Enumerable.Empty<T>();
        }
    }
}
