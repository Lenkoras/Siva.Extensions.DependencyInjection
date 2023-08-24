namespace Siva.Extensions.DependencyInjection.Providers
{
    internal class ClassServiceTypeProvider : BaseServiceTypeProvider
    {
        public ClassServiceTypeProvider(Type assignableType) : base(assignableType)
        {
            if (assignableType.IsValueType || assignableType.IsInterface)
            {
                throw new ArgumentException($"{nameof(assignableType)} should be a class.", nameof(assignableType));
            }
        }

        protected override Type? Provide(Type implementationType) =>
            implementationType.IsAssignableTo(AssignableType) ?
                implementationType :
                null;
    }
}
