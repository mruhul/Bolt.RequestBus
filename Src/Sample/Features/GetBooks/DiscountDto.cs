using System;

namespace Sample.GetBooks
{
    public class DiscountDto
    {
        public Guid BookId { get; set; }
        public decimal Discount { get; set; }
    }
}