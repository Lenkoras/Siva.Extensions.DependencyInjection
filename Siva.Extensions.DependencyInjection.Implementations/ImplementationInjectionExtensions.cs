using Microsoft.Extensions.DependencyInjection;
using Siva.Extensions.DependencyInjection.Factories;
using Siva.Extensions.DependencyInjection.Implementations;
using Siva.Extensions.DependencyInjection.Providers;
using System.Data;
using System.Reflection;

namespace Siva.Extensions.DependencyInjection
{
    /// <summary>
    /// An extension class designed to add all implementations of an interface or class from a sequence of types or assembly to the <see cref="IServiceCollection"/>.
    /// </summary>
    public static partial class ImplementationInjectionExtensions
    {
        /// <summary>
        /// Selects all implementation types of the <paramref name="targetAssignableType"/> 
        /// from the specified collection of <paramref name="types"/> 
        /// and tries to add them to the collection of <paramref name="services"/> using the specified <paramref name="modificationBehavior"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the <paramref name="types"/>.</param>
        /// <param name="types">Types to scan.</param>
        /// <param name="modificationBehavior">The behavior of modifying a collection of <paramref name="services"/>.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            IEnumerable<Type> types,
            IServiceCollectionModificationBehavior modificationBehavior)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(targetAssignableType);
            ArgumentNullException.ThrowIfNull(types);
            ArgumentNullException.ThrowIfNull(modificationBehavior);

            IServiceTypeProvider serviceTypeProvider = new ServiceTypeProviderFactory().Create(targetAssignableType);

            IEnumerable<Type> implementationTypes = types.Where(IsNotValueImplementationType);

            foreach (Type implementationType in implementationTypes)
            {
                if (serviceTypeProvider.TryProvide(implementationType, out Type? serviceType))
                {
                    modificationBehavior.TryAdd(services,
                        serviceType: serviceType,
                        implementationType: implementationType);
                }
            }

            return services;
        }

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
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            IEnumerable<Type> types,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            return AddImplementations(services,
                targetAssignableType,
                types,
                new UniqueImplementationsModificationBehavior(lifetime));
        }

        /// <summary>
        /// Selects all implementation types of the <paramref name="targetAssignableType"/>
        /// that are taken from the specified <paramref name="assembly"/>
        /// and and tries to add them to the collection of <paramref name="services"/> using the specified <paramref name="modificationBehavior"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface,
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the exported types.</param>
        /// <param name="assembly">Assembly to scan. 
        /// By default takes all exported types from this assembly. 
        /// To override the default behavior, use the <paramref name="selector"/>.</param>
        /// <param name="modificationBehavior">The behavior of modifying a collection of <paramref name="services"/>.</param>
        /// <param name="selector">A selector for getting collection types from the specified <paramref name="assembly"/>.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            Assembly assembly,
            IServiceCollectionModificationBehavior modificationBehavior,
            Func<Assembly, IEnumerable<Type>>? selector = null)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            selector ??= GetExportedTypes;

            return AddImplementations(services, targetAssignableType, selector(assembly), modificationBehavior);
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
        /// <param name="assembly">Assembly to scan. 
        /// By default takes all exported types from this assembly. 
        /// To override the default behavior, use the <paramref name="selector"/>.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <param name="selector">A selector for getting collection types from the specified <paramref name="assembly"/>.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            Func<Assembly, IEnumerable<Type>>? selector = null)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            selector ??= GetExportedTypes;

            return AddImplementations(services, targetAssignableType, selector(assembly), lifetime);
        }

        /// <summary>
        /// Selects all implementation types of the <paramref name="targetAssignableType"/>
        /// that are taken from the specified <paramref name="assemblies"/> and 
        /// adds them to the collection of <paramref name="services"/>.
        /// If the <paramref name="targetAssignableType"/> is an interface, 
        /// then the service type will be the first interface that is assignable to the <paramref name="targetAssignableType"/> or this interface itself.
        /// If the <paramref name="targetAssignableType"/> is a class, then the service type will be its implementation.
        /// </summary>
        /// <param name="services">Collection of the <see cref="ServiceDescriptor"/>s.</param>
        /// <param name="targetAssignableType">Any non-sealed type that will be searched for among the exported types.</param>
        /// <param name="assemblies">Assemblies to scan. 
        /// By default takes all exported types from these assemblies. 
        /// To override the default behavior, use the <paramref name="selector"/>.</param>
        /// <param name="modificationBehavior">The behavior of modifying a collection of <paramref name="services"/>.</param>
        /// <param name="selector">A selector for getting collection types from the specified collection of <paramref name="assemblies"/>.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            IEnumerable<Assembly> assemblies,
            IServiceCollectionModificationBehavior modificationBehavior,
            Func<Assembly, IEnumerable<Type>>? selector = null)
        {
            ArgumentNullException.ThrowIfNull(assemblies);
            selector ??= GetExportedTypes;

            return AddImplementations(services,
                targetAssignableType,
                types: assemblies.SelectMany(assembly => selector(assembly)),
                modificationBehavior);
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
        /// <param name="assemblies">Assemblies to scan. 
        /// By default takes all exported types from these assemblies. 
        /// To override the default behavior, use the <paramref name="selector"/>.</param>
        /// <param name="lifetime">The <see cref="ServiceLifetime"/> of the service.</param>
        /// <param name="selector">A selector for getting collection types from the specified collection of <paramref name="assemblies"/>.</param>
        /// <returns>The same service collection to continue the call chain.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">Throws an exception if <see cref="Type.IsSealed"/> for <paramref name="targetAssignableType"/> returns <see langword="true"/>.</exception>
        public static IServiceCollection AddImplementations(this IServiceCollection services,
            Type targetAssignableType,
            IEnumerable<Assembly> assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Scoped,
            Func<Assembly, IEnumerable<Type>>? selector = null)
        {
            ArgumentNullException.ThrowIfNull(assemblies);
            selector ??= GetExportedTypes;

            return AddImplementations(services,
                targetAssignableType,
                types: assemblies.SelectMany(assembly => selector(assembly)),
                lifetime);
        }

        private static Type[] GetExportedTypes(Assembly assembly) =>
            assembly.GetExportedTypes();

        private static bool IsNotValueImplementationType(Type type) =>
            !type.IsInterface && !type.IsAbstract && !type.IsValueType;
    }
}