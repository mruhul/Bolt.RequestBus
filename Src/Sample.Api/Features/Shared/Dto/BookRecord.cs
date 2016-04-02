using System;

namespace Sample.Api.Features.Shared.Dto
{
    public class BookRecord
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
    }
}
