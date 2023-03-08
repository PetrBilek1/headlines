using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PBilek.Infrastructure.DatetimeProvider;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using PBilek.ORM.EntityFrameworkCore.SQL.DependencyResolution;
using Headlines.ORM.Core.Context;
using Headlines.WebAPI.DependencyResolution;
using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.WebAPI.Tests.Integration.V1.TestUtils;
using Headlines.BL.Abstractions.EventBus;
using Microsoft.Extensions.Caching.Distributed;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Headlines.WebAPI.Tests.Integration
{
    public sealed class WebAPIFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private Mock<IDateTimeProvider>? _dateTimeProviderMock = null;
        private IObjectStorageWrapper? _objectStorageWrapperMock = null;
        private Mock<IEventBus>? _eventBusMock = null;
        private Mock<IDistributedCache>? _cacheMock = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {            
            builder.ConfigureTestServices(services =>
            {   
                services.RemoveORMDependencyGroup<HeadlinesDbContext>();
                services.AddORMDependencyGroup<HeadlinesDbContext>(DatabaseProvisioner.GetConnectionString(Guid.NewGuid()));


                if (_dateTimeProviderMock != null)
                {
                    services.RemoveAll(typeof(IDateTimeProvider));
                    services.AddTransient<IDateTimeProvider>(c => _dateTimeProviderMock.Object);
                }
                
                if (_objectStorageWrapperMock != null)
                {
                    services.RemoveObjectStorageDependencyGroup();
                    services.AddTransient<IObjectStorageWrapper>(c => _objectStorageWrapperMock);
                }

                if (_eventBusMock != null)
                {
                    services.RemoveAll(typeof(IEventBus));
                    services.AddTransient<IEventBus>(c => _eventBusMock.Object);
                }

                if (_cacheMock != null)
                {
                    services.RemoveAll(typeof(IDistributedCache));
                    services.AddTransient<IDistributedCache>(c => _cacheMock.Object);
                }
            });
        }
        /// <summary>
        /// Must be called before CreateClient()
        /// </summary>
        /// <param name="now"></param>
        public void MockDateTimeProvider(DateTime now)
        {
            _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);

            _dateTimeProviderMock
                .Setup(x => x.Now)
                .Returns(now);
        }

        /// <summary>
        /// Must be called before CreateClient()
        /// </summary>
        public void MockObjectStorageWrapper()
        {
            _objectStorageWrapperMock = new ObjectStorageWrapperMock(new List<(string, string, object)>());

        }

        /// <summary>
        /// Must be called before CreateClient()
        /// </summary>
        public Mock<IEventBus> MockEventBus()
        {
            _eventBusMock = new Mock<IEventBus>(MockBehavior.Strict);

            return _eventBusMock;
        }

        public Mock<IDistributedCache> MockCache()
        {
            _cacheMock = new Mock<IDistributedCache>(MockBehavior.Strict);

            return _cacheMock;
        }

        public async Task InitializeAsync()
        {
            await DatabaseProvisioner.InitializeAsync();
        }

        public new Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}