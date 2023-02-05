using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Headlines.WebAPI.Tests.Integration
{
    //using this solution because of performance reasons
    public static class DatabaseProvisioner
    {
        private static ProvisionerCore? _core;
        private static Dictionary<Guid, string>? _provisions;

        public static async Task InitializeAsync()
        {
            if (_core != null)
                return;

            _core = new ProvisionerCore("Aa123456", 1433);
            _provisions = new Dictionary<Guid, string>();

            await _core.DbContainer.StartAsync();
        }

        public static string GetConnectionString(Guid key)
        {
            if (_core == null || _provisions == null)
                throw new Exception("Provisioner not initialized.");

            if (!_provisions.TryGetValue(key, out string? connectionString))
            {
                connectionString = $"Data Source={_core.DbContainer.Hostname},{_core.DbPort}; Initial Catalog={key}; User Id=sa; Password={_core.DbPassword}; TrustServerCertificate=true;";
                _provisions.Add(key, connectionString);
            }

            return connectionString;
        }

        private sealed class ProvisionerCore
        {
            public string DbPassword { get; init; }
            public int DbPort { get; init; }
            public TestcontainersContainer DbContainer { get; init; }

            public ProvisionerCore(string password, int port)
            {
                DbPassword = password;
                DbPort = port;
                DbContainer = new TestcontainersBuilder<TestcontainersContainer>()
                .WithName($"mssql-fts-ha-{Guid.NewGuid()}")
                .WithImage("ghcr.io/petrbilek1/mssql-fts-ha:latest")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .WithEnvironment("MSSQL_SA_PASSWORD", DbPassword)
                .WithEnvironment("MSSQL_TCP_PORT", DbPort.ToString())
                .WithPortBinding(DbPort)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(DbPort))
                .Build();
            }
        }
    }
}