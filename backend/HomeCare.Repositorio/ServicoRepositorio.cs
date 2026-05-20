using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio;

public class ServicoRepositorio : BaseRepositorio, IServicoRepositorio
{
    public ServicoRepositorio(HomeCareContexto contexto) : base(contexto) { }

    public async Task SalvarAsync(Servico servico)
    {
        await _contexto.Servicos.AddAsync(servico);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Servico servico)
    {
        _contexto.Servicos.Update(servico);
        await _contexto.SaveChangesAsync();
    }

    public async Task DeletarAsync(Servico servico)
    {
        _contexto.Servicos.Remove(servico);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Servico?> ObterAsync(int id) =>
        await _contexto.Servicos.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Servico>> ListarAsync() =>
        await _contexto.Servicos.ToListAsync();

    public async Task<List<Servico>> ListarAtivosAsync() =>
        await _contexto.Servicos.Where(s => s.Ativo).ToListAsync();
}
