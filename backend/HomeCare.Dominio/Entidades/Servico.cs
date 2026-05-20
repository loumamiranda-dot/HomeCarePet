namespace HomeCare.Dominio.Entidades;

public class Servico
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool Ativo { get; set; } = true;
}
