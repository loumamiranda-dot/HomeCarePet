namespace HomeCare.Api.Modelos.Pet;

public class PetResposta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Raca { get; set; } = string.Empty;
    public int Idade { get; set; }
    public decimal Peso { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public int ClienteId { get; set; }
}
