using System;

namespace Sample.Api.Features.GetBooks
{
    public class DiscountDto
    {
        public Guid BookId { get; set; }
        public decimal Discount { get; set; }
    }
}