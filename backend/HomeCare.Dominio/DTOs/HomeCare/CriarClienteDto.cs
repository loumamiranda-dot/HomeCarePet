namespace HomeCare.Dominio.DTOs.HomeCare;

public class CriarClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
}
