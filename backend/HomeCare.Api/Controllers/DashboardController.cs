using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Administrador,Funcionario")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardRepositorio _dashboard;

    public DashboardController(IDashboardRepositorio dashboard)
    {
        _dashboard = dashboard;
    }

    // retorna tudo de uma vez pro dashboard
    [HttpGet]
    public async Task<IActionResult> Obter()
    {
        var dados = await _dashboard.ObterDashboardAsync();
        return Ok(dados);
    }
}
