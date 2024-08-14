using Dapper.Contrib.Extensions;
using FIAP_Contato.Data.Context;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using System.Data;

namespace FIAP_Contato.Data.Repository;

public class ContatoRepository : Context<Contato>, IContatoRepository
{
    private IDbConnection _dbConnection;
    public ContatoRepository(IDbConnection context) : base(context) { }

    public async Task<bool> AtualizarAsync(Contato entity) => await _context.UpdateAsync(entity);

    public async Task<int> CadastrarAsync(Contato entity) => await _context.InsertAsync(entity);

    public async Task<bool> DeletarAsync(Contato entity) => await _context.DeleteAsync(entity);

    public async Task<IEnumerable<Contato>> ObterTodosAsync() => await _context.GetAllAsync<Contato>();
}