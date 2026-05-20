namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado pela view vwAgendamentosHoje
public class AgendamentoHojeDto
{
    public int AgendamentoId { get; set; }
    public DateTime DataHora { get; set; }
    public int Status { get; set; }
    public string? Observacoes { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string NomePet { get; set; } = string.Empty;
    public int TipoPet { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public int DuracaoEmMinutos { get; set; }
}
