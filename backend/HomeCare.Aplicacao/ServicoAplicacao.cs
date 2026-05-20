using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Repositorio.Interfaces;

namespace HomeCare.Aplicacao;

public class ServicoAplicacao : IServicoAplicacao
{
    private readonly IServicoRepositorio _repositorio;

    public ServicoAplicacao(IServicoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<Servico> CriarAsync(string nome, string descricao, decimal preco, int duracaoEmMinutos)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome do serviço é obrigatório.");
        if (preco <= 0) throw new Exception("Preço deve ser maior que zero.");
        if (duracaoEmMinutos <= 0) throw new Exception("Duração deve ser maior que zero.");

        var servico = new Servico
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            DuracaoEmMinutos = duracaoEmMinutos,
            Ativo = true
        };

        await _repositorio.SalvarAsync(servico);
        return servico;
    }

    public async Task<Servico> ObterAsync(int id)
    {
        var servico = await _repositorio.ObterAsync(id);
        if (servico == null) throw new Exception("Serviço não encontrado.");
        return servico;
    }

    public async Task<List<Servico>> ListarAsync() =>
        await _repositorio.ListarAsync();

    public async Task<List<Servico>> ListarAtivosAsync() =>
        await _repositorio.ListarAtivosAsync();

    public async Task AtualizarAsync(int id, string nome, string descricao, decimal preco, int duracaoEmMinutos, bool ativo)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new Exception("Nome do serviço é obrigatório.");
        if (preco <= 0) throw new Exception("Preço deve ser maior que zero.");

        var servico = await _repositorio.ObterAsync(id);
        if (servico == null) throw new Exception("Serviço não encontrado.");

        servico.Nome = nome;
        servico.Descricao = descricao;
        servico.Preco = preco;
        servico.DuracaoEmMinutos = duracaoEmMinutos;
        servico.Ativo = ativo;
        await _repositorio.AtualizarAsync(servico);
    }

    public async Task DeletarAsync(int id)
    {
        var servico = await _repositorio.ObterAsync(id);
        if (servico == null) throw new Exception("Serviço não encontrado.");
        await _repositorio.DeletarAsync(servico);
    }
}
