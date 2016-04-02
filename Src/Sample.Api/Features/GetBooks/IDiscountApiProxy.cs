using System;
using System.Collections.Generic;

namespace Sample.Api.Features.GetBooks
{
    public interface IDiscountApiProxy
    {
        IEnumerable<DiscountDto> Get(IEnumerable<Guid> bookIds);
    }
}
