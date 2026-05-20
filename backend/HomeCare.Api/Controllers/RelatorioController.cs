using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/relatorios")]
[Authorize(Roles = "Administrador,Funcionario")]
public class RelatorioController : ControllerBase
{
    private readonly IServicoConsultaRepositorio _servicoConsulta;
    private readonly IClienteConsultaRepositorio _clienteConsulta;

    public RelatorioController(
        IServicoConsultaRepositorio servicoConsulta,
        IClienteConsultaRepositorio clienteConsulta)
    {
        _servicoConsulta = servicoConsulta;
        _clienteConsulta = clienteConsulta;
    }

    // relatorio de servicos - usa a view do banco
    [HttpGet("servicos")]
    public async Task<IActionResult> RelatorioServicos()
    {
        var relatorio = await _servicoConsulta.ObterRelatorioAsync();
        return Ok(relatorio);
    }

    // receita de um servico especifico (usa function do sql)
    [HttpGet("servicos/{id}/receita")]
    public async Task<IActionResult> ReceitaServico(int id)
    {
        var receita = await _servicoConsulta.ObterReceitaTotalAsync(id);
        return Ok(new { ServicoId = id, ReceitaTotal = receita });
    }

    [HttpGet("clientes-com-pets")]
    public async Task<IActionResult> ClientesComPets()
    {
        var clientes = await _clienteConsulta.ObterClientesComPetsAsync();
        return Ok(clientes);
    }
}
