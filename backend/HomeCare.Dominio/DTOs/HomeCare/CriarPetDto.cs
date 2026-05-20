namespace HomeCare.Dominio.DTOs.HomeCare;

public class CriarPetDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Tipo { get; set; }
    public string? Raca { get; set; }
    public int? Idade { get; set; }
    public decimal? Peso { get; set; }
    public string? Observacoes { get; set; }
}
