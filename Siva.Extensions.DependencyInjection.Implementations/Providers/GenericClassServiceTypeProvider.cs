namespace Siva.Extensions.DependencyInjection.Providers
{
    internal class GenericClassServiceTypeProvider : ClassServiceTypeProvider
    {
        public GenericClassServiceTypeProvider(Type assignableType) : base(assignableType)
        {
            if (!assignableType.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"{nameof(assignableType)} should be generic type defenition.", nameof(assignableType));
            }
        }

        protected override Type? Provide(Type implementationType)
        {
            Type? currentTypeToCompare = implementationType;
            do
            {
                if (currentTypeToCompare.IsGenericType && currentTypeToCompare.GetGenericTypeDefinition() == AssignableType)
                {
                    return currentTypeToCompare;
                }
                else
                {
                    currentTypeToCompare = currentTypeToCompare.BaseType;
                }
            }
            while (currentTypeToCompare is not null);

            return null;
        }
    }
}
