using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class InfrastructureFixture : IAsyncLifetime
    {
        private readonly DatabaseFixture _mySqlFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public string MySqlConnectionString => _mySqlFixture.ConnectionString;
        public string RabbitMqConnectionString => _rabbitMqFixture.ConnectionString;
        

        public InfrastructureFixture()
        {
            _mySqlFixture = new DatabaseFixture();
            _rabbitMqFixture = new RabbitMqFixture();
        }

        public async Task InitializeAsync()
        {
            var mySqlTask = _mySqlFixture.InitializeAsync();
            var rabbitMqTask = _rabbitMqFixture.InitializeAsync();
           
            await Task.WhenAll(mySqlTask, rabbitMqTask);
        }

        public async Task DisposeAsync()
        {
            var disposeMySql = _mySqlFixture.DisposeAsync();
            var disposeRabbitMq = _rabbitMqFixture.DisposeAsync();
           
            await Task.WhenAll(disposeMySql, disposeRabbitMq);
        }

        public async Task ResetDatabaseAsync() => await _mySqlFixture.ResetDatabaseAsync();
    }

}
