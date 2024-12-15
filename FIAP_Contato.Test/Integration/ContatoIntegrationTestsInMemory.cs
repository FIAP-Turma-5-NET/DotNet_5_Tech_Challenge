using FIAP_Contato.Domain.Interface.Repository;
using MassTransit;
using MassTransit.Testing;
using Xunit;
using FIAP_Contato.Data.Repository;
using MySql.Data.MySqlClient;
using Bogus;
using FIAP_Contato.Application.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Model;
using System.Net.Http.Json;
using FIAP_Contato.Consumer.Interface;
using FIAP_Contato.Consumer.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(DatabaseCollection))]
    public class ContatoIntegrationTestsInMemory : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private readonly IContatoRepository _contatoRepository;
        private readonly ITestHarness _harness;
        private readonly DatabaseFixture _fixture;

        public ContatoIntegrationTestsInMemory(DatabaseFixture fixture)
        {
            _factory = new WebApplicationFactory<Program>();
            _fixture = fixture;

            // Configuração do TestHarness usando a nova API
            var provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<ContatoConsumer>();
                })
                .BuildServiceProvider();
            _harness = provider.GetRequiredService<ITestHarness>();
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
        }

        public async Task InitializeAsync()
        {
            await _harness.Start(); // Inicia o harness
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _harness.Stop(); // Para o harness
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task Deve_CadastrarContato_EnviarFilaE_SalvarNoBanco()
        {
            // Arrange
            var fakerContatoModel = new Faker<ContatoModel>("pt_BR")
                .RuleFor(f => f.Nome, f => f.Name.FullName())
                .RuleFor(f => f.Telefone, f => f.Phone.PhoneNumberFormat())
                .RuleFor(f => f.Email, f => f.Internet.Email());
            var contatoModel = fakerContatoModel.Generate();

            // Act 
            var response = await _client.PostAsJsonAsync("/api/Contato", contatoModel);

            // Assert 
            response.EnsureSuccessStatusCode();
            var resultado = await response.Content.ReadAsStringAsync();
            Assert.Equal("Contato cadastrado com sucesso!", resultado);


            Assert.True(await _harness.Published.Any<ContatoMensagem>());


            var consumerHarness = _harness.GetConsumerHarness<ContatoConsumer>();
            Assert.True(await consumerHarness.Consumed.Any<ContatoMensagem>(), "O consumidor não consumiu a mensagem.");


            var contatos = await _contatoRepository.ObterTodosAsync();
            Assert.Contains(contatos, c => c.Nome == contatoModel.Nome && c.Email == contatoModel.Email);
        }
    }

    public class FakeConsumerService : IConsumerService
    {
        public Task<string> CadastrarContato(ContatoMensagem request)
        {
            return Task.FromResult("Cadastrado com sucesso!");
        }
    }
}

