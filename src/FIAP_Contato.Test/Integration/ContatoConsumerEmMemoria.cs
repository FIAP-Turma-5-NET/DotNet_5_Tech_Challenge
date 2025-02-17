﻿using MassTransit.Testing;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model;

using Xunit;
using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Test.Integration
{
    public class ContatoConsumerEmMemoria : IConsumer<ContatoMensagem>
    {
       
        public static List<ContatoMensagem> MensagensRecebidas = new List<ContatoMensagem>();

        public ContatoConsumerEmMemoria()
        {
       
        }

        public async Task Consume(ConsumeContext<ContatoMensagem> context)
        {
          
            MensagensRecebidas.Add(context.Message);          
            await Task.CompletedTask;
        }
        

        
        [Fact]
        [Trait("Categoria", "Integration")]
        public async Task VerificaDetalhesDoContatoNaMensagemRecebida()
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
                    Nome = "João Silva Santos",
                    Email = "joao.silva@exemplo.com",
                    Telefone = "11988887777",
                    TipoDeEvento = "Cadastrar"
                };

                // Act
                await harness.Bus.Publish(contatoMensagem);

                // Assert
                var consumerHarness = harness.GetConsumerHarness<ContatoConsumerEmMemoria>();
                Assert.True(await consumerHarness.Consumed.Any<ContatoMensagem>(),
                    "Mensagem não foi consumida");
              
                var mensagensRecebidas = MensagensRecebidas;             

                await Task.Delay(5000);

                var mensagemRecebida = mensagensRecebidas.FirstOrDefault(c => c.Nome == contatoMensagem.Nome);
                Assert.Equal("João Silva Santos", mensagemRecebida?.Nome);
                Assert.Equal("joao.silva@exemplo.com", mensagemRecebida?.Email);
                Assert.Equal("11988887777", mensagemRecebida?.Telefone);
                Assert.Equal("Cadastrar", mensagemRecebida?.TipoDeEvento);
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
