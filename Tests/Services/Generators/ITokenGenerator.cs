using Tests.Models;

namespace Tests.Services.Generators
{
    public interface ITokenGenerator<T>
    {
        string Create(User user);
    }
}