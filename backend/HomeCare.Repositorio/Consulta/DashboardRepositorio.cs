using System.Data;
using Dapper;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Fabrica;
using HomeCare.Repositorio.Interfaces.Consulta;

namespace HomeCare.Repositorio.Consulta;

public class DashboardRepositorio : IDashboardRepositorio
{
    private readonly IConexaoFabrica _fabrica;

    private static readonly Dictionary<int, string> _statusNome = new()
    {
        [1] = "Pendente",
        [2] = "Confirmado",
        [3] = "Finalizado",
        [4] = "Cancelado"
    };

    private static readonly string[] _meses =
    [
        "", "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
        "Jul", "Ago", "Set", "Out", "Nov", "Dez"
    ];

    public DashboardRepositorio(IConexaoFabrica fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<DashboardDto> ObterDashboardAsync()
    {
        // roda tudo em paralelo pra ser mais rapido
        // cada metodo abre sua propria conexao
        var resumoTask = ObterResumoGeralAsync();
        var receitaTask = ObterReceitaAsync();
        var statusTask = ObterAgendamentosPorStatusAsync();
        var servicosTask = ObterTopServicosAsync();
        var clientesTask = ObterTopClientesAsync();
        var tendenciaTask = ObterTendenciaMensalAsync();
        var proximosTask = ObterProximosAgendamentosAsync();

        await Task.WhenAll(resumoTask, receitaTask, statusTask,
            servicosTask, clientesTask, tendenciaTask, proximosTask);

        return new DashboardDto
        {
            ResumoGeral = await resumoTask,
            Receita = await receitaTask,
            AgendamentosPorStatus = await statusTask,
            TopServicos = await servicosTask,
            TopClientes = await clientesTask,
            TendenciaMensal = await tendenciaTask,
            ProximosAgendamentos = await proximosTask
        };
    }

    private async Task<ResumoGeralDto> ObterResumoGeralAsync()
    {
        using var con = _fabrica.Criar();
        return await con.QuerySingleAsync<ResumoGeralDto>("""
            SELECT
                (SELECT COUNT(*) FROM dbo.Cliente WHERE Ativo = 1)        AS TotalClientesAtivos,
                (SELECT COUNT(*) FROM dbo.Pet)                            AS TotalPets,
                (SELECT COUNT(*) FROM dbo.Servico WHERE Ativo = 1)        AS TotalServicosAtivos,
                (SELECT COUNT(*) FROM dbo.Agendamento
                 WHERE CAST(DataHora AS DATE) = CAST(GETDATE() AS DATE)
                   AND Status IN (1, 2))                                  AS TotalAgendamentosHoje
            """);
    }

    private async Task<ReceitaDto> ObterReceitaAsync()
    {
        using var con = _fabrica.Criar();
        return await con.QuerySingleAsync<ReceitaDto>("""
            SELECT
                ISNULL((
                    SELECT SUM(s.Preco)
                    FROM dbo.Agendamento a JOIN dbo.Servico s ON a.ServicoId = s.ServicoId
                    WHERE a.Status = 3
                ), 0)                                   AS ReceitaTotal,
                ISNULL((
                    SELECT SUM(s.Preco)
                    FROM dbo.Agendamento a JOIN dbo.Servico s ON a.ServicoId = s.ServicoId
                    WHERE a.Status = 3
                      AND MONTH(a.DataHora) = MONTH(GETDATE())
                      AND YEAR(a.DataHora)  = YEAR(GETDATE())
                ), 0)                                   AS ReceitaMesAtual,
                MONTH(GETDATE())                        AS MesAtual,
                YEAR(GETDATE())                         AS AnoAtual
            """);
    }

    private async Task<List<AgendamentoPorStatusDto>> ObterAgendamentosPorStatusAsync()
    {
        using var con = _fabrica.Criar();
        var rows = await con.QueryAsync<StatusCount>("""
            SELECT Status, COUNT(*) AS Total
            FROM dbo.Agendamento
            GROUP BY Status
            ORDER BY Status
            """);

        return rows.Select(r => new AgendamentoPorStatusDto
        {
            Status = _statusNome.GetValueOrDefault(r.Status, "Desconhecido"),
            Total  = r.Total
        }).ToList();
    }

    private async Task<List<ServicoRankingDto>> ObterTopServicosAsync()
    {
        using var con = _fabrica.Criar();
        return (await con.QueryAsync<ServicoRankingDto>("""
            SELECT TOP 5
                s.ServicoId,
                s.Nome,
                s.Preco,
                COUNT(a.AgendamentoId)                                              AS TotalUsos,
                ISNULL(SUM(CASE WHEN a.Status = 3 THEN s.Preco ELSE 0 END), 0)     AS ReceitaTotal
            FROM dbo.Servico s
            LEFT JOIN dbo.Agendamento a ON s.ServicoId = a.ServicoId
            GROUP BY s.ServicoId, s.Nome, s.Preco
            ORDER BY TotalUsos DESC, ReceitaTotal DESC
            """)).ToList();
    }

    private async Task<List<ClienteRankingDto>> ObterTopClientesAsync()
    {
        using var con = _fabrica.Criar();
        return (await con.QueryAsync<ClienteRankingDto>("""
            SELECT TOP 5
                c.ClienteId,
                c.Nome,
                c.Email,
                COUNT(a.AgendamentoId)                                              AS TotalAgendamentos,
                ISNULL(SUM(CASE WHEN a.Status = 3 THEN s.Preco ELSE 0 END), 0)     AS ValorTotal
            FROM dbo.Cliente c
            LEFT JOIN dbo.Agendamento a ON c.ClienteId = a.ClienteId
            LEFT JOIN dbo.Servico s     ON a.ServicoId = s.ServicoId
            GROUP BY c.ClienteId, c.Nome, c.Email
            ORDER BY TotalAgendamentos DESC, ValorTotal DESC
            """)).ToList();
    }

    private async Task<List<TendenciaMensalDto>> ObterTendenciaMensalAsync()
    {
        using var con = _fabrica.Criar();
        var rows = (await con.QueryAsync<TendenciaMensalDto>("""
            SELECT
                YEAR(a.DataHora)   AS Ano,
                MONTH(a.DataHora)  AS Mes,
                COUNT(*)           AS TotalAgendamentos,
                ISNULL(SUM(CASE WHEN a.Status = 3 THEN s.Preco ELSE 0 END), 0) AS Receita
            FROM dbo.Agendamento a
            JOIN dbo.Servico s ON a.ServicoId = s.ServicoId
            WHERE a.DataHora >= DATEFROMPARTS(
                YEAR(DATEADD(MONTH, -5, GETDATE())),
                MONTH(DATEADD(MONTH, -5, GETDATE())), 1)
            GROUP BY YEAR(a.DataHora), MONTH(a.DataHora)
            ORDER BY Ano, Mes
            """)).ToList();

        foreach (var r in rows)
            r.MesNome = _meses[r.Mes];

        return rows;
    }

    private async Task<List<ProximoAgendamentoDto>> ObterProximosAgendamentosAsync()
    {
        using var con = _fabrica.Criar();
        return (await con.QueryAsync<ProximoAgendamentoDto>("""
            SELECT TOP 5
                a.AgendamentoId,
                a.DataHora,
                a.Status        AS StatusCodigo,
                c.Nome          AS NomeCliente,
                p.Nome          AS NomePet,
                s.Nome          AS NomeServico
            FROM dbo.Agendamento a
            JOIN dbo.Cliente c ON a.ClienteId = c.ClienteId
            JOIN dbo.Pet p     ON a.PetId     = p.PetId
            JOIN dbo.Servico s ON a.ServicoId = s.ServicoId
            WHERE a.Status IN (1, 2)
              AND a.DataHora >= GETDATE()
            ORDER BY a.DataHora ASC
            """)).Select(r =>
        {
            r.Status = _statusNome.GetValueOrDefault(r.StatusCodigo, "Desconhecido");
            return r;
        }).ToList();
    }

    private record StatusCount(int Status, int Total);
}
