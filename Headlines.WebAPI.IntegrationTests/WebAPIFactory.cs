using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Headlines.DependencyResolution;
using Xunit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PBilek.Infrastructure.DatetimeProvider;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace Headlines.WebAPI.Tests.Integration
{
    public sealed class WebAPIFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private readonly MsSqlTestcontainer _dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
                .WithDatabase(new MsSqlTestcontainerConfiguration
                {
                    Password = "localdevpassword#123",                    
                })
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                .Build();

        private Mock<IDateTimeProvider>? _dateTimeProviderMock = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {            
            builder.ConfigureTestServices(services =>
            {
                services.RemoveORMDependencyGroup();
                services.AddORMDependencyGroup($"{_dbContainer.ConnectionString} TrustServerCertificate=true;");

                if (_dateTimeProviderMock != null)
                {
                    services.RemoveAll(typeof(IDateTimeProvider));
                    services.AddTransient<IDateTimeProvider>(c => _dateTimeProviderMock.Object);
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

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.StopAsync();
        }
    }
}