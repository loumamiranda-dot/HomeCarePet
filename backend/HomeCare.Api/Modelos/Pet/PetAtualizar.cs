using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Api.Modelos.Pet;

public class PetAtualizar
{
    public string Nome { get; set; } = string.Empty;
    public TiposPet Tipo { get; set; }
    public string Raca { get; set; } = string.Empty;
    public int Idade { get; set; }
    public decimal Peso { get; set; }
    public string Observacoes { get; set; } = string.Empty;
}
