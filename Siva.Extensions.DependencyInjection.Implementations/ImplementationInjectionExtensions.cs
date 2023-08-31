using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siva.Extensions.DependencyInjection.Factories;
using Siva.Extensions.DependencyInjection.Providers;
using System.Reflection;

namespace Siva.Extensions.DependencyInjection
{
    /// <summary>
    /// An extension class designed to add all implementations of an interface or class from a sequence of types or assembly.
    /// </summary>
    public static partial class ImplementationInjectionExtensions
    {
        /// <summary>
        /// Adds all unique implementations of the <paramref name="serviceTypeToScan"/> to the collection of <paramref name="services"/>.
        /// If the <paramref name="serviceTypeToScan"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="serviceTypeToScan"/> or this interface itself.
        /// If the <paramref name="serviceTypeToScan"/> is a class, then the service type will be its implementation.
        /// The types of implementations will be taken from the specified <paramref name="typesToScan"/>.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="serviceTypeToScan">Any non-sealed type.</param>
        /// <param name="typesToScan">Types to scan.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="serviceTypeToScan"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type serviceTypeToScan, IEnumerable<Type> typesToScan, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceTypeToScan);
            ArgumentNullException.ThrowIfNull(typesToScan);

            IServiceTypeProvider serviceTypeProvider = new ServiceTypeProviderFactory().Create(serviceTypeToScan);

            HashSet<Type> setOfUsedServiceTypes = new();

            IEnumerable<Type> types = typesToScan.Where(IsNotValueImplementationType);

            foreach (Type implementationType in types)
            {
                if (serviceTypeProvider.TryProvide(implementationType, out Type? serviceType) &&
                    setOfUsedServiceTypes.Add(serviceType)) // if this type of service is not already in use, then adds to the service collection
                {
                    services.TryAdd(new ServiceDescriptor(
                        serviceType: serviceType,
                        implementationType: implementationType,
                        lifetime: lifetime));
                }
            }

            return services;
        }

        /// <summary>
        /// Adds all unique implementations of the <paramref name="serviceTypeToScan"/> to the collection of <paramref name="services"/>.
        /// If the <paramref name="serviceTypeToScan"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="serviceTypeToScan"/> or this interface itself.
        /// If the <paramref name="serviceTypeToScan"/> is a class, then the service type will be its implementation.
        /// The types to search for public implementations will be taken from the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="serviceTypeToScan">Any non-sealed type.</param>
        /// <param name="assembly">Assembly to scan.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="serviceTypeToScan"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type serviceTypeToScan, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return AddImplementations(services, serviceTypeToScan, assembly.GetExportedTypes(), lifetime);
        }

        /// <summary>
        /// Adds all unique implementations of the <paramref name="serviceTypeToScan"/> to the collection of <paramref name="services"/>.
        /// If the <paramref name="serviceTypeToScan"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="serviceTypeToScan"/> or this interface itself.
        /// If the <paramref name="serviceTypeToScan"/> is a class, then the service type will be its implementation.
        /// The types to search for public implementations will be taken from the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="serviceTypeToScan">Any non-sealed type.</param>
        /// <param name="assemblies">The sequence of assembly to scan.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="serviceTypeToScan"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type serviceTypeToScan, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(assemblies);

            foreach (Assembly assembly in assemblies)
            {
                AddImplementations(services, serviceTypeToScan, assembly, lifetime);
            }

            return services;
        }

        private static bool IsNotValueImplementationType(Type type) =>
            !type.IsInterface && !type.IsAbstract && !type.IsValueType;
    }
}