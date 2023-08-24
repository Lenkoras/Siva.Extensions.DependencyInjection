using Siva.Extensions.DependencyInjection.Providers;

namespace Siva.Extensions.DependencyInjection.Factories
{
    internal class ServiceTypeProviderFactory
    {
        public IServiceTypeProvider Create(Type type)
        {
            if (type.IsSealed)
            {
                throw new ArgumentException($"{nameof(type)} should be non-sealed class or interface.", nameof(type));
            }

            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    return new GenericInterfaceServiceTypeProvider(type); // here type is a generic interface 
                }
                return new InterfaceServiceTypeProvider(type); // here type is a regular interface
            }

            if (type.IsGenericType)
            {
                return new GenericClassServiceTypeProvider(type); // here type is a generic class, because value type is sealed
            }
            return new ClassServiceTypeProvider(type); // here serviceTypeToScan is a class
        }
    }
}
