namespace Sample.Api.Infrastructure.Mappers
{
    public class AutoMappedMapper : IMapper
    {
        public TOutput Map<TOutput>(object input)
        {
            return AutoMapper.Mapper.Map<TOutput>(input);
        }
    }
}