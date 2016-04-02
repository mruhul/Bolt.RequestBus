using System.Linq;
using Bolt.Common.Extensions;
using Sample.Api.Features.Shared;
using Sample.Api.Features.Shared.Dto;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.CreateBook
{
    public interface IBookRepository
    {
        void Insert(BookRecord record);
        BookRecord GetByTitle(string title);
    }

    public class BookRepository : IBookRepository
    {
        private readonly IPersistentStore store;

        public BookRepository(IPersistentStore store)
        {
            this.store = store;
        }

        public void Insert(BookRecord record)
        {
            store.Write(record);
        }

        public BookRecord GetByTitle(string title)
        {
            return store.Read<BookRecord>().FirstOrDefault(x => x.Title.IsSame(title));
        }
    }
}