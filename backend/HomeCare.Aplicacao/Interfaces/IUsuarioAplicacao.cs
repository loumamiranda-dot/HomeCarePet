using HomeCare.Dominio.Entidades;

namespace HomeCare.Aplicacao.Interfaces;

public interface IUsuarioAplicacao
{
    Task<Usuario> ObterAsync(int id);
    Task<List<Usuario>> ListarAsync();
    Task AtualizarAsync(int id, string nome, string email);
    Task DeletarAsync(int id);
    Task RestaurarAsync(int id);
}
