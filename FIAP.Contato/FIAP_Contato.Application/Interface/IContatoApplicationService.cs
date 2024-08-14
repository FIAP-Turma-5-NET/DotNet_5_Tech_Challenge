using FIAP_Contato.Application.Model;

namespace FIAP_Contato.Application.Interface;

public interface IContatoApplicationService
{
    Task<string> CadastrarContato(ContatoModel request);
    Task<string> AtualizarContato(int id, ContatoModel request);
    Task<IEnumerable<ContatoModelResponse>> ObterTodosContatos(string? ddd);
    Task<string> DeletarContato(int id);
}