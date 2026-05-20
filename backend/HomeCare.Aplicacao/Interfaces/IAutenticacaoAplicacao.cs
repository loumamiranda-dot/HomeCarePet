using HomeCare.Dominio.Entidades;

namespace HomeCare.Aplicacao.Interfaces;

public interface IAutenticacaoAplicacao
{
    Task<string> EntrarAsync(string email, string senha);
    Task<Usuario> RegistrarClienteAsync(string nome, string email, string senha, string telefone, string endereco);
    Task<Usuario> RegistrarAdminAsync(string nome, string email, string senha, int tipoUsuario);
}
