# Siva.Extensions.DependencyInjection.Implementations
Extensions designed to add all implementations of an interface or class from a sequence of types or assemblies to the IServiceCollection.

# How to use

Quick example:
```csharp
using Microsoft.Extensions.DependencyInjection;
using Siva.Extensions.DependencyInjection;

...
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(IRepository<>), typeof(Program).Assembly);
}
```
This command will add all `IRepository<T>` implementations to the `ServiceCollection` from the specified assembly.

For example, here the `UserRepository` class will be added as an implementation type and `IUserRepository` as a service type:
```csharp
public interface IRepository<T> 
{

}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByNameAsync(string name);
}

public class UserRepository : IUserRepository
{
    public Task<User?> GetByNameAsync(string name)
    { 
        ...
    }
}
```
And you can use this in your services as follows:
```csharp
public class CurrentUserService
{
    private readonly IUserRepository userRepository;

    public CurrentUserService(IUserRepository userRepository) 
    {
        this.userRepository = userRepository;
    }
}
```
In the future, when we add another public service implementing this interface (for example, `BookRepository` and `IBookRepository`), we won't need to manually add it to the collection of services.

Need to add a class as a service type? No problem, you can do it like this:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(Factory<>), typeof(Program).Assembly);
}

...

public abstract class Factory<T>
{
    public abstract T Create();
}

public class ShoeFactory : Factory<Shoe>
{
    public override Shoe Create() 
    {
        return new Shoe();
    }
}

public class TrainFactory : Factory<Train>
{
    public override Train Create() 
    {
        return new Train();
    }
}
```
And here, the `ShoeFactory` and `TrainFactory` types will be added as implementations, and the `Factory<Shoe>` and `Factory<Train>` types will act as service types, respectively.

By default, the lifetime of the added services is `ServiceLifetime.Scoped`, but if necessary, you can set any other as follows:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(Factory<>), typeof(Program).Assembly, ServiceLifetime.Singleton);
}
```

For specific scenarios, you can also add service implementations directly from the type collection:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(IRepository<>), typeof(Program).Assembly.GetTypes().Where(type => type.IsNotPublic));
}
```
In the example above, all non-public types that implement the `IRepository<T>` interface will be added.

# Restrictions

You can't use a sealed type (struct or class with a sealed modifier) as a type to scan. 
So, this example will throw an `ArgumentException`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(int), typeof(Program).Assembly);
}
```
This behavior is used because sealed types can't be implemented or inherited, and therefore scanning for such types is a logical error.

[See extra examples](https://github.com/Lenkoras/Siva.Extensions.DependencyInjection.Implementations/blob/main/Tests/ImplementationInjectionExtensionsTests.cs).