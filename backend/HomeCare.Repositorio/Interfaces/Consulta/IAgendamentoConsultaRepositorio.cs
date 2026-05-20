using HomeCare.Dominio.DTOs.HomeCare;

namespace HomeCare.Repositorio.Interfaces.Consulta;

public interface IAgendamentoConsultaRepositorio
{
    // Stored Procedures
    Task<int> CriarAsync(CriarAgendamentoDto dto);
    Task<IEnumerable<AgendamentoDetalhadoDto>> ObterPorClienteAsync(int clienteId);
    Task<IEnumerable<AgendamentoDetalhadoDto>> ObterPorStatusAsync(int status);
    Task AtualizarStatusAsync(AtualizarStatusDto dto);
    Task CancelarAsync(int agendamentoId);

    // Views
    Task<IEnumerable<AgendamentoDetalhadoDto>> ObterDetalhadosAsync();
    Task<IEnumerable<AgendamentoHojeDto>> ObterHojeAsync();

    // Functions
    Task<IEnumerable<AgendamentoPeriodoDto>> ObterNoPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<int> ObterTotalPorClienteAsync(int clienteId);

    Task<int> FinalizarAgendamentosPassadosAsync();
}
