using AutoMapper;
using FIAP_Contato.Application.Interface;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface;

namespace FIAP_Contato.Application.Service;

public class ContatoApplicationService : IContatoApplicationService
{
    private readonly IContatoDomainService _contatoDomainService;
    private IMapper _mapper;

    public ContatoApplicationService(IContatoDomainService contatoDomainService, IMapper mapper)
    {
        _contatoDomainService = contatoDomainService;
        _mapper = mapper;
    }
    
    public async Task<string> AtualizarContato(int id, ContatoModel request)
    {
        var req = _mapper.Map<Contato>(request);
        req.Id = id;

        return await _contatoDomainService.AtualizarContato(req);
    }

    public async Task<string> CadastrarContato(ContatoModel request)
    {
        var req = _mapper.Map<Contato>(request);
        return await _contatoDomainService.CadastrarContato(req);
    }

    public async Task<string> DeletarContato(int id)
    {
        return await _contatoDomainService.DeletarContato(id);
    }

    public async Task<IEnumerable<ContatoModelResponse>> ObterTodosContatos(string? ddd)
    {
        var res = await _contatoDomainService.ObterTodosContatos(ddd);

        return _mapper.Map<IEnumerable<ContatoModelResponse>>(res);
    }
}
