using FIAP_Contato.Application.Mapper;
using FIAP_Contato.Consumer.Service;
using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;

using MySql.Data.MySqlClient;

using Shared.Model;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(DatabaseCollection))]
    public class ContatoServiceAtualizarContato : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ConsumerService _consumerService;
        private AutoMapper.IMapper _mapper;

        public ContatoServiceAtualizarContato(DatabaseFixture fixture)
        {
            _mapper = MapperConfiguration.RegisterMapping();
            _fixture = fixture;
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
            _consumerService = new ConsumerService(_contatoRepository, _mapper);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task AtualizarContatoEspecificoNoBanco()
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

            contatoAtualizado.Nome = "João Modificado";

            var cadastro = _mapper.Map<ContatoMensagem>(contatoAtualizado);

            
            var atualizarResultado = await _consumerService.AtualizarContato(cadastro);            

            // Assert
            Assert.Equal("Atualizado com sucesso!", atualizarResultado);         
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
