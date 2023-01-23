using DependencyValidation.Tests.DTO;
using System.Text;

namespace DependencyValidation.Tests
{
    internal static class TestUtils
    {
        public static DependencyAssertionResult ValidateServices(List<ServiceDescriptor> services, List<ValidationServiceDescriptor> descriptors)
        {
            var searchFailed = false;
            var failedText = new StringBuilder();

            foreach (var descriptor in descriptors)
            {
                var match = services.SingleOrDefault(x =>
                    x.ServiceType == descriptor.ServiceType &&
                    x.ImplementationType == descriptor.ImplementationType &&
                    x.Lifetime == descriptor.Lifetime);

                if (match is not null)
                    continue;

                if (!searchFailed)
                {
                    failedText.AppendLine("Failed to find registered service for:");
                    searchFailed = true;
                }

                failedText.AppendLine($"{descriptor.ServiceType!.Name}|{descriptor.ImplementationType?.Name}|{descriptor.Lifetime}");
            }

            return new DependencyAssertionResult
            {
                Success = !searchFailed,
                Message = failedText.ToString()
            };
        }
    }
}