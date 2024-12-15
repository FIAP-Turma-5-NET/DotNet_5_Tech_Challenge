using FIAP_Contato.Consumer.Consumer;

using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model;


using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class ContatoMensageriaInMemoryTests
    {
        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task Deve_EnviarEReceberMensagemNoRabbitMQ_EmMemoria()
        {
            // Arrange
            var services = new ServiceCollection();

            // Configurar MassTransit para usar InMemory
            services.AddMassTransitTestHarness(cfg =>
            {
                // Configurar um consumidor de teste
                cfg.AddConsumer<TesteContatoConsumer>();
            });

            // Construir o provider de serviços
            var provider = services.BuildServiceProvider();

            // Obter o Test Harness
            var harness = provider.GetRequiredService<ITestHarness>();

            // Iniciar o harness
            await harness.Start();

            try
            {
                // Criar uma mensagem de contato de teste
                var contatoMensagem = new ContatoMensagem
                {
                    Nome = "Teste Integração",
                    Email = "teste@integracao.com",
                    Telefone = "11999999999",
                    TipoDeEvento = "Cadastrar"
                };

                // Publicar a mensagem
                await harness.Bus.Publish(contatoMensagem);

                var result = await harness.Published.Any<ContatoMensagem>();

                // Aguardar o processamento da mensagem
                Assert.True(result, "Mensagem não foi publicada");

                // Verificar se o consumidor recebeu a mensagem
                var consumerHarness = harness.GetConsumerHarness<TesteContatoConsumer>();
                var consumo = await consumerHarness.Consumed.Any<ContatoMensagem>();

                Assert.True(consumo,"Mensagem não foi consumida");
            }
            finally
            {
                // Parar o harness
                await harness.Stop();
            }
        }

    }
}
