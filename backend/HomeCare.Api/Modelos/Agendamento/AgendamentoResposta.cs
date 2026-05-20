namespace HomeCare.Api.Modelos.Agendamento;

public class AgendamentoResposta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public int PetId { get; set; }
    public string NomePet { get; set; } = string.Empty;
    public int ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}
