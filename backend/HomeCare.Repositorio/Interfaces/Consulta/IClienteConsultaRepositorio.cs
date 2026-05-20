using HomeCare.Dominio.DTOs.HomeCare;

namespace HomeCare.Repositorio.Interfaces.Consulta;

public interface IClienteConsultaRepositorio
{
    // Stored Procedures
    Task<int> CadastrarAsync(CriarClienteDto dto);
    Task<int> CadastrarPetAsync(CriarPetDto dto);

    // Views
    Task<IEnumerable<ClienteComPetsDto>> ObterClientesComPetsAsync();
}
