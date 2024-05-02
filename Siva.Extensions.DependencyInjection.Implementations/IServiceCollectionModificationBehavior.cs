using Microsoft.Extensions.DependencyInjection;

namespace Siva.Extensions.DependencyInjection.Implementations
{
    /// <summary>
    /// Represents the behavior of modifying a collection of services.
    /// </summary>
    public interface IServiceCollectionModificationBehavior
    {
        /// <summary>
        /// Tries to add the specified <paramref name="serviceType"/> and its <paramref name="implementationType"/> to the collection of <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <param name="implementationType">The type of the service implementation.</param>
        /// <returns><see langword="true"/> if the service was added to the collection; otherwise, <see langword="false"/>.</returns>
        bool TryAdd(IServiceCollection services, Type serviceType, Type implementationType);
    }
}
