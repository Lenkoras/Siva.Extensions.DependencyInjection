using Tests.Models;

namespace Tests.Services.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        ValueTask<User?> GetByNameAsync(string name);
        string GenerateToken(User user);
    }
}