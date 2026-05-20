namespace HomeCare.Dominio.DTOs.HomeCare;

// Mapeado pela view vwClientesComPets
public class ClienteComPetsDto
{
    public int ClienteId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public DateTime DataCadastro { get; set; }
    public int TotalPets { get; set; }
}
