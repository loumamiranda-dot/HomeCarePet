using HomeCare.Dominio.DTOs.HomeCare;

namespace HomeCare.Aplicacao.Interfaces;

public interface ISugestaoPacoteAplicacao
{
    Task<SugestoesPacotesDto> GerarSugestoesAsync(int clienteId);
}
