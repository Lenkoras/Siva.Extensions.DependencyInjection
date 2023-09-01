using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Siva.Extensions.DependencyInjection.Factories;
using Siva.Extensions.DependencyInjection.Providers;
using System.Reflection;

namespace Siva.Extensions.DependencyInjection
{
    /// <summary>
    /// An extension class designed to add all implementations of an interface or class from a sequence of types or assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    public static partial class ImplementationInjectionExtensions
    {
        /// <summary>
        /// Selects all unique implementation types of the <paramref name="targetAssignableType"/> 
        /// from the specified collection of <paramref name="types"/> 
        /// and adds them to the collection of <paramref name="services"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the <paramref name="types"/>.</param>
        /// <param name="types">Types to scan.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type targetAssignableType, IEnumerable<Type> types, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(targetAssignableType);
            ArgumentNullException.ThrowIfNull(types);

            IServiceTypeProvider serviceTypeProvider = new ServiceTypeProviderFactory().Create(targetAssignableType);

            HashSet<Type> setOfUsedServiceTypes = new();

            IEnumerable<Type> implementationTypes = types.Where(IsNotValueImplementationType);

            foreach (Type implementationType in implementationTypes)
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
        /// Selects all unique implementation types of the <paramref name="targetAssignableType"/>
        /// that are taken from the specified <paramref name="assembly"/>
        /// and adds them to the collection of <paramref name="services"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface,
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the exported types.</param>
        /// <param name="assembly">Assembly to scan. Takes all exported types from this assembly.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type targetAssignableType, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return AddImplementations(services, targetAssignableType, assembly.GetExportedTypes(), lifetime);
        }

        /// <summary>
        /// Selects all unique implementation types of the <paramref name="targetAssignableType"/>
        /// that are taken from the specified <paramref name="assemblies"/> and 
        /// adds them to the collection of <paramref name="services"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the exported types.</param>
        /// <param name="assemblies">Assemblies to scan. Takes all exported types from these assemblies.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services, Type targetAssignableType, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullException.ThrowIfNull(assemblies);

            foreach (Assembly assembly in assemblies)
            {
                AddImplementations(services, targetAssignableType, assembly, lifetime);
            }

            return services;
        }

        private static bool IsNotValueImplementationType(Type type) =>
            !type.IsInterface && !type.IsAbstract && !type.IsValueType;
    }
}