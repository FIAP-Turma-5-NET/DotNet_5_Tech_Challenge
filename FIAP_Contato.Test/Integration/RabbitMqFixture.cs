using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class RabbitMqFixture : IAsyncLifetime
    {
        private readonly IContainer _rabbitMqContainer;
        public string ConnectionString { get; private set; }

        public RabbitMqFixture()
        {
            _rabbitMqContainer = new ContainerBuilder()
                .WithImage("rabbitmq:3-management")
                .WithName("rabbitmq-fiap-contato-test")
                .WithPortBinding(5672, true)
                .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
                .WithEnvironment("RABBITMQ_DEFAULT_PASS", "password")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5672))
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _rabbitMqContainer.StartAsync();
            var hostPort = _rabbitMqContainer.GetMappedPublicPort(5672);
            ConnectionString = $"amqp://admin:password@localhost:{hostPort}";
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqContainer.StopAsync();
        }
    }

}
