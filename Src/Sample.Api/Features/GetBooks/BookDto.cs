using System;

namespace Sample.Api.Features.GetBooks
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
        public decimal Discount { get; set; }

        public decimal DisplayPrice => Discount > 0 ? Price - (Price*(Discount/100)) : Price;
    }
}