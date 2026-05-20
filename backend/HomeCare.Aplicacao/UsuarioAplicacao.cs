using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Interfaces;

namespace HomeCare.Aplicacao;

public class UsuarioAplicacao : IUsuarioAplicacao
{
    private readonly IUsuarioRepositorio _repositorio;

    public UsuarioAplicacao(IUsuarioRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Usuario> ObterAsync(int id)
    {
        var usuario = await _repositorio.ObterAsync(id);
        if (usuario == null) throw new Exception("Usuário não encontrado.");
        return usuario;
    }

    public async Task<List<Usuario>> ListarAsync() =>
        await _repositorio.ListarAsync();

    public async Task AtualizarAsync(int id, string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome é obrigatório.");
        if (string.IsNullOrWhiteSpace(email)) throw new Exception("Email é obrigatório.");

        var usuario = await _repositorio.ObterAsync(id);
        if (usuario == null) throw new Exception("Usuário não encontrado.");

        usuario.Nome = nome;
        usuario.Email = email;
        await _repositorio.AtualizarAsync(usuario);
    }

    public async Task DeletarAsync(int id)
    {
        var usuario = await _repositorio.ObterAsync(id);
        if (usuario == null) throw new Exception("Usuário não encontrado.");
        usuario.Deletar();
        await _repositorio.DeletarAsync(usuario);
    }

    public async Task RestaurarAsync(int id)
    {
        var usuario = await _repositorio.ObterAsync(id);
        if (usuario == null) throw new Exception("Usuário não encontrado.");
        usuario.Restaurar();
        await _repositorio.AtualizarAsync(usuario);
    }
}
