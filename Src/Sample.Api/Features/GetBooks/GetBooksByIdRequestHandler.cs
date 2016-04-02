using System.Linq;
using Bolt.Common.Extensions;
using Bolt.RequestBus.Handlers;
using Sample.Api.Features.Shared;
using Sample.Api.Infrastructure;
using Sample.Api.Infrastructure.Mappers;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.GetBooks
{
    public class GetBooksByIdRequestHandler : RequestHandlerBase<GetBookByIdRequest, BookDto>
    {
        private readonly IBookRepository repository;
        private readonly IMapper mapper;

        public GetBooksByIdRequestHandler(IBookRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        protected override BookDto Process(GetBookByIdRequest msg)
        {
            return repository.GetById(msg.Id).NullSafeGet(bookRecord => mapper.Map<BookDto>(bookRecord));
        }
    }
}