
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
                switch (context.Message.TipoDeEvento)
                {
                    case "Cadastrar":
                        await _consumerService.CadastrarContato(context.Message);
                        break; // Finaliza o caso "Cadastrar".
                    case "Atualizar":
                        await _consumerService.AtualizarContato(context.Message);
                        break; // Finaliza o caso "Atualizar".
                    case "Deletar":
                        await _consumerService.DeletarContato(context.Message);
                        break; // Finaliza o caso "Deletar".
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex.Message}");
            }
        }
    }

}
