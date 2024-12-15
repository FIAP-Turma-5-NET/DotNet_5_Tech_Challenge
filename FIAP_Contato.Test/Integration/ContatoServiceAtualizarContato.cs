using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using MySql.Data.MySqlClient;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(DatabaseCollection))]
    public class ContatoServiceAtualizarContato : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ContatoDomainService _contatoDomainService;

        public ContatoServiceAtualizarContato(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _contatoDomainService = new ContatoDomainService(_contatoRepository);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task AtualizarContatoEspecificoNoBanco()
        {
            // Arrange
            var contato = new ContatoDataBuilder().Build();    
            await _contatoDomainService.CadastrarContato(contato);          
            var contatoAtualizado = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Nome == contato.Nome);

            contatoAtualizado.Nome = "João Modificado";

            // Act
            var resultado = await _contatoDomainService.AtualizarContato(contatoAtualizado);

            var contatoAtualizadoBanco = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Id == contatoAtualizado.Id);

            // Assert
            Assert.Equal("Atualizado com sucesso!", resultado);
            Assert.Equal("João Modificado", contatoAtualizadoBanco.Nome);
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
