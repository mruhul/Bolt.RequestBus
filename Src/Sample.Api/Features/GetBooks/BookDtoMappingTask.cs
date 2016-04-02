using Sample.Api.Features.Shared;
using Sample.Api.Features.Shared.Dto;
using Sample.Api.Infrastructure;

namespace Sample.Api.Features.GetBooks
{
    public class BookDtoMappingTask : IStartUpTask
    {
        public void Run()
        {
            //TODO: Find a proper way to use automapper as this way of doing will be obsolute in next version
            AutoMapper.Mapper.CreateMap<BookRecord, BookDto>()
                .ForMember(p => p.Discount, opt => opt.Ignore());
        }
    }
}