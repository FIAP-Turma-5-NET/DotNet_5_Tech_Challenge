using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [CollectionDefinition(nameof(RabbitMqCollection))]
    public class RabbitMqCollection : ICollectionFixture<RabbitMqFixture> { }
}
