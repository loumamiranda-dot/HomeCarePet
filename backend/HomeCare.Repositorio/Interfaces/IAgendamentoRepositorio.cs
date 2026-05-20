using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Repositorio.Interfaces;

public interface IAgendamentoRepositorio
{
    Task SalvarAsync(Agendamento agendamento);
    Task AtualizarAsync(Agendamento agendamento);
    Task DeletarAsync(Agendamento agendamento);
    Task<Agendamento?> ObterAsync(int id);
    Task<List<Agendamento>> ListarAsync(int pagina, int tamanhoPagina);
    Task<List<Agendamento>> ListarPorClienteAsync(int clienteId);
    Task<List<Agendamento>> ListarPorStatusAsync(StatusAgendamento status);
}
