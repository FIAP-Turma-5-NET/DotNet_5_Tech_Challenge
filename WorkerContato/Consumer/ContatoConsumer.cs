
using FIAP_Contato.Consumer.Interface;

using MassTransit;

using Shared.Model;


namespace FIAP_Contato.Consumer.Consumer
{
  

    public class ContatoConsumer : IConsumer<ContatoMensagem>
    {
        private readonly IConsumerService _consumerService;

        public ContatoConsumer(IConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        public async Task Consume(ConsumeContext<ContatoMensagem> context)
        {
            try
            {  
                await _consumerService.CadastrarContato(context.Message);
            }
            catch (Exception ex)
            {          
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
            }
        }
    }

}
