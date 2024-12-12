using FIAP_Contato.Domain.Entity;

using Shared.Model;

namespace FIAP_Contato.Consumer.Interface
{
    public interface IConsumerService
    {
        Task<string> CadastrarContato(ContatoMensagem request);
    }
}
