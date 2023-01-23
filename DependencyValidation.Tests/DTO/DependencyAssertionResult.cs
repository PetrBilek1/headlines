namespace DependencyValidation.Tests.DTO
{
    internal sealed class DependencyAssertionResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
    }
}