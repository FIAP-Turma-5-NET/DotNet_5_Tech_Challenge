
namespace FIAP_Contato.Producer.Interface
{
    public interface IProducerService
    {
        Task EnviarMensagemAsync<T>(T mensagem, string queueName) where T : class;
    }
}
