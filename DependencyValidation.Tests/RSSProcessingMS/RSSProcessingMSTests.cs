using DependencyValidation.Tests.DTO;
using Headlines.RSSProcessingMicroService;
using Headlines.RSSProcessingMicroService.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using PBilek.RSSReaderService;
using Xunit;

namespace DependencyValidation.Tests.RSSProcessingMS
{
    public sealed class RSSProcessingMSTests
    {
        private readonly List<ValidationServiceDescriptor> _descriptors = new()
        {
            new ValidationServiceDescriptor
            {
                ServiceType = typeof(IRSSReaderService),
                ImplementationType = typeof(RSSReaderService),
                Lifetime = ServiceLifetime.Transient
            },
            new ValidationServiceDescriptor
            {
                ServiceType = typeof(IRSSSourceReaderService),
                ImplementationType = typeof(RSSSourceReaderService),
                Lifetime = ServiceLifetime.Scoped
            },
            new ValidationServiceDescriptor
            {
                ServiceType = typeof(IRSSProcessorService),
                ImplementationType = typeof(RSSProcessorService),
                Lifetime = ServiceLifetime.Scoped
            }
        };

        [Fact]
        public void RegistrationValidation()
        {
            var app = new WebApplicationFactory<ServiceWorker>()
                .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(serviceCollection =>
                {
                    var services = serviceCollection.ToList();
                    var result = TestUtils.ValidateServices(services, _descriptors);

                    if (!result.Success)
                    {
                        Assert.Fail(result.Message!);
                    }                    
                }));

            app.CreateClient();
        }
    }
}