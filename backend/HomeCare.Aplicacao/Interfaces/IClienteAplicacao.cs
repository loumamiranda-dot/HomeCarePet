using HomeCare.Dominio.Entidades;

namespace HomeCare.Aplicacao.Interfaces;

public interface IClienteAplicacao
{
    Task<Cliente> CriarAsync(string nome, string email, string senha, string telefone, string endereco);
    Task<Cliente> ObterAsync(int id);
    Task<Cliente> ObterPorUsuarioIdAsync(int usuarioId);
    Task<List<Cliente>> ListarAsync(int pagina, int tamanhoPagina);
    Task AtualizarAsync(int id, string nome, string email, string telefone, string endereco);
    Task DeletarAsync(int id);
}
