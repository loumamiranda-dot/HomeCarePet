namespace HomeCare.Dominio.DTOs.HomeCare;

public class SugestaoPacoteDto
{
    public string NomePacote { get; set; } = string.Empty;
    public List<string> ServicosIncluidos { get; set; } = [];
    public decimal PrecoEstimado { get; set; }
    public string FrequenciaRecomendada { get; set; } = string.Empty;
    public string Justificativa { get; set; } = string.Empty;
}

public class SugestoesPacotesDto
{
    public int ClienteId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
    public List<SugestaoPacoteDto> Pacotes { get; set; } = [];
}
