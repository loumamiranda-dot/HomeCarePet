using System.Data;
using Dapper;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Fabrica;
using HomeCare.Repositorio.Interfaces.Consulta;

namespace HomeCare.Repositorio.Consulta;

public class AgendamentoConsultaRepositorio : IAgendamentoConsultaRepositorio
{
    private readonly IConexaoFabrica _fabrica;

    public AgendamentoConsultaRepositorio(IConexaoFabrica fabrica)
    {
        _fabrica = fabrica;
    }

    // ── Stored Procedures ──────────────────────────────────────

    public async Task<int> CriarAsync(CriarAgendamentoDto dto)
    {
        using var conexao = _fabrica.Criar();

        var parametros = new DynamicParameters();
        parametros.Add("@ClienteId",   dto.ClienteId,   DbType.Int32);
        parametros.Add("@PetId",       dto.PetId,       DbType.Int32);
        parametros.Add("@ServicoId",   dto.ServicoId,   DbType.Int32);
        parametros.Add("@DataHora",    dto.DataHora,    DbType.DateTime2);
        parametros.Add("@Observacoes", dto.Observacoes, DbType.String);

        var resultado = await conexao.QuerySingleAsync<dynamic>(
            "dbo.spCriarAgendamento",
            parametros,
            commandType: CommandType.StoredProcedure);

        return (int)resultado.AgendamentoId;
    }

    public async Task<IEnumerable<AgendamentoDetalhadoDto>> ObterPorClienteAsync(int clienteId)
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<AgendamentoDetalhadoDto>(
            "dbo.spObterAgendamentosPorCliente",
            new { ClienteId = clienteId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<AgendamentoDetalhadoDto>> ObterPorStatusAsync(int status)
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<AgendamentoDetalhadoDto>(
            "dbo.spObterAgendamentosPorStatus",
            new { Status = status },
            commandType: CommandType.StoredProcedure);
    }

    public async Task AtualizarStatusAsync(AtualizarStatusDto dto)
    {
        using var conexao = _fabrica.Criar();

        var parametros = new DynamicParameters();
        parametros.Add("@AgendamentoId", dto.AgendamentoId, DbType.Int32);
        parametros.Add("@NovoStatus",    dto.NovoStatus,    DbType.Int32);

        await conexao.ExecuteAsync(
            "dbo.spAtualizarStatusAgendamento",
            parametros,
            commandType: CommandType.StoredProcedure);
    }

    public async Task CancelarAsync(int agendamentoId)
    {
        using var conexao = _fabrica.Criar();

        await conexao.ExecuteAsync(
            "dbo.spCancelarAgendamento",
            new { AgendamentoId = agendamentoId },
            commandType: CommandType.StoredProcedure);
    }

    // ── Views ──────────────────────────────────────────────────

    public async Task<IEnumerable<AgendamentoDetalhadoDto>> ObterDetalhadosAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<AgendamentoDetalhadoDto>(
            "SELECT * FROM dbo.vwAgendamentosDetalhados ORDER BY DataHora DESC");
    }

    public async Task<IEnumerable<AgendamentoHojeDto>> ObterHojeAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<AgendamentoHojeDto>(
            "SELECT * FROM dbo.vwAgendamentosHoje ORDER BY DataHora");
    }

    // ── Functions ──────────────────────────────────────────────

    public async Task<IEnumerable<AgendamentoPeriodoDto>> ObterNoPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QueryAsync<AgendamentoPeriodoDto>(
            "SELECT * FROM dbo.fnAgendamentosNoPeriodo(@DataInicio, @DataFim) ORDER BY DataHora",
            new { DataInicio = dataInicio, DataFim = dataFim });
    }

    public async Task<int> ObterTotalPorClienteAsync(int clienteId)
    {
        using var conexao = _fabrica.Criar();

        return await conexao.QuerySingleAsync<int>(
            "SELECT dbo.fnTotalAgendamentosCliente(@ClienteId)",
            new { ClienteId = clienteId });
    }

    public async Task<int> FinalizarAgendamentosPassadosAsync()
    {
        using var conexao = _fabrica.Criar();

        return await conexao.ExecuteAsync(@"
            UPDATE a
            SET a.Status = 3
            FROM dbo.Agendamento a
            INNER JOIN dbo.Servico s ON s.ServicoId = a.ServicoId
            WHERE a.Status IN (1, 2)
              AND DATEADD(MINUTE, s.DuracaoEmMinutos, a.DataHora) < GETDATE()");
    }
}
