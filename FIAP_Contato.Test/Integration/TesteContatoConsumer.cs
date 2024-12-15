using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model;

using Xunit;

namespace FIAP_Contato.Test.Integration
{
    public class TesteContatoConsumer : IConsumer<ContatoMensagem>
    {
       
        public static List<ContatoMensagem> MensagensRecebidas = new List<ContatoMensagem>();

        public TesteContatoConsumer()
        {
       
        }

        public async Task Consume(ConsumeContext<ContatoMensagem> context)
        {
            // Registrar a mensagem recebida
            MensagensRecebidas.Add(context.Message);     

            // Simular processamento
            await Task.CompletedTask;
        }
        

        // Teste adicional para verificar detalhes da mensagem recebida
        [Fact]
        public async Task Deve_VerificarDetalhesDoContatoNaMensagemRecebida()
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<TesteContatoConsumer>();
            });

            var provider = services.BuildServiceProvider();
            var harness = provider.GetRequiredService<ITestHarness>();

            await harness.Start();

            try
            {
                var contatoMensagem = new ContatoMensagem
                {
                    Nome = "João Silva",
                    Email = "joao.silva@exemplo.com",
                    Telefone = "11988887777",
                    TipoDeEvento = "Cadastrar"
                };

                // Act
                await harness.Bus.Publish(contatoMensagem);

                // Assert
                var consumerHarness = harness.GetConsumerHarness<TesteContatoConsumer>();
                Assert.True(await consumerHarness.Consumed.Any<ContatoMensagem>(),
                    "Mensagem não foi consumida");

                // Verificar detalhes específicos da mensagem
                var mensagensRecebidas = TesteContatoConsumer.MensagensRecebidas;
                Assert.Single(mensagensRecebidas);

                var mensagemRecebida = mensagensRecebidas.First();
                Assert.Equal("João Silva", mensagemRecebida.Nome);
                Assert.Equal("joao.silva@exemplo.com", mensagemRecebida.Email);
                Assert.Equal("11988887777", mensagemRecebida.Telefone);
                Assert.Equal("Cadastrar", mensagemRecebida.TipoDeEvento);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
