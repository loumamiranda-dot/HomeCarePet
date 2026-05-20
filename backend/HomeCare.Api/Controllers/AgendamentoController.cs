using HomeCare.Api.Modelos.Agendamento;
using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/agendamentos")]
[Authorize]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoAplicacao _agendamentoAplicacao;
    private readonly IClienteAplicacao _clienteAplicacao;
    private readonly IAgendamentoConsultaRepositorio _agendamentoConsulta;

    public AgendamentoController(
        IAgendamentoAplicacao agendamentoAplicacao,
        IClienteAplicacao clienteAplicacao,
        IAgendamentoConsultaRepositorio agendamentoConsulta)
    {
        _agendamentoAplicacao = agendamentoAplicacao;
        _clienteAplicacao = clienteAplicacao;
        _agendamentoConsulta = agendamentoConsulta;
    }

    [HttpGet]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var agendamentos = await _agendamentoAplicacao.ListarAsync(pagina, tamanhoPagina);
        return Ok(agendamentos.Select(MapearResposta));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;
        var agendamento = await _agendamentoAplicacao.ObterAsync(id, usuarioId, role);
        return Ok(MapearResposta(agendamento));
    }

    [HttpGet("meus-agendamentos")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> MeusAgendamentos()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cliente = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
        var agendamentos = await _agendamentoAplicacao.ListarPorClienteAsync(cliente.Id);
        return Ok(agendamentos.Select(MapearResposta));
    }

    [HttpGet("por-status/{status}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> ListarPorStatus(StatusAgendamento status)
    {
        var agendamentos = await _agendamentoAplicacao.ListarPorStatusAsync(status);
        return Ok(agendamentos.Select(MapearResposta));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] AgendamentoCriar requisicao)
    {
        if (User.IsInRole("Cliente"))
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var clienteLogado = await _clienteAplicacao.ObterPorUsuarioIdAsync(usuarioId);
            if (clienteLogado.Id != requisicao.ClienteId) return Forbid();
        }

        var agendamento = await _agendamentoAplicacao.CriarAsync(
            requisicao.ClienteId, requisicao.PetId, requisicao.ServicoId,
            requisicao.DataHora, requisicao.Observacoes);

        return Created(string.Empty, MapearResposta(agendamento));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AgendamentoAtualizar requisicao)
    {
        await _agendamentoAplicacao.AtualizarAsync(id, requisicao.DataHora, requisicao.Status, requisicao.Observacoes);
        return NoContent();
    }

    [HttpPut("{id}/cancelar")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _agendamentoAplicacao.CancelarAsync(id, usuarioId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _agendamentoAplicacao.DeletarAsync(id);
        return NoContent();
    }

    // endpoints que usam stored procedure (dapper)

    [HttpGet("hoje")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> AgendaHoje()
    {
        var resultado = await _agendamentoConsulta.ObterHojeAsync();
        return Ok(resultado);
    }

    [HttpGet("detalhados")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> Detalhados()
    {
        var resultado = await _agendamentoConsulta.ObterDetalhadosAsync();
        return Ok(resultado);
    }

    [HttpGet("periodo")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> PorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        var resultado = await _agendamentoConsulta.ObterNoPeriodoAsync(dataInicio, dataFim);
        return Ok(resultado);
    }

    [HttpGet("cliente/{clienteId}/total")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> TotalPorCliente(int clienteId)
    {
        var total = await _agendamentoConsulta.ObterTotalPorClienteAsync(clienteId);
        return Ok(new { ClienteId = clienteId, TotalAgendamentos = total });
    }

    [HttpGet("historico/cliente/{clienteId}")]
    [Authorize(Roles = "Administrador,Funcionario")]
    public async Task<IActionResult> HistoricoCliente(int clienteId)
    {
        var resultado = await _agendamentoConsulta.ObterPorClienteAsync(clienteId);
        return Ok(resultado);
    }

    // converte a entidade pro modelo de resposta
    private static AgendamentoResposta MapearResposta(HomeCare.Dominio.Entidades.Agendamento a) => new()
    {
        Id = a.Id,
        ClienteId = a.ClienteId,
        NomeCliente = a.Cliente?.Nome ?? string.Empty,
        PetId = a.PetId,
        NomePet = a.Pet?.Nome ?? string.Empty,
        ServicoId = a.ServicoId,
        NomeServico = a.Servico?.Nome ?? string.Empty,
        DataHora = a.DataHora,
        Status = a.Status.ToString(),
        Observacoes = a.Observacoes
    };
}
