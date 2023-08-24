using System.Diagnostics.CodeAnalysis;

namespace Siva.Extensions.DependencyInjection.Providers
{
    internal abstract class BaseServiceTypeProvider : IServiceTypeProvider
    {
        public Type AssignableType { get; }

        public BaseServiceTypeProvider(Type assignableType)
        {
            ArgumentNullException.ThrowIfNull(assignableType);

            AssignableType = assignableType;
        }

        public bool TryProvide(Type implementationType, [NotNullWhen(true)] out Type? serviceType)
        {
            serviceType = Provide(implementationType);

            return serviceType is not null;
        }

        protected abstract Type? Provide(Type implementationType);
    }
}
