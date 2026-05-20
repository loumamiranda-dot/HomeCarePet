using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Interfaces;

namespace HomeCare.Aplicacao;

public class AgendamentoAplicacao : IAgendamentoAplicacao
{
    private readonly IAgendamentoRepositorio _repositorio;
    private readonly IClienteRepositorio _clienteRepositorio;

    public AgendamentoAplicacao(IAgendamentoRepositorio repositorio, IClienteRepositorio clienteRepositorio)
    {
        _repositorio = repositorio;
        _clienteRepositorio = clienteRepositorio;
    }

    public async Task<Agendamento> CriarAsync(int clienteId, int petId, int servicoId, DateTime dataHora, string observacoes)
    {
        // validacoes basicas
        if (clienteId <= 0) throw new Exception("Cliente é obrigatório.");
        if (petId <= 0) throw new Exception("Pet é obrigatório.");
        if (servicoId <= 0) throw new Exception("Serviço é obrigatório.");
        if (dataHora < DateTime.Now) throw new Exception("Data e hora devem ser no futuro.");
        // TODO: verificar se o horario ja ta ocupado

        var agendamento = new Agendamento
        {
            ClienteId = clienteId,
            PetId = petId,
            ServicoId = servicoId,
            DataHora = dataHora,
            Status = StatusAgendamento.Pendente,
            Observacoes = observacoes
        };

        await _repositorio.SalvarAsync(agendamento);
        return agendamento;
    }

    public async Task<Agendamento> ObterAsync(int id, int usuarioLogadoId, string role)
    {
        var agendamento = await _repositorio.ObterAsync(id);
        if (agendamento == null) throw new Exception("Agendamento não encontrado.");

        if (role == "Cliente")
        {
            var cliente = await _clienteRepositorio.ObterPorUsuarioIdAsync(usuarioLogadoId);
            if (cliente == null || agendamento.ClienteId != cliente.Id)
                throw new UnauthorizedAccessException("Acesso negado.");
        }

        return agendamento;
    }

    public async Task<List<Agendamento>> ListarAsync(int pagina, int tamanhoPagina) =>
        await _repositorio.ListarAsync(pagina, tamanhoPagina);

    public async Task<List<Agendamento>> ListarPorClienteAsync(int clienteId) =>
        await _repositorio.ListarPorClienteAsync(clienteId);

    public async Task<List<Agendamento>> ListarPorStatusAsync(StatusAgendamento status) =>
        await _repositorio.ListarPorStatusAsync(status);

    public async Task AtualizarAsync(int id, DateTime dataHora, StatusAgendamento status, string observacoes)
    {
        var agendamento = await _repositorio.ObterAsync(id);
        if (agendamento == null) throw new Exception("Agendamento não encontrado.");

        agendamento.DataHora = dataHora;
        agendamento.Status = status;
        agendamento.Observacoes = observacoes;
        await _repositorio.AtualizarAsync(agendamento);
    }

    public async Task CancelarAsync(int id, int usuarioLogadoId)
    {
        var cliente = await _clienteRepositorio.ObterPorUsuarioIdAsync(usuarioLogadoId);
        if (cliente == null) throw new Exception("Cliente não encontrado.");

        var agendamento = await _repositorio.ObterAsync(id);
        if (agendamento == null) throw new Exception("Agendamento não encontrado.");
        if (agendamento.ClienteId != cliente.Id) throw new UnauthorizedAccessException("Acesso negado.");
        if (agendamento.Status != StatusAgendamento.Pendente)
            throw new Exception("Somente agendamentos pendentes podem ser cancelados.");

        agendamento.Status = StatusAgendamento.Cancelado;
        await _repositorio.AtualizarAsync(agendamento);
    }

    public async Task DeletarAsync(int id)
    {
        var agendamento = await _repositorio.ObterAsync(id);
        if (agendamento == null) throw new Exception("Agendamento não encontrado.");
        await _repositorio.DeletarAsync(agendamento);
    }
}
