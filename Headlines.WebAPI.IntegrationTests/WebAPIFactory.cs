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

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Headlines.WebAPI.Tests.Integration
{
    public sealed class WebAPIFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {

        private Mock<IDateTimeProvider>? _dateTimeProviderMock = null;
        private IObjectStorageWrapper? _objectStorageWrapperMock = null;

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
            MockObjectStorageWrapper(new List<(string, string, object)>());
        }
        public void MockObjectStorageWrapper(ICollection<(string Bucket, string Key, object Object)> data)
        {
            _objectStorageWrapperMock = new ObjectStorageWrapperMock(data);
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