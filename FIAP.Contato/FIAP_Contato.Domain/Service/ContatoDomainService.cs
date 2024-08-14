using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface;
using FIAP_Contato.Domain.Interface.Repository;

namespace FIAP_Contato.Domain.Service;

public class ContatoDomainService : IContatoDomainService
{
    private readonly IContatoRepository _contatoRepository;

    public ContatoDomainService(IContatoRepository contatoRepository) =>_contatoRepository = contatoRepository;

    public async Task<string> AtualizarContato(Contato request)
    {
        var response = string.Empty;
        var entity = _contatoRepository.ObterTodosAsync().Result.FirstOrDefault(c => c.Id.Equals(request.Id));

        if (entity != null)
        {
            request.TratarTelefone(request.Telefone);
            var res = await _contatoRepository.AtualizarAsync(request);

            if (res)
                response = "Atualizado com sucesso!";
            else
                throw new InvalidOperationException("Erro ao Atualizar!");
        }
        else 
        {
            throw new InvalidOperationException("Contato não existe!");
        }
        return response;
    }

    public async Task<string> CadastrarContato(Contato request)
    {
        var response = string.Empty;

        var entity = _contatoRepository.ObterTodosAsync().Result
            .FirstOrDefault(c => c.Nome.Equals(request.Nome, StringComparison.CurrentCultureIgnoreCase) && c.Email.Equals(request.Email));

        if (entity == null)
        {
            request.TratarTelefone(request.Telefone);
            var res = await _contatoRepository.CadastrarAsync(request);

            if (res == 0)
                response = "Cadastrado com sucesso!";
            else
                throw new InvalidOperationException("Erro ao Cadastrar!");
        }
        else 
        {
            throw new InvalidOperationException("Contato já existe!");
        }

        return response;
    }

    public async Task<string> DeletarContato(int id)
    {
        var response = string.Empty;

        var entity = ObterTodosContatos().Result.FirstOrDefault(c => c.Id.Equals(id));

        if (entity != null)
        {
            var res = await _contatoRepository.DeletarAsync(entity);

            if (res)
                response = "Deletado com sucesso!";
            else
                throw new InvalidOperationException("Erro ao Deletar!");
        }
        else 
        {
            throw new InvalidOperationException("Contato não existe!");
        }
        
        return response;
    }

    public async Task<IEnumerable<Contato>> ObterTodosContatos(string? ddd = null)
    {
        var resp = await _contatoRepository.ObterTodosAsync();

        if (!string.IsNullOrEmpty(ddd))
            resp = resp.Where(r => r.DDD.Equals(ddd));

        return resp;
    }
}