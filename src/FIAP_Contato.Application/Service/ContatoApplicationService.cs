using AutoMapper;
using FIAP_Contato.Application.Interface;
using FIAP_Contato.Application.Model;
using FIAP_Contato.Domain.Entity;
using FIAP_Contato.Domain.Interface;
using FIAP_Contato.Producer.Interface;

using Shared.Model;


namespace FIAP_Contato.Application.Service;

public class ContatoApplicationService : IContatoApplicationService
{
    private readonly IContatoDomainService _contatoDomainService;
    private IMapper _mapper;
    private readonly IContatoProducer _contatoProducer;

    public ContatoApplicationService(IContatoDomainService contatoDomainService, IMapper mapper, IContatoProducer contatoProducer)
    {
        _contatoDomainService = contatoDomainService;
        _mapper = mapper;
        _contatoProducer = contatoProducer;
    }

    public async Task<string> CadastrarContato(ContatoModel request)
    {
        var contato = _mapper.Map<Contato>(request);
        _contatoDomainService.TratarContato(contato);

        var contatoExiste = await _contatoDomainService.VerificarContatoExistente(contato);

        if (contatoExiste)
        {
            throw new InvalidOperationException("Contato já existe!");
        }

        var contatoMensagem = _mapper.Map<ContatoMensagem>(contato);
        contatoMensagem.TipoDeEvento = "Cadastrar";
        await _contatoProducer.EnviarContatoAsync(contatoMensagem);

        return "Contato cadastrado com sucesso!";
    }

    public async Task<string> AtualizarContato(int id, ContatoModel request)
    {
        var contato = _mapper.Map<Contato>(request);
        contato.Id = id;
        _contatoDomainService.TratarContato(contato);
        var contatoExiste = await _contatoDomainService.VerificarContatoExistentePorId(contato.Id);

        if (!contatoExiste)
        {
            throw new InvalidOperationException("Contato não existe para ser alterado!");
        }
        var contatoMensagem = _mapper.Map<ContatoMensagem>(contato);
        contatoMensagem.TipoDeEvento = "Atualizar";
        await _contatoProducer.EnviarContatoAsync(contatoMensagem);

        return "Contato atualizado com sucesso!";
    }   

    public async Task<string> DeletarContato(int id)
    {
        var contato = await _contatoDomainService.ObterDadosContatoPorId(id);

        if (contato == null)
        {
            throw new InvalidOperationException("Contato não existe para ser deletado!");
        }
        _contatoDomainService.TratarContato(contato);
        var contatoMensagem = _mapper.Map<ContatoMensagem>(contato);
        contatoMensagem.TipoDeEvento = "Deletar";
        await _contatoProducer.EnviarContatoAsync(contatoMensagem);

        return "Contato deletado com sucesso!";
    }

    public async Task<IEnumerable<ContatoModelResponse>> ObterTodosContatos(string? ddd)
    {
        var res = await _contatoDomainService.ObterTodosContatos(ddd);

        return _mapper.Map<IEnumerable<ContatoModelResponse>>(res);
    }
}
