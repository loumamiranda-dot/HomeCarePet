using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Dominio.Entidades;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public TiposUsuario TipoUsuario { get; set; }
    public bool Ativo { get; set; } = true;

    public void Deletar() => Ativo = false;
    public void Restaurar() => Ativo = true;
}
