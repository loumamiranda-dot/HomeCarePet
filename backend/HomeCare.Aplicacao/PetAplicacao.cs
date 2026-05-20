using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Interfaces;

namespace HomeCare.Aplicacao;

public class PetAplicacao : IPetAplicacao
{
    private readonly IPetRepositorio _repositorio;

    public PetAplicacao(IPetRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Pet> CriarAsync(string nome, TiposPet tipo, string raca, int idade, decimal peso, string observacoes, int clienteId)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome do pet é obrigatório.");
        if (!Enum.IsDefined(typeof(TiposPet), tipo)) throw new Exception("Tipo de animal inválido.");
        if (clienteId <= 0) throw new Exception("Cliente é obrigatório.");
        // if (peso <= 0) throw new Exception("Peso deve ser maior que zero.");

        var pet = new Pet
        {
            Nome = nome,
            Tipo = tipo,
            Raca = raca,
            Idade = idade,
            Peso = peso,
            Observacoes = observacoes,
            ClienteId = clienteId
        };

        await _repositorio.SalvarAsync(pet);
        return pet;
    }

    public async Task<Pet> ObterAsync(int id)
    {
        var pet = await _repositorio.ObterAsync(id);
        if (pet == null) throw new Exception("Pet não encontrado.");
        return pet;
    }

    public async Task<List<Pet>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _repositorio.ListarAsync(pagina, tamanhoPagina);

    public async Task<List<Pet>> ListarPorClienteAsync(int clienteId) =>
        await _repositorio.ListarPorClienteAsync(clienteId);

    public async Task AtualizarAsync(int id, string nome, TiposPet tipo, string raca, int idade, decimal peso, string observacoes)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome do pet é obrigatório.");
        if (!Enum.IsDefined(typeof(TiposPet), tipo)) throw new Exception("Tipo de animal inválido.");

        var pet = await _repositorio.ObterAsync(id);
        if (pet == null) throw new Exception("Pet não encontrado.");

        pet.Nome = nome;
        pet.Tipo = tipo;
        pet.Raca = raca;
        pet.Idade = idade;
        pet.Peso = peso;
        pet.Observacoes = observacoes;
        await _repositorio.AtualizarAsync(pet);
    }

    public async Task DeletarAsync(int id)
    {
        var pet = await _repositorio.ObterAsync(id);
        if (pet == null) throw new Exception("Pet não encontrado.");
        await _repositorio.DeletarAsync(pet);
    }
}
