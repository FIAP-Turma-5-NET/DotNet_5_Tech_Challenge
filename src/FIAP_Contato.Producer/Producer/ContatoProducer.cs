using FIAP_Contato.Producer.Interface;
using Shared.Model;

namespace FIAP_Contato.Producer.Producers;

public class ContatoProducer: IContatoProducer
{
    private readonly IProducerService _producerService;
    private readonly string _queueName;

    public ContatoProducer(IProducerService producerService)
    {
        _producerService = producerService;
        _queueName = Environment.GetEnvironmentVariable("MassTransit_Filas_ContatoFila") ?? string.Empty;
    }

    public async Task EnviarContatoAsync(ContatoMensagem mensagem)
    {  
        await _producerService.EnviarMensagemAsync(mensagem, _queueName + "-" + mensagem.TipoDeEvento);
    }
}