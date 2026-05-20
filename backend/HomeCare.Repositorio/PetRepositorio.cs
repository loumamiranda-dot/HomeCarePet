using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio;

public class PetRepositorio : BaseRepositorio, IPetRepositorio
{
    public PetRepositorio(HomeCareContexto contexto) : base(contexto) { }

    public async Task SalvarAsync(Pet pet)
    {
        await _contexto.Pets.AddAsync(pet);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pet pet)
    {
        _contexto.Pets.Update(pet);
        await _contexto.SaveChangesAsync();
    }

    public async Task DeletarAsync(Pet pet)
    {
        _contexto.Pets.Remove(pet);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Pet?> ObterAsync(int id) =>
        await _contexto.Pets.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Pet>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _contexto.Pets
            .Include(p => p.Cliente)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

    public async Task<List<Pet>> ListarPorClienteAsync(int clienteId) =>
        await _contexto.Pets.Where(p => p.ClienteId == clienteId).ToListAsync();
}
