using HomeCare.Api.Modelos.Usuario;
using HomeCare.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioAplicacao _usuarioAplicacao;

    public UsuarioController(IUsuarioAplicacao usuarioAplicacao)
    {
        _usuarioAplicacao = usuarioAplicacao;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _usuarioAplicacao.ListarAsync();
        return Ok(usuarios.Select(u => new UsuarioResposta
        {
            Id = u.Id, Nome = u.Nome, Email = u.Email,
            TipoUsuario = u.TipoUsuario.ToString(), Ativo = u.Ativo
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var usuario = await _usuarioAplicacao.ObterAsync(id);
        return Ok(new UsuarioResposta
        {
            Id = usuario.Id, Nome = usuario.Nome, Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario.ToString(), Ativo = usuario.Ativo
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioAtualizar requisicao)
    {
        await _usuarioAplicacao.AtualizarAsync(id, requisicao.Nome, requisicao.Email);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _usuarioAplicacao.DeletarAsync(id);
        return NoContent();
    }

    [HttpPut("{id}/restaurar")]
    public async Task<IActionResult> Restaurar(int id)
    {
        await _usuarioAplicacao.RestaurarAsync(id);
        return NoContent();
    }
}
