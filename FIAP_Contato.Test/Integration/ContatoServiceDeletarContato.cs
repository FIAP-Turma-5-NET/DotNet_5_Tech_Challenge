using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using MySql.Data.MySqlClient;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(DatabaseCollection))]
    public class ContatoServiceDeletarContato : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ContatoDomainService _contatoDomainService;

        public ContatoServiceDeletarContato(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _contatoDomainService = new ContatoDomainService(_contatoRepository);
        }

       

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task DeletaContatoEspecificoNoBanco()
        {
            // Arrange
            var contato = new ContatoDataBuilder().Build();  

            await _contatoDomainService.CadastrarContato(contato);
            var contatoParaDeletar = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Nome == contato.Nome);

            // Act
            var resultado = await _contatoDomainService.DeletarContato(contatoParaDeletar.Id);
            var contatosRestantes = await _contatoDomainService.ObterTodosContatos();

            // Assert
            Assert.Equal("Deletado com sucesso!", resultado);            
            Assert.DoesNotContain(contatosRestantes, c => c.Nome == contato.Nome);
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
