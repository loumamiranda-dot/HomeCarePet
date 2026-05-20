namespace HomeCare.Dominio.Entidades;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; } = DateTime.Now;
    public bool Ativo { get; set; } = true;
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<Pet> Pets { get; set; } = new List<Pet>();

    public void Deletar() => Ativo = false;
    public void Restaurar() => Ativo = true;
}
