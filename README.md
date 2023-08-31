# Siva.Extensions.DependencyInjection.Implementations
Extensions designed to add all implementations of an interface or class from a sequence of types or assemblies.

# How to use

Quick example:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddImplementations(typeof(IRepository<>), typeof(Program).Assembly);
}
```
This command will add all `IRepository<>` implementations to the `ServiceCollection` from the specified assembly.

For example, here the `UserRepository` class will be added as an implementation type and `IUserRepository` as a service type:
```csharp
public interface IRepository<T> 
{

}

public interface IUserRepository : IRepository<User>
{
    User? GetByNameAsync(string name);
}

public class UserRepository : IUserRepository
{
    public User? GetByNameAsync(string name)
    { 
        ...
    }
}
```

Or you can also use a class as a base type:
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