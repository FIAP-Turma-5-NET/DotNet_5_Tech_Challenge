using FIAP_Contato.Producer.Interface;
using Microsoft.Extensions.Configuration;

using Shared.Model;


namespace FIAP_Contato.Producer.Producers
{
    public class ContatoProducer: IContatoProducer
    {
        private readonly IProducerService _producerService;
        private readonly string _queueName;

        public ContatoProducer(IProducerService producerService, IConfiguration configuration)
        {
            _producerService = producerService;
           
            _queueName = configuration["MassTransit:Filas:ContatoFila"] ?? throw new ArgumentNullException("Nome da fila de contato não configurado.");
        }

        public async Task EnviarContatoAsync(ContatoMensagem mensagem)
        {  
            await _producerService.EnviarMensagemAsync(mensagem, _queueName);
        }
    }
}
