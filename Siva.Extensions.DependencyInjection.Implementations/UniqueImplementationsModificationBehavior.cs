using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Siva.Extensions.DependencyInjection.Implementations
{
    /// <summary>
    /// Represents the behavior of modifying a collection of services. It adds only unique service descriptors.
    /// </summary>
    public class UniqueImplementationsModificationBehavior :
        IServiceCollectionModificationBehavior
    {
        private readonly ServiceLifetime lifetime;
        private readonly HashSet<Type> _setOfUsedServiceTypes = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnrestrictedModificationBehavior"/> class with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the services.</param>
        public UniqueImplementationsModificationBehavior(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            this.lifetime = lifetime;
        }


        /// <inheritdoc/>
        public bool TryAdd(IServiceCollection services, Type serviceType, Type implementationType)
        {
            // if this type of service is not already in use, then adds to the service collection
            if (_setOfUsedServiceTypes.Add(serviceType))
            {
                services.TryAdd(new ServiceDescriptor(
                        serviceType: serviceType,
                        implementationType: implementationType,
                        lifetime: lifetime));
                return true;
            }
            return false;
        }
    }
}
