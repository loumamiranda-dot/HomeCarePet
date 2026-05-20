namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado por spListarServicosAtivos
public class ServicoAtivoDto
{
    public int ServicoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool Ativo { get; set; }
}
