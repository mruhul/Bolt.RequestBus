using System;
using System.Collections.Generic;

namespace Sample.Features.GetBooks
{
    public interface IDiscountApiProxy
    {
        IEnumerable<DiscountDto> Get(IEnumerable<Guid> bookIds);
    }
}
