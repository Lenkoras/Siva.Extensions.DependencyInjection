using Microsoft.Extensions.DependencyInjection;

namespace Siva.Extensions.DependencyInjection.Implementations
{
    /// <summary>
    /// Represents the behavior of modifying a collection of services. It adds all service descriptors without any restrictions.
    /// </summary>
    public class UnrestrictedModificationBehavior :
        IServiceCollectionModificationBehavior
    {
        private readonly ServiceLifetime lifetime;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedModificationBehavior"/> class with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the services.</param>
        public UnrestrictedModificationBehavior(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            this.lifetime = lifetime;
        }

        /// <inheritdoc/>
        public bool TryAdd(IServiceCollection services, Type serviceType, Type implementationType)
        {
            services.Add(new ServiceDescriptor(
                        serviceType: serviceType,
                        implementationType: implementationType,
                        lifetime: lifetime));
            return true;
        }
    }
}
