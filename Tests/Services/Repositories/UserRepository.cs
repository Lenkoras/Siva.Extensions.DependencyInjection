using Tests.Models;
using Tests.Services.Generators;

namespace Tests.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> users;
        private readonly IUserTokenGenerator userTokenGenerator;

        public UserRepository(IUserTokenGenerator userTokenGenerator)
        {
            this.userTokenGenerator = userTokenGenerator
                ?? throw new ArgumentNullException(nameof(userTokenGenerator));

            users = new(3) {
                new("Eva"),
                new("Siva"),
                new("Martin"),
                new("Max"),
                new("Slava")
            };
        }

        public ValueTask AddAsync(User user, CancellationToken cancellationToken)
        {
            users.Add(user);
            return ValueTask.CompletedTask;
        }

        public ValueTask<User?> GetByNameAsync(string name)
        {
            return ValueTask.FromResult(users.FirstOrDefault(user => user.Name == name));
        }

        public string GenerateToken(User user)
        {
            return userTokenGenerator.Create(user);
        }
    }
}