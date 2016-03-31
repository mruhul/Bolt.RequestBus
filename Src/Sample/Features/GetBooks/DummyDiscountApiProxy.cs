using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.Features.GetBooks
{
    public class DummyDiscountApiProxy : IDiscountApiProxy
    {
        private readonly Random rnd = new Random();

        public IEnumerable<DiscountDto> Get(IEnumerable<Guid> bookIds)
        {
            return bookIds.Select(bookId => new DiscountDto
            {
                BookId = bookId,
                Discount = rnd.Next(0, 30)
            });
        }
    }
}