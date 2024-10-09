using Dapper;
using Dapper.Contrib.Extensions;

using FIAP_Contato.Data.Context;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface.Repository;
using System.Data;

namespace FIAP_Contato.Data.Repository;

public class ContatoRepository(IDbConnection context) : Context<Contato>(context), IContatoRepository
{
    public async Task<int> CadastrarScalarAsync(Contato contato) {

        var parametros = new DynamicParameters();
        parametros.Add("@Nome", contato.Nome);
        parametros.Add("@DDD", contato.DDD);
        parametros.Add("@Telefone", contato.Telefone);
        parametros.Add("@Email", contato.Email);

        var sql = @"INSERT INTO FIAPContato.Contato (Nome, DDD, Telefone, Email) 
                VALUES (@Nome, @DDD, @Telefone, @Email);
                SELECT LAST_INSERT_ID();";

        var id = await context.ExecuteScalarAsync<int>(sql, parametros);

        return id;  

    }
}