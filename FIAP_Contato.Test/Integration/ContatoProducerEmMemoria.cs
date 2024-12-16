using FIAP_Contato.Consumer.Consumer;

using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model;


using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class ContatoProducerEmMemoria
    {
        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task EnviaRecebeMensagemEmMemoria()
        {
            // Arrange
            var services = new ServiceCollection();           
            services.AddMassTransitTestHarness(cfg =>
            {
               
                cfg.AddConsumer<ContatoConsumerEmMemoria>();
            });
            
            var provider = services.BuildServiceProvider();           
            var harness = provider.GetRequiredService<ITestHarness>();          
            await harness.Start();

            try
            {
                
                var contatoMensagem = new ContatoMensagem
                {
                    Nome = "Teste Integração",
                    Email = "teste@integracao.com",
                    Telefone = "11999999999",
                    TipoDeEvento = "Cadastrar"
                };
                
                await harness.Bus.Publish(contatoMensagem);
                var result = await harness.Published.Any<ContatoMensagem>();
                
                Assert.True(result, "Mensagem não foi publicada");
                
                var consumerHarness = harness.GetConsumerHarness<ContatoConsumerEmMemoria>();
                var consumo = await consumerHarness.Consumed.Any<ContatoMensagem>();

                Assert.True(consumo,"Mensagem não foi consumida");
            }
            finally
            {                
                await harness.Stop();
            }
        }

    }
}
