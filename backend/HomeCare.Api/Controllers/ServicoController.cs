using HomeCare.Api.Modelos.Servico;
using HomeCare.Aplicacao.Interfaces;
using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Api.Controllers;

[ApiController]
[Route("api/servicos")]
public class ServicoController : ControllerBase
{
    private readonly IServicoAplicacao _servicoAplicacao;
    private readonly IServicoConsultaRepositorio _servicoConsulta;

    public ServicoController(IServicoAplicacao servicoAplicacao, IServicoConsultaRepositorio servicoConsulta)
    {
        _servicoAplicacao = servicoAplicacao;
        _servicoConsulta = servicoConsulta;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var servicos = await _servicoAplicacao.ListarAtivosAsync();
        return Ok(servicos.Select(s => new ServicoResposta
        {
            Id = s.Id, Nome = s.Nome, Descricao = s.Descricao,
            Preco = s.Preco, DuracaoEmMinutos = s.DuracaoEmMinutos, Ativo = s.Ativo
        }));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Obter(int id)
    {
        var servico = await _servicoAplicacao.ObterAsync(id);
        return Ok(new ServicoResposta
        {
            Id = servico.Id, Nome = servico.Nome, Descricao = servico.Descricao,
            Preco = servico.Preco, DuracaoEmMinutos = servico.DuracaoEmMinutos, Ativo = servico.Ativo
        });
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Criar([FromBody] ServicoCriar requisicao)
    {
        var servico = await _servicoAplicacao.CriarAsync(
            requisicao.Nome, requisicao.Descricao, requisicao.Preco, requisicao.DuracaoEmMinutos);

        return Created(string.Empty, new ServicoResposta
        {
            Id = servico.Id, Nome = servico.Nome, Descricao = servico.Descricao,
            Preco = servico.Preco, DuracaoEmMinutos = servico.DuracaoEmMinutos, Ativo = servico.Ativo
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ServicoAtualizar requisicao)
    {
        await _servicoAplicacao.AtualizarAsync(id, requisicao.Nome, requisicao.Descricao,
            requisicao.Preco, requisicao.DuracaoEmMinutos, requisicao.Ativo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Deletar(int id)
    {
        await _servicoAplicacao.DeletarAsync(id);
        return NoContent();
    }

    [HttpGet("ativos/sp")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarAtivosSp()
    {
        var resultado = await _servicoConsulta.ListarAtivosAsync();
        return Ok(resultado);
    }
}
