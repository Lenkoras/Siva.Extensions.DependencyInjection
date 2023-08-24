using System.Diagnostics.CodeAnalysis;

namespace Siva.Extensions.DependencyInjection.Providers
{
    internal class InterfaceServiceTypeProvider : BaseServiceTypeProvider
    {
        public InterfaceServiceTypeProvider(Type assignableType) : base(assignableType)
        {
            if (!assignableType.IsInterface)
            {
                throw new ArgumentException($"{nameof(assignableType)} should be an interface", nameof(assignableType));
            }
        }

        protected override Type? Provide(Type implementationType)
        {
            Type[] interfaces = implementationType.GetInterfaces();

            Type? targetInterfaceType = PrepareType(implementationType, interfaces); // confirms that targetInterfaceType is an implementation of the AssignableType

            if (targetInterfaceType is not null && implementationType.IsAssignableTo(targetInterfaceType))
            {
                // tries to get an interface inherited from the targetInterfaceType or itself
                return GetServiceType(interfaces, targetInterfaceType);
            }
            return null;
        }

        protected virtual Type? PrepareType(Type implementationType, Type[] interfaces)
        {
            return implementationType.IsAssignableTo(AssignableType) ?
                        AssignableType :
                    null;
        }

        [return: NotNullIfNotNull(nameof(targetAssignableType))]
        private static Type GetServiceType(Type[] types, Type targetAssignableType) =>
            types.FirstOrDefault(
                        type => type != targetAssignableType &&
                            type.IsAssignableTo(targetAssignableType))
                        ?? targetAssignableType;
    }
}
