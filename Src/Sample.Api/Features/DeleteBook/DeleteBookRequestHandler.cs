using System.Threading.Tasks;
using Bolt.RequestBus.Handlers;

namespace Sample.Api.Features.DeleteBook
{
    public class DeleteBookRequestHandler : AsyncRequestHandlerBase<DeleteBookRequest>
    {
        private readonly IBookRepository repo;

        public DeleteBookRequestHandler(IBookRepository repo)
        {
            this.repo = repo;
        }

        protected override Task ProcessAsync(DeleteBookRequest msg)
        {
            return repo.DeleteAsync(msg.Id);
        }
    }
}