namespace Tests.Services.Repositories
{
    public interface IRepository<T>
    {
        ValueTask AddAsync(T entity, CancellationToken cancellationToken);
    }
}