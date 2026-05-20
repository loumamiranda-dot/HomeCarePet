using HomeCare.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/sugestoes")]
[Authorize]
public class SugestaoController : ControllerBase
{
    private readonly ISugestaoPacoteAplicacao _sugestaoPacote;
    private readonly IClienteAplicacao _clienteAplicacao;

    public SugestaoController(ISugestaoPacoteAplicacao sugestaoPacote, IClienteAplicacao clienteAplicacao)
    {
        _sugestaoPacote = sugestaoPacote;
        _clienteAplicacao = clienteAplicacao;
    }

    // gera sugestao pra um cliente especifico
    [HttpGet("cliente/{clienteId}/pacotes")]
    [Authorize(Roles = "Administrador,Funcionario,Cliente")]
    public async Task<IActionResult> SugerirPacotes(int clienteId)
    {
        if (User.IsInRole("Cliente"))
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var clienteLogado = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (clienteLogado.Id != clienteId)
                return Forbid();
        }

        var sugestoes = await _sugestaoPacote.GerarSugestoesAsync(clienteId);
        return Ok(sugestoes);
    }

    // esse aqui pega o cliente logado automaticamente
    [HttpGet("minhas-sugestoes")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MinhasSugestoes()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
        var sugestoes = await _sugestaoPacote.GerarSugestoesAsync(cliente.Id);
        return Ok(sugestoes);
    }
}
