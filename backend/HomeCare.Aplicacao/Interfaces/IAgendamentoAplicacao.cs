using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Aplicacao.Interfaces;

public interface IAgendamentoAplicacao
{
    Task<Agendamento> CriarAsync(int clienteId, int petId, int servicoId, DateTime dataHora, string observacoes);
    Task<Agendamento> ObterAsync(int id, int usuarioLogadoId, string role);
    Task<List<Agendamento>> ListarAsync(int pagina, int tamanhoPagina);
    Task<List<Agendamento>> ListarPorClienteAsync(int clienteId);
    Task<List<Agendamento>> ListarPorStatusAsync(StatusAgendamento status);
    Task AtualizarAsync(int id, DateTime dataHora, StatusAgendamento status, string observacoes);
    Task CancelarAsync(int id, int usuarioLogadoId);
    Task DeletarAsync(int id);
}
