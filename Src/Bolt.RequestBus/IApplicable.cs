using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.RequestBus
{
    public interface IApplicable
    {
        bool IsApplicable(IExecutionContext context);
    }

    public interface IApplicable<TRequest>
    {
        bool IsApplicable(IExecutionContext context, TRequest request);
    }
}
