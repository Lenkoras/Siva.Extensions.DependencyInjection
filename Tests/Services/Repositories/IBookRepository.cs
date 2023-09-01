using Tests.Models;

namespace Tests.Services.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
    }

    public class BookRepository : IBookRepository
    {
        public ValueTask AddAsync(Book entity, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}