using HomeCare.Dominio.Entidades;

namespace HomeCare.Repositorio.Interfaces;

public interface IUsuarioRepositorio
{
    Task SalvarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task DeletarAsync(Usuario usuario);
    Task<Usuario?> ObterAsync(int id);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<List<Usuario>> ListarAsync();
}
