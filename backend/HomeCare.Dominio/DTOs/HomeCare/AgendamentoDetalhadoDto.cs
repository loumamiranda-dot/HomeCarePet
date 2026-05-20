namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado por spObterAgendamentosPorCliente, spObterAgendamentosPorStatus
// e pela view vwAgendamentosDetalhados
public class AgendamentoDetalhadoDto
{
    public int AgendamentoId { get; set; }
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public string? Observacoes { get; set; }

    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public string? Telefone { get; set; }

    public int PetId { get; set; }
    public string NomePet { get; set; } = string.Empty;
    public int TipoPet { get; set; }
    public string? Raca { get; set; }
    public decimal? Peso { get; set; }

    public int ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public string? DescricaoServico { get; set; }
}
