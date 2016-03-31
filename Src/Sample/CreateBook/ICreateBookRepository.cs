using Bolt.Logger;
using Bolt.Serializer;

namespace Sample.CreateBook
{
    public interface ICreateBookRepository
    {
        void Create(CreateBookData data);
    }

    public class CreateBookRepository : ICreateBookRepository
    {
        private readonly ILogger logger;
        private readonly ISerializer serializer;

        public CreateBookRepository(ILogger logger, ISerializer serializer)
        {
            this.logger = logger;
            this.serializer = serializer;
        }

        public void Create(CreateBookData data)
        {
            logger.Info($"Book Data: {serializer.Serialize(data)}");
        }
    }
}