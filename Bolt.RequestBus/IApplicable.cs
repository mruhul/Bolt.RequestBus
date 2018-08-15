using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.RequestBus
{
    public interface IApplicable
    {
        bool IsApplicable(IExecutionContextReader context);
    }

    public interface IApplicable<TRequest>
    {
        bool IsApplicable(IExecutionContextReader context, TRequest request);
    }
}
