using HomeCare.Dominio.Entidades;

namespace HomeCare.Repositorio.Interfaces;

public interface IClienteRepositorio
{
    Task SalvarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task DeletarAsync(Cliente cliente);
    Task<Cliente?> ObterAsync(int id);
    Task<Cliente?> ObterPorUsuarioIdAsync(int usuarioId);
    Task<Cliente?> ObterPorEmailAsync(string email);
    Task<List<Cliente>> ListarAsync(int pagina, int tamanhoPagina);
}
