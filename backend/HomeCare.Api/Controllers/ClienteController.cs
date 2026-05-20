using HomeCare.Api.Modelos.Cliente;
using HomeCare.Aplicacao.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteAplicacao _clienteAplicacao;

    public ClienteController(IClienteAplicacao clienteAplicacao)
    {
        _clienteAplicacao = clienteAplicacao;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var clientes = await _clienteAplicacao.ListarAsync(pagina, tamanhoPagina);
        var resposta = clientes.Select(c => new ClienteResposta
        {
            Id = c.Id, Nome = c.Nome, Email = c.Email,
            Telefone = c.Telefone, Endereco = c.Endereco,
            DataCadastro = c.DataCadastro, Ativo = c.Ativo
        });
        return Ok(resposta);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        var cliente = await _clienteAplicacao.ObterAsync(id);

        if (role == "Cliente")
        {
            var meuPerfil = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (meuPerfil.Id != id) return Forbid();
        }

        return Ok(new ClienteResposta
        {
            Id = cliente.Id, Nome = cliente.Nome, Email = cliente.Email,
            Telefone = cliente.Telefone, Endereco = cliente.Endereco,
            DataCadastro = cliente.DataCadastro, Ativo = cliente.Ativo
        });
    }

    [HttpGet("meu-perfil")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MeuPerfil()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
        return Ok(new ClienteResposta
        {
            Id = cliente.Id, Nome = cliente.Nome, Email = cliente.Email,
            Telefone = cliente.Telefone, Endereco = cliente.Endereco,
            DataCadastro = cliente.DataCadastro, Ativo = cliente.Ativo
        });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Criar([FromBody] ClienteCriar requisicao)
    {
        var cliente = await _clienteAplicacao.CriarAsync(
            requisicao.Nome, requisicao.Email, requisicao.Senha,
            requisicao.Telefone, requisicao.Endereco);

        return Created(string.Empty, new ClienteResposta
        {
            Id = cliente.Id, Nome = cliente.Nome, Email = cliente.Email,
            Telefone = cliente.Telefone, Endereco = cliente.Endereco,
            DataCadastro = cliente.DataCadastro, Ativo = cliente.Ativo
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteAtualizar requisicao)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        if (role == "Cliente")
        {
            var meuPerfil = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (meuPerfil.Id != id) return Forbid();
        }

        await _clienteAplicacao.AtualizarAsync(id, requisicao.Nome, requisicao.Email,
            requisicao.Telefone, requisicao.Endereco);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _clienteAplicacao.DeletarAsync(id);
        return NoContent();
    }
}
