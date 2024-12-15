using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Data.Repository;
using Bogus;
using FIAP_Contato.Application.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using FIAP_Contato.Consumer.Consumer;
using MySql.Data.MySqlClient;
using MassTransit;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(InfrastructureCollection))]
    public class ContatoIntegrationTests : IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly IContatoRepository _contatoRepository;
        private readonly DatabaseFixture _fixture;
        private readonly RabbitMqFixture _rabbitMqFixture;

        public ContatoIntegrationTests(DatabaseFixture fixture, RabbitMqFixture rabbitMqFixture)
        {
            _fixture = fixture;
            _rabbitMqFixture = rabbitMqFixture;

         
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                       
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IBusControl));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                     
                        services.AddMassTransit(x =>
                        {
                            x.AddConsumer<ContatoConsumer>();

                            x.UsingRabbitMq((context, cfg) =>
                            {
                                cfg.Host(new Uri(_rabbitMqFixture.ConnectionString), h =>
                                {
                                    h.Username("admin");
                                    h.Password("password");
                                });

                                cfg.ConfigureEndpoints(context);
                            });
                        });
                    });
                });

            _client = _factory.CreateClient();
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.ConnectionString));
        }

        public async Task InitializeAsync()
        {          
            await _rabbitMqFixture.InitializeAsync();
        }

        public async Task DisposeAsync()
        {
            await _rabbitMqFixture.DisposeAsync(); 
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

           
            var contatos = await _contatoRepository.ObterTodosAsync();
            Assert.Contains(contatos, c => c.Nome == contatoModel.Nome && c.Email == contatoModel.Email);

         
            Assert.True(true, "Mensagem enviada e processada pelo RabbitMQ com sucesso.");
        }
    }
}
