using Dapper;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MySqlConnector;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class ContextFixture : IAsyncLifetime
    {
        private readonly IContainer _mySqlContainer;

        public string ConnectionString { get; private set; }

        public ContextFixture()
        {
            _mySqlContainer = new ContainerBuilder()
                .WithImage("mysql:8.0.32")
                .WithName("mysql-fiap-contato-test")
                .WithPortBinding(3306, true)
                .WithEnvironment("MYSQL_ROOT_PASSWORD", "202410")
                .WithEnvironment("MYSQL_DATABASE", "FIAPContato")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilPortIsAvailable(3306))
                .Build();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _mySqlContainer.StartAsync();

                var hostPort = _mySqlContainer.GetMappedPublicPort(3306);
                ConnectionString = $"Server=localhost;Port={hostPort};database=FIAPContato;uid=root;pwd=202410;AllowPublicKeyRetrieval=True;SslMode=None;";

                using var connection = new MySqlConnection(ConnectionString);
                await connection.OpenAsync();
                var createTableSql = @"
                    CREATE TABLE IF NOT EXISTS FIAPContato.Contato 
                    (
                        ID INT AUTO_INCREMENT PRIMARY KEY,
                        Nome VARCHAR(100) NOT NULL,
                        DDD VARCHAR(2) NOT NULL,
                        Telefone VARCHAR(15) NOT NULL,
                        Email VARCHAR(100) NOT NULL
                    );";
                await connection.ExecuteAsync(createTableSql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing MySQL container: {ex.Message}");
                throw;
            }
        }

        public async Task DisposeAsync()
        {
            await _mySqlContainer.StopAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync("TRUNCATE TABLE FIAPContato.Contato;");
        }
    }
}
