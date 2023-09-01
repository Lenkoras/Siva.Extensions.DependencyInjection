using Tests.Models;

namespace Tests.Services.Generators
{
    public class UserTokenGenerator : IUserTokenGenerator
    {
        public string Create(User user)
        {
            return $"User.{user.Name}.{Guid.NewGuid()}";
        }
    }
}