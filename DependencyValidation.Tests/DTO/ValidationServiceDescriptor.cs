namespace DependencyValidation.Tests.DTO
{
    internal sealed class ValidationServiceDescriptor
    {
        public Type? ServiceType { get; init; }
        public Type? ImplementationType { get; init; }
        public ServiceLifetime Lifetime { get; init; }
    }
}