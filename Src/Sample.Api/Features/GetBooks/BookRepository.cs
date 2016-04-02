using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sample.Api.Features.Shared;
using Sample.Api.Infrastructure.PersistentStores;

namespace Sample.Api.Features.GetBooks
{
    public interface IBookRepository
    {
        BookRecord GetById(Guid id);
    }

    public class BookRepository : IBookRepository
    {
        private readonly IPersistentStore store;

        public BookRepository(IPersistentStore store)
        {
            this.store = store;
        }

        public BookRecord GetById(Guid id)
        {
            return store.Read<BookRecord>().SingleOrDefault(book => book.Id == id);
        }
    }
}