using HomeCare.Api.Modelos.Autenticacao;
using HomeCare.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/autenticacao")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoAplicacao _autenticacaoAplicacao;

    public AutenticacaoController(IAutenticacaoAplicacao autenticacaoAplicacao)
    {
        _autenticacaoAplicacao = autenticacaoAplicacao;
    }

    [HttpPost("entrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Entrar([FromBody] LoginRequisicao requisicao)
    {
        var token = await _autenticacaoAplicacao.EntrarAsync(requisicao.Email, requisicao.Senha);
        return Ok(new { Token = token });
    }

    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] RegistroRequisicao requisicao)
    {
        var usuario = await _autenticacaoAplicacao.RegistrarClienteAsync(
            requisicao.Nome, requisicao.Email, requisicao.Senha,
            requisicao.Telefone, requisicao.Endereco);

        return Created(string.Empty, new { usuario.Id, usuario.Nome, usuario.Email });
    }

    [HttpPost("registrar-admin")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> RegistrarAdmin([FromBody] RegistroAdminRequisicao requisicao)
    {
        var usuario = await _autenticacaoAplicacao.RegistrarAdminAsync(
            requisicao.Nome, requisicao.Email, requisicao.Senha, requisicao.TipoUsuario);

        return Created(string.Empty, new { usuario.Id, usuario.Nome, usuario.Email, usuario.TipoUsuario });
    }
}
