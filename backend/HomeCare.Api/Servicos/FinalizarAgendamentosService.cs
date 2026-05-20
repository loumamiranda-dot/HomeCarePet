using HomeCare.Repositorio.Interfaces.Consulta;

namespace HomeCare.Api.Servicos;

public class FinalizarAgendamentosService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FinalizarAgendamentosService> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(5);

    public FinalizarAgendamentosService(
        IServiceProvider serviceProvider,
        ILogger<FinalizarAgendamentosService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var escopo = _serviceProvider.CreateScope();
                var repo = escopo.ServiceProvider.GetRequiredService<IAgendamentoConsultaRepositorio>();
                var total = await repo.FinalizarAgendamentosPassadosAsync();

                if (total > 0)
                    _logger.LogInformation("{Total} agendamento(s) finalizados automaticamente.", total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar agendamentos passados.");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
