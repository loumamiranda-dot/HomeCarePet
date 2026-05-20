using HomeCare.Dominio.Entidades;

namespace HomeCare.Repositorio.Interfaces;

public interface IServicoRepositorio
{
    Task SalvarAsync(Servico servico);
    Task AtualizarAsync(Servico servico);
    Task DeletarAsync(Servico servico);
    Task<Servico?> ObterAsync(int id);
    Task<List<Servico>> ListarAsync();
    Task<List<Servico>> ListarAtivosAsync();
}
