using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Domain.Interface;

public interface IContatoDomainService
{
    Task<string> CadastrarContato(Contato request);
    Task<string> AtualizarContato(Contato request);
    Task<IEnumerable<Contato>> ObterTodosContatos(string? ddd);
    Task<string> DeletarContato(int id);
}