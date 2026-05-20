using HomeCare.Api.Modelos.Pet;
using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/pets")]
[Authorize]
public class PetController : ControllerBase
{
    private readonly IPetAplicacao _petAplicacao;
    private readonly IClienteAplicacao _clienteAplicacao;
    private readonly IClienteConsultaRepositorio _clienteConsulta;

    public PetController(
        IPetAplicacao petAplicacao,
        IClienteAplicacao clienteAplicacao,
        IClienteConsultaRepositorio clienteConsulta)
    {
        _petAplicacao = petAplicacao;
        _clienteAplicacao = clienteAplicacao;
        _clienteConsulta = clienteConsulta;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var pets = await _petAplicacao.ListarAsync(pagina, tamanhoPagina);
        return Ok(pets.Select(MapearResposta));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var pet = await _petAplicacao.ObterAsync(id);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        if (role == "Cliente")
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (pet.ClienteId != cliente.Id) return Forbid();
        }

        return Ok(MapearResposta(pet));
    }

    [HttpGet("meus-pets")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MeusPets()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
        var pets = await _petAplicacao.ListarPorClienteAsync(cliente.Id);
        return Ok(pets.Select(MapearResposta));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PetCriar requisicao)
    {
        if (User.IsInRole("Cliente"))
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var clienteLogado = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (clienteLogado.Id != requisicao.ClienteId) return Forbid();
        }

        var pet = await _petAplicacao.CriarAsync(
            requisicao.Nome, requisicao.Tipo, requisicao.Raca,
            requisicao.Idade, requisicao.Peso, requisicao.Observacoes, requisicao.ClienteId);

        return Created(string.Empty, MapearResposta(pet));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] PetAtualizar requisicao)
    {
        var pet = await _petAplicacao.ObterAsync(id);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        if (role == "Cliente")
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (pet.ClienteId != cliente.Id) return Forbid();
        }

        await _petAplicacao.AtualizarAsync(id, requisicao.Nome, requisicao.Tipo,
            requisicao.Raca, requisicao.Idade, requisicao.Peso, requisicao.Observacoes);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var pet = await _petAplicacao.ObterAsync(id);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        if (role == "Cliente")
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (pet.ClienteId != cliente.Id) return Forbid();
        }

        await _petAplicacao.DeletarAsync(id);
        return NoContent();
    }

    // endpoint usando stored procedure
    [HttpPost("sp")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> CriarViaSp([FromBody] CriarPetDto dto)
    {
        var petId = await _clienteConsulta.CadastrarPetAsync(dto);
        return Created(string.Empty, new { PetId = petId });
    }

    private static PetResposta MapearResposta(HomeCare.Dominio.Entidades.Pet pet) => new()
    {
        Id = pet.Id,
        Nome = pet.Nome,
        Tipo = pet.Tipo.ToString(),
        Raca = pet.Raca,
        Idade = pet.Idade,
        Peso = pet.Peso,
        Observacoes = pet.Observacoes,
        ClienteId = pet.ClienteId
    };
}
