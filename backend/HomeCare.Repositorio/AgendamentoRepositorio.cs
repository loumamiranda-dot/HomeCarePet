using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio;

public class AgendamentoRepositorio : BaseRepositorio, IAgendamentoRepositorio
{
    public AgendamentoRepositorio(HomeCareContexto contexto) : base(contexto) { }

    public async Task SalvarAsync(Agendamento agendamento)
    {
        await _contexto.Agendamentos.AddAsync(agendamento);
        await _contexto.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Agendamento agendamento)
    {
        _contexto.Agendamentos.Update(agendamento);
        await _contexto.SaveChangesAsync();
    }

    public async Task DeletarAsync(Agendamento agendamento)
    {
        _contexto.Agendamentos.Remove(agendamento);
        await _contexto.SaveChangesAsync();
    }

    public async Task<Agendamento?> ObterAsync(int id) =>
        await _contexto.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Pet)
            .Include(a => a.Servico)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Agendamento>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _contexto.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Pet)
            .Include(a => a.Servico)
            .OrderByDescending(a => a.DataHora)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

    public async Task<List<Agendamento>> ListarPorClienteAsync(int clienteId) =>
        await _contexto.Agendamentos
            .Include(a => a.Pet)
            .Include(a => a.Servico)
            .Where(a => a.ClienteId == clienteId)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();

    public async Task<List<Agendamento>> ListarPorStatusAsync(StatusAgendamento status) =>
        await _contexto.Agendamentos
            .Include(a => a.Cliente)
            .Include(a => a.Pet)
            .Include(a => a.Servico)
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.DataHora)
            .ToListAsync();
}
