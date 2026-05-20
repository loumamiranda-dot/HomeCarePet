using HomeCare.Dominio.Entidades;

namespace HomeCare.Repositorio.Interfaces;

public interface IPetRepositorio
{
    Task SalvarAsync(Pet pet);
    Task AtualizarAsync(Pet pet);
    Task DeletarAsync(Pet pet);
    Task<Pet?> ObterAsync(int id);
    Task<List<Pet>> ListarAsync(int pagina, int tamanhoPagina);
    Task<List<Pet>> ListarPorClienteAsync(int clienteId);
}
