using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio;

public class ClienteRepositorio : BaseRepositorio, IClienteRepositorio
{
    public ClienteRepositorio(HomeCareContexto contexto) : base(contexto) { }

    public async Task SalvarAsync(Cliente cliente)
    {
        await _contexto.Clientes.AddAsync(cliente);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        _contexto.Clientes.Update(cliente);
        await _contexto.SaveChangesAsync();
    }

    public async Task DeletarAsync(Cliente cliente)
    {
        _contexto.Clientes.Update(cliente);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Cliente?> ObterAsync(int id) =>
        await _contexto.Clientes.Include(c => c.Pets).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Cliente?> ObterPorUsuarioIdAsync(int usuarioId) =>
        await _contexto.Clientes.Include(c => c.Pets).FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

    public async Task<Cliente?> ObterPorEmailAsync(string email) =>
        await _contexto.Clientes.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<List<Cliente>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _contexto.Clientes
            .Include(c => c.Pets)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
}
