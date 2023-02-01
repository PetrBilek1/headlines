using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
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
        private readonly string _dbPassword;
        private readonly int _dbPort;
        private readonly TestcontainersContainer _dbContainer;

        private Mock<IDateTimeProvider>? _dateTimeProviderMock = null;

        public WebAPIFactory()
        {
            _dbPassword = "Aa123456";
            _dbPort = new Random().Next(50000, 59999);

            _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithName($"mssql-fts-ha-{Guid.NewGuid()}")
                .WithImage("ghcr.io/petrbilek1/mssql-fts-ha:latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("MSSQL_SA_PASSWORD", _dbPassword)
                .WithEnvironment("MSSQL_TCP_PORT", _dbPort.ToString())
                .WithPortBinding(_dbPort)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(_dbPort))
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {            
            builder.ConfigureTestServices(services =>
            {   
                services.RemoveORMDependencyGroup();
                services.AddORMDependencyGroup($"Data Source={_dbContainer.Hostname},{_dbPort}; Initial Catalog=Catalog; User Id=sa; Password={_dbPassword}; TrustServerCertificate=true;");

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