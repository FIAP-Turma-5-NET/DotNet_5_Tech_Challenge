using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using MySql.Data.MySqlClient;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(ContextCollection))]
    public class ContatoServiceObterTodosOsContatos : IAsyncLifetime
    {
        private readonly ContextFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ContatoDomainService _contatoDomainService;

        public ContatoServiceObterTodosOsContatos(ContextFixture fixture)
        {
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _contatoDomainService = new ContatoDomainService(_contatoRepository);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task ObterTodosContatos_DeveRetornarListaContatos()
        {
            // Arrange
            var contatos = new ContatoDataBuilder().BuildList(10);

            foreach (var contato in contatos)
            {
                await _contatoDomainService.CadastrarContato(contato);
            }

            // Act
            var contatosObtidos = await _contatoDomainService.ObterTodosContatos();

            // Assert
            Assert.NotEmpty(contatosObtidos);
            Assert.Equal(10, contatosObtidos.Count());
        }

        Task IAsyncLifetime.InitializeAsync()
        {
           return Task.CompletedTask;
        }

        Task IAsyncLifetime.DisposeAsync()
        {
            return _fixture.ResetDatabaseAsync();
        }
    }
}
