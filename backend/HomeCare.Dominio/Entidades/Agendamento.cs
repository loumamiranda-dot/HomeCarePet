using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Dominio.Entidades;

public class Agendamento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int PetId { get; set; }
    public int ServicoId { get; set; }
    public DateTime DataHora { get; set; }
    public StatusAgendamento Status { get; set; } = StatusAgendamento.Pendente;
    public string Observacoes { get; set; } = string.Empty;
    public Cliente? Cliente { get; set; }
    public Pet? Pet { get; set; }
    public Servico? Servico { get; set; }
}
