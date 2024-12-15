using Xunit;

namespace FIAP_Contato.Test.Integration;
[CollectionDefinition(nameof(DatabaseCollection))]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}