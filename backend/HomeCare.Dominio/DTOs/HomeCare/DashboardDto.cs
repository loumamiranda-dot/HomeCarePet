namespace HomeCare.Dominio.DTOs.HomeCare;

public class DashboardDto
{
    public ResumoGeralDto ResumoGeral { get; set; } = new();
    public ReceitaDto Receita { get; set; } = new();
    public List<AgendamentoPorStatusDto> AgendamentosPorStatus { get; set; } = [];
    public List<ServicoRankingDto> TopServicos { get; set; } = [];
    public List<ClienteRankingDto> TopClientes { get; set; } = [];
    public List<TendenciaMensalDto> TendenciaMensal { get; set; } = [];
    public List<ProximoAgendamentoDto> ProximosAgendamentos { get; set; } = [];
}

public class ResumoGeralDto
{
    public int TotalClientesAtivos { get; set; }
    public int TotalPets { get; set; }
    public int TotalServicosAtivos { get; set; }
    public int TotalAgendamentosHoje { get; set; }
}

public class ReceitaDto
{
    public decimal ReceitaTotal { get; set; }
    public decimal ReceitaMesAtual { get; set; }
    public int MesAtual { get; set; }
    public int AnoAtual { get; set; }
}

public class AgendamentoPorStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Total { get; set; }
}

public class ServicoRankingDto
{
    public int ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int TotalUsos { get; set; }
    public decimal ReceitaTotal { get; set; }
}

public class ClienteRankingDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalAgendamentos { get; set; }
    public decimal ValorTotal { get; set; }
}

public class TendenciaMensalDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string MesNome { get; set; } = string.Empty;
    public int TotalAgendamentos { get; set; }
    public decimal Receita { get; set; }
}

public class ProximoAgendamentoDto
{
    public int AgendamentoId { get; set; }
    public DateTime DataHora { get; set; }
    public string Status { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    public string NomePet { get; set; } = string.Empty;
    public string NomeServico { get; set; } = string.Empty;

    // Usado internamente pelo Dapper — não exposto ao JSON
    [System.Text.Json.Serialization.JsonIgnore]
    public int StatusCodigo { get; set; }
}
