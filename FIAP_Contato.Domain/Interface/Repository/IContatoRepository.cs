using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Domain.Interface.Repository;

public interface IContatoRepository : IRepositoryBase<Contato>
{
    Task<int> CadastrarScalarAsync(Contato contato);
    Task<bool> AtualizarScalarAsync(Contato contato);
    Task<bool> DeletarScalarAsync(int id);
}

