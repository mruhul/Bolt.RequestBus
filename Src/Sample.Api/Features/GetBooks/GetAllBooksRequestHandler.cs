using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bolt.Common.Extensions;
using Bolt.RequestBus.Handlers;
using Sample.Api.Infrastructure.Mappers;

namespace Sample.Api.Features.GetBooks
{
    public class GetAllBooksRequestHandler : AsyncRequestHandlerBase<GetAllBooksRequest, IEnumerable<BookDto>>
    {
        private readonly IBookRepository repository;
        private readonly IMapper mapper;

        public GetAllBooksRequestHandler(IBookRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        protected override async Task<IEnumerable<BookDto>> ProcessAsync(GetAllBooksRequest msg)
        {
            var records = await repository.GetAllAsync();

            return records.NullSafe().Select(record => mapper.Map<BookDto>(record));
        }
    }
}