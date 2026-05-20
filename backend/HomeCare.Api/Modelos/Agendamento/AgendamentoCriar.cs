namespace HomeCare.Api.Modelos.Agendamento;

public class AgendamentoCriar
{
    public int ClienteId { get; set; }
    public int PetId { get; set; }
    public int ServicoId { get; set; }
    public DateTime DataHora { get; set; }
    public string Observacoes { get; set; } = string.Empty;
}
