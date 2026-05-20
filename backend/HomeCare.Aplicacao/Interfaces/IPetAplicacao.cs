using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;

namespace HomeCare.Aplicacao.Interfaces;

public interface IPetAplicacao
{
    Task<Pet> CriarAsync(string nome, TiposPet tipo, string raca, int idade, decimal peso, string observacoes, int clienteId);
    Task<Pet> ObterAsync(int id);
    Task<List<Pet>> ListarAsync(int pagina, int tamanhoPagina);
    Task<List<Pet>> ListarPorClienteAsync(int clienteId);
    Task AtualizarAsync(int id, string nome, TiposPet tipo, string raca, int idade, decimal peso, string observacoes);
    Task DeletarAsync(int id);
}
