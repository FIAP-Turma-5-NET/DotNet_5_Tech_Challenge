using FIAP_Contato.Domain.Entity;

namespace FIAP_Contato.Domain.Interface.Repository;

public interface IRepositoryBase<T> where T : EntityBase
{
    Task<int> CadastrarAsync(T entity);
    Task<bool> AtualizarAsync(T entity);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<bool> DeletarAsync(T entity);
}