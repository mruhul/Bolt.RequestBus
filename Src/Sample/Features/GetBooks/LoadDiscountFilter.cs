using System.Linq;
using Bolt.RequestBus.Filters;

namespace Sample.GetBooks
{
    public class LoadDiscountFilter : RequestFilterBase<GetBookByIdRequest, BookDto>
    {
        private readonly IDiscountApiProxy proxy;

        public LoadDiscountFilter(IDiscountApiProxy proxy)
        {
            this.proxy = proxy;
        }

        public override void OnCompleted(GetBookByIdRequest request, BookDto value)
        {
            var discounts = proxy.Get(new[] {value.Id});
            value.Discount = discounts.FirstOrDefault(x => x.BookId == value.Id)?.Discount ?? 0;
        }
    }
}