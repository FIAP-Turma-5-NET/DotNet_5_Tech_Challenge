using FIAP_Contato.Producer.Producers;
using FIAP_Contato.Test.Integration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FIAP_Contato.Producer.Interface;
using Shared.Model;


namespace FIAP_Contato.Test.Integration
{
    [Collection(nameof(RabbitMqCollection))]
    public class ContatoProducerIntegrationTests : IAsyncLifetime
    {
        private readonly RabbitMqFixture _rabbitMqFixture;
        private IBusControl _busControl;
        private ContatoProducer _contatoProducer;
        private TestConsumer _testConsumer;

        public ContatoProducerIntegrationTests(RabbitMqFixture rabbitMqFixture)
        {
            _rabbitMqFixture = rabbitMqFixture;
        }

        public async Task InitializeAsync()
        {
            // Configuração do MassTransit para testes usando o RabbitMQFixture
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                {
                    new KeyValuePair<string, string?>("MassTransit:Filas:ContatoFila", "contato-queue")
                })
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IProducerService, ProducerServiceMock>();

            // Configura MassTransit
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TestConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(_rabbitMqFixture.ConnectionString, h =>
                    {
                        h.Username("admin");
                        h.Password("password");
                    });

                    cfg.ReceiveEndpoint("contato-queue", e =>
                    {
                        e.ConfigureConsumer<TestConsumer>(context);
                    });
                });
            });


            var serviceProvider = services.BuildServiceProvider();

            // Inicializa os componentes
            _busControl = serviceProvider.GetRequiredService<IBusControl>();
            await _busControl.StartAsync();

            // Inicializa o produtor
            var producerService = serviceProvider.GetRequiredService<IProducerService>();
            _contatoProducer = new ContatoProducer(producerService, configuration);

            // Inicializa o consumidor
            _testConsumer = new TestConsumer();
        }

        public async Task DisposeAsync()
        {
            await _busControl.StopAsync();
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task Deve_EnviarMensagemParaFilaRabbitMQ()
        {
           
            var mensagem = new ContatoMensagem
            {
                Nome = "João Silva",
                Email = "joao.silva@teste.com",
                Telefone = "11999999999"
            };
        
            await _contatoProducer.EnviarContatoAsync(mensagem);

      
            var mensagemRecebida = await _testConsumer.MensagemRecebidaAsync(mensagem);

            Assert.True(mensagemRecebida, "Mensagem não foi recebida pela fila RabbitMQ.");
        }

        // Consumidor fictício para validar o recebimento da mensagem
        public class TestConsumer : IConsumer<ContatoMensagem>
        {

            private static ContatoMensagem _mensagemRecebida;

            public Task Consume(ConsumeContext<ContatoMensagem> context)
            {
                _mensagemRecebida = context.Message;

                Console.WriteLine($"Mensagem recebida: {context.Message.Nome}");
                return Task.CompletedTask;
            }

            public async Task<bool> MensagemRecebidaAsync(ContatoMensagem mensagemEsperada)
            {

                await Task.Delay(1000);

                var mensagemRecebida = ObterMensagemRecebida();

                return mensagemRecebida != null &&
                       mensagemRecebida.Nome == mensagemEsperada.Nome &&
                       mensagemRecebida.Email == mensagemEsperada.Email &&
                       mensagemRecebida.Telefone == mensagemEsperada.Telefone;
            }


            public static ContatoMensagem ObterMensagemRecebida()
            {
                return _mensagemRecebida;
            }
        }
    }

    // Mock do serviço produtor
    public class ProducerServiceMock : IProducerService
    {
        private readonly IBus _bus;

        public ProducerServiceMock(IBus bus)
        {
            _bus = bus;
        }

        public async Task EnviarMensagemAsync<T>(T mensagem, string queueName) where T : class
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(mensagem);
        }
    }
}

[CollectionDefinition(nameof(RabbitMqCollection))]
public class RabbitMqCollection : ICollectionFixture<RabbitMqFixture> { }
