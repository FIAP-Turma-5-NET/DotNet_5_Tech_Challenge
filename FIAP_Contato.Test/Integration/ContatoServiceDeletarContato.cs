using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Consumer.Service;
using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface.Repository;
using MySql.Data.MySqlClient;

using Shared.Model;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(DatabaseCollection))]
    public class ContatoServiceDeletarContato : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ConsumerService _consumerService;
        private AutoMapper.IMapper _mapper;

        public ContatoServiceDeletarContato(DatabaseFixture fixture)
        {
            _mapper = MapperConfiguration.RegisterMapping();
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _consumerService = new ConsumerService(_contatoRepository, _mapper);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task DeletarContatoEspecificoNoBanco()
        {
            // Arrange

            var mensagem = new ContatoMensagem
            {
                Nome = "João Silva",
                DDD = "11",
                Telefone = "999999999",
                Email = "joao.silva@teste.com"
            };

            // Act

            var cadastrarResultado = await _consumerService.CadastrarContato(mensagem);

            var contatos = await _contatoRepository.ObterTodosAsync();

            var contatoAtualizado = contatos
                .FirstOrDefault(c => c.Nome == mensagem.Nome);            

            var cadastro = _mapper.Map<ContatoMensagem>(contatoAtualizado);


            var deletarResultado = await _consumerService.DeletarContato(cadastro);

            // Assert
            Assert.Equal("Deletado com sucesso!", deletarResultado);
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
