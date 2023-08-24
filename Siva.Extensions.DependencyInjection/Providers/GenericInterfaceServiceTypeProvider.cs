namespace Siva.Extensions.DependencyInjection.Providers
{
    internal class GenericInterfaceServiceTypeProvider : InterfaceServiceTypeProvider
    {
        public GenericInterfaceServiceTypeProvider(Type assignableType) : base(assignableType)
        {
            if (!assignableType.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"{nameof(assignableType)} should not be generic type definition.", nameof(assignableType));
            }
        }

        protected override Type? PrepareType(Type implementationType, Type[] interfaces) =>
            interfaces.FirstOrDefault(
                        type => type.IsGenericType &&
                            type.GetGenericTypeDefinition() == AssignableType);
    }
}
