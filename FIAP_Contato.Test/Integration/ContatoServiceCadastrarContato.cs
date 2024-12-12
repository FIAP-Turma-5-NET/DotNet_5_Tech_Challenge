using Bogus;

using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using MySql.Data.MySqlClient;
using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(ContextCollection))]
    public class ContatoServiceCadastrarContato : IAsyncLifetime
    {
        private readonly ContextFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ContatoDomainService _contatoDomainService;

        public ContatoServiceCadastrarContato(ContextFixture fixture)
        {
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _contatoDomainService = new ContatoDomainService(_contatoRepository);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task CadastraContatoNoBancoComInformacoesCorretas()
        {
            // Arrange
            var contato = new ContatoDataBuilder().Build();    

            // Act
            var resultado = await _contatoDomainService.CadastrarContato(contato);
            var contatos = await _contatoDomainService.ObterTodosContatos();

            // Assert
            Assert.Equal("Cadastrado com sucesso!", resultado);  
            Assert.Contains(contatos, c => c.Nome == contato.Nome);
            Assert.Contains(contatos, c => c.Email == contato.Email);
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
