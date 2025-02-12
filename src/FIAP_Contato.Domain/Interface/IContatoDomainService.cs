using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Domain.Interface;

public interface IContatoDomainService
{
    Task<string> CadastrarContato(Contato request);
    Task<string> AtualizarContato(Contato request);
    Task<IEnumerable<Contato>> ObterTodosContatos(string? ddd);
    Task<string> DeletarContato(int id);
    Task<bool> VerificarContatoExistente(Contato contato);
    Task<bool> VerificarContatoExistentePorId(int id);
    Task<Contato> ObterDadosContatoPorId(int id);
    void TratarContato(Contato contato);
}