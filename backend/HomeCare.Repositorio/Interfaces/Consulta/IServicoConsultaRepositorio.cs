using HomeCare.Dominio.DTOs.HomeCare;

namespace HomeCare.Repositorio.Interfaces.Consulta;

public interface IServicoConsultaRepositorio
{
    // Stored Procedures
    Task<IEnumerable<ServicoAtivoDto>> ListarAtivosAsync();

    // Views
    Task<IEnumerable<RelatorioServicoDto>> ObterRelatorioAsync();

    // Functions
    Task<decimal> ObterReceitaTotalAsync(int servicoId);
}
