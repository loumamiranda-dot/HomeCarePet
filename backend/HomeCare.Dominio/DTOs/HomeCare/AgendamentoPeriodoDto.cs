namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado pela table-valued function fnAgendamentosNoPeriodo
public class AgendamentoPeriodoDto
{
    public int AgendamentoId { get; set; }
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public string? Observacoes { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public string NomePet { get; set; } = string.Empty;
    public int TipoPet { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
}
