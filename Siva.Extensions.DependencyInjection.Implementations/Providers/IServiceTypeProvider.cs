using System.Diagnostics.CodeAnalysis;

namespace Siva.Extensions.DependencyInjection.Providers
{
    internal interface IServiceTypeProvider
    {
        bool TryProvide(Type implementationType, [NotNullWhen(true)] out Type? serviceType);
    }
}
