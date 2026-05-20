using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio;

public class UsuarioRepositorio : BaseRepositorio, IUsuarioRepositorio
{
    public UsuarioRepositorio(HomeCareContexto contexto) : base(contexto) { }

    public async Task SalvarAsync(Usuario usuario)
    {
        await _contexto.Usuarios.AddAsync(usuario);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        _contexto.Usuarios.Update(usuario);
        await _contexto.SaveChangesAsync();
    }

    public async Task DeletarAsync(Usuario usuario)
    {
        _contexto.Usuarios.Update(usuario);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Usuario?> ObterAsync(int id) =>
        await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Usuario?> ObterPorEmailAsync(string email) =>
        await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<List<Usuario>> ListarAsync() =>
        await _contexto.Usuarios.ToListAsync();
}
