using Xunit;

namespace FIAP_Contato.Test.Integration;
[CollectionDefinition(nameof(ContextCollection))]
public class ContextCollection : ICollectionFixture<ContextFixture>
{
}

