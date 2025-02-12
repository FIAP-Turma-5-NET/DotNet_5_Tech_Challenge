using FIAP_Contato.Application.Interface;
using FIAP_Contato.Application.Model;
using Microsoft.AspNetCore.Mvc;

namespace FIAP_Contato.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContatoController : ControllerBase
{
    private IContatoApplicationService _contatoApplicationService;

    public ContatoController(IContatoApplicationService contatoApplicationService) => _contatoApplicationService = contatoApplicationService;

    /// <summary>
    /// Cadastrar um contato
    /// </summary>
    /// <param name="request">Informação do contato</param>
    /// <returns></returns>
    /// <response code="204">Solicitação executada com sucesso</response>
    /// <response code="400">Falha ao processar requisição</response>
    /// <response code="500">Erro do Servidor</response>
    [HttpPost]
    public async Task<IActionResult> CadastrarContato(ContatoModel request)
    {
        var response = await _contatoApplicationService.CadastrarContato(request);
        return Ok(response);
    }

    /// <summary>
    /// Atualizar um contato
    /// </summary>
    /// <param name="id">Id do contato</param>
    /// <param name="request">Informações do contato</param>
    /// <returns></returns>
    /// <response code="204">Solicitação executada com sucesso</response>
    /// <response code="400">Falha ao processar requisição</response>
    /// <response code="500">Erro do Servidor</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> AtualizarContato(int id, ContatoModel request)
    {
        var response = await _contatoApplicationService.AtualizarContato(id, request);
        return Ok(response);
    }

    /// <summary>
    /// Buscar todos contatos ou por DDD
    /// </summary>
    /// <param name="request">Informação do contato</param>
    /// <returns></returns>
    /// <response code="204">Solicitação executada com sucesso</response>
    /// <response code="400">Falha ao processar requisição</response>
    /// <response code="404">Contato não encontrado</response>
    /// <response code="500">Erro do Servidor</response>
    [HttpGet]
    public async Task<IActionResult> ObterTodosContatos(string? ddd)
    {
        var response = await _contatoApplicationService.ObterTodosContatos(ddd);

        return response.Any() ? Ok(response) : NotFound();
    }

    /// <summary>
    /// Deletar um contato
    /// </summary>
    /// <param name="id">Id do contato</param>
    /// <response code="204">Solicitação com sucesso</response>
    /// <response code="400">Falha ao processar requisição</response>
    /// <response code="500">Erro do Servidor</response>
    [HttpDelete("id")]
    public async Task<IActionResult> DeletarContato(int id)
    {
        var response = await _contatoApplicationService.DeletarContato(id);
        return Ok(response);
    }
}
