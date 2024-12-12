using FIAP_Contato.Producer.Interface;

using MassTransit;

namespace FIAP_Contato.Infrastructure.Producer.Service
{
    public class ProducerService : IProducerService
    {
        private readonly IBus _bus;       

        public ProducerService(IBus bus)
        {
            _bus = bus;          
        }        

        public async Task EnviarMensagemAsync<T>(T mensagem, string queueName) where T : class
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await endpoint.Send(mensagem);           

            await Task.CompletedTask;
        }
    }
}
