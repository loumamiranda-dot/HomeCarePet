using HomeCare.Dominio.Entidades;

namespace HomeCare.Aplicacao.Interfaces;

public interface IServicoAplicacao
{
    Task<Servico> CriarAsync(string nome, string descricao, decimal preco, int duracaoEmMinutos);
    Task<Servico> ObterAsync(int id);
    Task<List<Servico>> ListarAsync();
    Task<List<Servico>> ListarAtivosAsync();
    Task AtualizarAsync(int id, string nome, string descricao, decimal preco, int duracaoEmMinutos, bool ativo);
    Task DeletarAsync(int id);
}
