using Xunit;

namespace FIAP_Contato.Test.Integration;
[CollectionDefinition(nameof(InfrastructureCollection))]
public class InfrastructureCollection : ICollectionFixture<InfrastructureFixture>   
{ 
}