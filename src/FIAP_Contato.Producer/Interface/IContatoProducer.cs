using Shared.Model;

namespace FIAP_Contato.Producer.Interface
{
    public interface IContatoProducer
    {
        Task EnviarContatoAsync(ContatoMensagem mensagem);
    }
}
