using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Domain.Service;
using FIAP_Contato.Test.Infra;
using MySql.Data.MySqlClient;
using Dapper;
using Xunit;
using FIAP_Contato.Data.Repository;
using Assert = Xunit.Assert;

namespace FIAP_Contato.Test.Integration
{
    public class ContatoService : IClassFixture<DockerFixture>
    {
        private readonly DockerFixture _dockerFixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ContatoDomainService _contatoDomainService;      

        public ContatoService(DockerFixture dockerFixture)
        {
            _dockerFixture = dockerFixture;        

            using (var connection = new MySqlConnection(_dockerFixture.GetConnectionString()))
            {
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS FIAPContato.Contato 
                    (
                        ID INT AUTO_INCREMENT PRIMARY KEY,
                        Nome VARCHAR(100) NOT NULL,
                        DDD VARCHAR(2) NOT NULL,
                        Telefone VARCHAR(15) NOT NULL,
                        Email VARCHAR(100) NOT NULL
                    );
                ";
                connection.Execute(createTableQuery);
            }
           
            _contatoRepository = new ContatoRepository(new MySqlConnection(_dockerFixture.GetConnectionString()));
            
            _contatoDomainService = new ContatoDomainService(_contatoRepository);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task CadastrarContato_DeveCadastrarComSucesso()
        {
            // Arrange
            var contato = new Contato
            {
                Nome = "João Silva",
                DDD = "",
                Telefone = "(11) 91234-4321",
                Email = "joao.silva@gmail.com"
            };

            // Act
            var resultado = await _contatoDomainService.CadastrarContato(contato);

            // Assert
            Assert.Equal("Cadastrado com sucesso!", resultado);

            // Validando se o contato foi inserido no banco
            var contatos = await _contatoDomainService.ObterTodosContatos();
            Assert.Contains(contatos, c => c.Email == "joao.silva@gmail.com");
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task AtualizarContato_DeveAtualizarComSucesso()
        {
            // Arrange
            var contato = new Contato
            {
                Nome = "João Atualizado",
                DDD = "",
                Telefone = "(21) 99999-4682",
                Email = "joao.atualizado@gmail.com"
            };

            // Primeiro cadastrar um contato para depois atualizar
            await _contatoDomainService.CadastrarContato(contato);

            // Modificando dados do contato
            var contatoAtualizado = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Email == "joao.atualizado@gmail.com");

            contatoAtualizado.Nome = "João Modificado";

            // Act
            var resultado = await _contatoDomainService.AtualizarContato(contatoAtualizado);

            // Assert
            Assert.Equal("Atualizado com sucesso!", resultado);

            // Validar atualização no banco
            var contatoAtualizadoBanco = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Id == contatoAtualizado.Id);

            Assert.Equal("João Modificado", contatoAtualizadoBanco.Nome);
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task DeletarContato_DeveDeletarComSucesso()
        {
            // Arrange
            var contato = new Contato
            {
                Nome = "Carlos Deletado",
                DDD = "",
                Telefone = "(31) 90234-4682",
                Email = "carlos.deletado@gmail.com"
            };

            // Cadastrar o contato para deletá-lo
            await _contatoDomainService.CadastrarContato(contato);
            var contatoParaDeletar = (await _contatoDomainService.ObterTodosContatos())
                .FirstOrDefault(c => c.Email == "carlos.deletado@gmail.com");

            // Act
            var resultado = await _contatoDomainService.DeletarContato(contatoParaDeletar.Id);

            // Assert
            Assert.Equal("Deletado com sucesso!", resultado);

            // Verificar se foi removido do banco
            var contatosRestantes = await _contatoDomainService.ObterTodosContatos();
            Assert.DoesNotContain(contatosRestantes, c => c.Email == "carlos.deletado@gmail.com");
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task ObterTodosContatos_DeveRetornarListaContatos()
        {
            // Arrange
            var contato1 = new Contato
            {
                Nome = "Maria Contato",
                DDD = "",
                Telefone = "(99) 20035-4682",
                Email = "maria.contato@gmail.com"
            };
            var contato2 = new Contato
            {
                Nome = "José Contato",
                DDD = "",
                Telefone = "(99) 20035-4682",
                Email = "jose.contato@gmail.com"
            };  

            var contato3 = new Contato
            {
                Nome = "João Contato",
                DDD = "",
                Telefone = "(41) 20035-4682",
                Email = "joao.contato@gmail.com"
            };

            await _contatoDomainService.CadastrarContato(contato1);
            await _contatoDomainService.CadastrarContato(contato2);
            await _contatoDomainService.CadastrarContato(contato3);

            // Act
            var contatos = await _contatoDomainService.ObterTodosContatos("99");

            // Assert
            Assert.NotEmpty(contatos);
            Assert.Equal(2, contatos.Count());
        }  
    }


}
