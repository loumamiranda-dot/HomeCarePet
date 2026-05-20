using HomeCare.Dominio.DTOs.HomeCare;

namespace HomeCare.Repositorio.Interfaces.Consulta;

public interface IDashboardRepositorio
{
    Task<DashboardDto> ObterDashboardAsync();
}
