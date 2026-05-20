namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado pela view vwRelatorioServicos
public class RelatorioServicoDto
{
    public int ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public int TotalAgendamentos { get; set; }
    public int TotalFinalizados { get; set; }
    public int TotalCancelados { get; set; }
    public decimal ReceitaTotal { get; set; }
}
