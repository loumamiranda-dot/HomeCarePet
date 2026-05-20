namespace HomeCare.Api.Modelos.Servico;

public class ServicoResposta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int DuracaoEmMinutos { get; set; }
    public bool Ativo { get; set; }
}
