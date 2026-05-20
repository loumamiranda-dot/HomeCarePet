using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Api.Modelos.Agendamento;

public class AgendamentoAtualizar
{
    public DateTime DataHora { get; set; }
    public StatusAgendamento Status { get; set; }
    public string Observacoes { get; set; } = string.Empty;
}
