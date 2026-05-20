using System.Data;
using Dapper;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Fabrica;
using HomeCare.Repositorio.Interfaces.Consulta;

namespace HomeCare.Repositorio.Consulta;

public class ServicoConsultaRepositorio : IServicoConsultaRepositorio
{
    private readonly IConexaoFabrica _fabrica;

    public ServicoConsultaRepositorio(IConexaoFabrica fabrica)
    {
        _fabrica = fabrica;
    }

    // ── Stored Procedures ──────────────────────────────────────

    public async Task<IEnumerable<ServicoAtivoDto>> ListarAtivosAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<ServicoAtivoDto>(
            "dbo.spListarServicosAtivos",
            commandType: CommandType.StoredProcedure);
    }

    // ── Views ──────────────────────────────────────────────────

    public async Task<IEnumerable<RelatorioServicoDto>> ObterRelatorioAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<RelatorioServicoDto>(
            "SELECT * FROM dbo.vwRelatorioServicos ORDER BY ReceitaTotal DESC");
    }

    // ── Functions ──────────────────────────────────────────────

    public async Task<decimal> ObterReceitaTotalAsync(int servicoId)
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QuerySingleAsync<decimal>(
            "SELECT dbo.fnReceitaTotalServico(@ServicoId)",
            new { ServicoId = servicoId });
    }
}
