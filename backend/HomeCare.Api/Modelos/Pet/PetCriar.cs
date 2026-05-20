using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Api.Modelos.Pet;

public class PetCriar
{
    public string Nome { get; set; } = string.Empty;
    public TiposPet Tipo { get; set; }
    public string Raca { get; set; } = string.Empty;
    public int Idade { get; set; }
    public decimal Peso { get; set; }
    public string Observacoes { get; set; } = string.Empty;
    public int ClienteId { get; set; }
}
