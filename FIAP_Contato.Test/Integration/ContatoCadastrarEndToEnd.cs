using FIAP_Contato.Producer.Producers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FIAP_Contato.Producer.Interface;
using Shared.Model;
using FIAP_Contato.Consumer.Service;
using FIAP_Contato.Data.Repository;
using FIAP_Contato.Domain.Interface.Repository;
using FIAP_Contato.Application.Mapper;
using MySqlConnector;

namespace FIAP_Contato.Test.Integration
{


    [Collection(nameof(InfrastructureCollection))]
    public class ContatoCadastrarEndToEnd : IAsyncLifetime
    {
       
        private IBusControl _busControl;
        private ContatoProducer _contatoProducer;
        private TestConsumer _testConsumer;
        private readonly InfrastructureFixture _fixture;
        private readonly IContatoRepository _contatoRepository;
        private readonly ConsumerService _consumerService;
        private AutoMapper.IMapper _mapper;

        public ContatoCadastrarEndToEnd(InfrastructureFixture fixture)
        {
            _mapper = MapperConfiguration.RegisterMapping();
            _fixture = fixture;
         
            _contatoRepository = new ContatoRepository(new MySqlConnection(_fixture.MySqlConnectionString));
            _consumerService = new ConsumerService(_contatoRepository, _mapper);
        }

        public async Task InitializeAsync()
        {
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
                    cfg.Host(_fixture.RabbitMqConnectionString, h =>
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


            _busControl = serviceProvider.GetRequiredService<IBusControl>();
            await _busControl.StartAsync();


            var producerService = serviceProvider.GetRequiredService<IProducerService>();
            _contatoProducer = new ContatoProducer(producerService, configuration);

            _testConsumer = new TestConsumer();
        }

        public async Task DisposeAsync()
        {
            await _busControl.StopAsync();
        }

        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task EnviaRecebeMensagemESalvaNoBanco()
        {
            var mensagem = new ContatoMensagem
            {
                Nome = "João Silva",
                DDD = "11",
                Telefone = "999999999",
                Email = "joao.silva@teste.com"
            };

            await _contatoProducer.EnviarContatoAsync(mensagem);

            var mensagemRecebida = await _testConsumer.MensagemRecebidaAsync(mensagem);

            // Assert consumiu a mensagem
            Assert.NotNull(mensagemRecebida);

            var resultado = await _consumerService.CadastrarContato(mensagem);        

            // Assert cadastrou no banco
            Assert.Equal("Cadastrado com sucesso!", resultado);  
        }

    
        public class TestConsumer : IConsumer<ContatoMensagem>
        {

            private static ContatoMensagem _mensagemRecebida;

            public Task Consume(ConsumeContext<ContatoMensagem> context)
            {
                _mensagemRecebida = context.Message;

                Console.WriteLine($"Mensagem recebida: {context.Message.Nome}");
                return Task.CompletedTask;
            }

            public async Task<ContatoMensagem> MensagemRecebidaAsync(ContatoMensagem mensagemEsperada)
            {
                await Task.Delay(1000);  
                return ObterMensagemRecebida();
            }


            public static ContatoMensagem ObterMensagemRecebida()
            {
                return _mensagemRecebida;
            }
        }
    }

   
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




