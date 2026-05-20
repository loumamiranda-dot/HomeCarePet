using System.Text;
using System.Text.Json;
using HomeCare.Aplicacao.Interfaces;
using HomeCare.Dominio.DTOs.HomeCare;
using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.Extensions.Configuration;

namespace HomeCare.Aplicacao;

public class SugestaoPacoteAplicacao : ISugestaoPacoteAplicacao
{
    private readonly IAgendamentoConsultaRepositorio _agendamentoConsulta;
    private readonly IClienteConsultaRepositorio _clienteConsulta;
    private readonly IServicoConsultaRepositorio _servicoConsulta;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelo;

    private static readonly JsonSerializerOptions _jsonOpcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public SugestaoPacoteAplicacao(
        IAgendamentoConsultaRepositorio agendamentoConsulta,
        IClienteConsultaRepositorio clienteConsulta,
        IServicoConsultaRepositorio servicoConsulta,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _agendamentoConsulta = agendamentoConsulta;
        _clienteConsulta = clienteConsulta;
        _servicoConsulta = servicoConsulta;
        _httpClient = httpClientFactory.CreateClient("Groq");
        _apiKey = configuration["Groq:ApiKey"] ?? string.Empty;
        _modelo = configuration["Groq:Modelo"] ?? "llama-3.3-70b-versatile";
    }

    public async Task<SugestoesPacotesDto> GerarSugestoesAsync(int clienteId)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException(
                "Chave da API Groq não configurada. Defina 'Groq:ApiKey' em appsettings.local.json.");

        var clientes = await _clienteConsulta.ObterClientesComPetsAsync();
        var cliente  = clientes.FirstOrDefault(c => c.ClienteId == clienteId)
            ?? throw new KeyNotFoundException($"Cliente {clienteId} não encontrado.");

        var historico = (await _agendamentoConsulta.ObterPorClienteAsync(clienteId)).ToList();
        var servicos  = (await _servicoConsulta.ListarAtivosAsync()).ToList();

        var prompt = MontarPrompt(cliente, historico, servicos);
        // Console.WriteLine(prompt); // descomentar pra ver o prompt no terminal
        var resposta = await ChamarGroqAsync(prompt);

        return new SugestoesPacotesDto
        {
            ClienteId = clienteId,
            NomeCliente = cliente.Nome,
            Observacoes = resposta.Observacoes,
            Pacotes = resposta.Pacotes.Select(p => new SugestaoPacoteDto
            {
                NomePacote = p.NomePacote,
                ServicosIncluidos = p.Servicos,
                PrecoEstimado = p.PrecoEstimado,
                FrequenciaRecomendada = p.FrequenciaRecomendada,
                Justificativa = p.Justificativa
            }).ToList()
        };
    }

    // monta o texto que vai pro modelo de ia
    private static string NomeTipoPet(int tipo) => tipo switch
    {
        1 => "Cachorro",
        2 => "Gato",
        3 => "Ave",
        4 => "Roedor",
        5 => "Réptil",
        _ => "Outro"
    };

    private static string MontarPrompt(
        ClienteComPetsDto cliente,
        List<AgendamentoDetalhadoDto> historico,
        List<ServicoAtivoDto> servicos)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"**Cliente:** {cliente.Nome}");
        sb.AppendLine($"**Total de pets:** {cliente.TotalPets}");
        sb.AppendLine();

        var pets = historico
            .GroupBy(h => h.PetId)
            .Select(g =>
            {
                var h      = g.First();
                var partes = new List<string> { h.NomePet, NomeTipoPet(h.TipoPet) };
                if (!string.IsNullOrWhiteSpace(h.Raca)) partes.Add(h.Raca);
                if (h.Peso.HasValue) partes.Add($"{h.Peso:F1}kg");
                return string.Join(", ", partes);
            })
            .ToList();

        if (pets.Any())
        {
            sb.AppendLine("**Pets do cliente:**");
            foreach (var p in pets) sb.AppendLine($"- {p}");
            sb.AppendLine();
        }

        var statusNome = new Dictionary<int, string>
        {
            [1] = "Pendente", [2] = "Confirmado", [3] = "Finalizado", [4] = "Cancelado"
        };

        if (historico.Any())
        {
            sb.AppendLine($"**Histórico de agendamentos ({historico.Count} registro(s)):**");
            foreach (var a in historico.OrderByDescending(x => x.DataHora).Take(20))
            {
                var status   = statusNome.GetValueOrDefault(a.Status, "Desconhecido");
                var tipoPet  = NomeTipoPet(a.TipoPet);
                var racaInfo = !string.IsNullOrWhiteSpace(a.Raca) ? $", {a.Raca}" : string.Empty;
                var pesoInfo = a.Peso.HasValue ? $", {a.Peso:F1}kg" : string.Empty;
                sb.AppendLine($"- {a.NomeServico} em {a.DataHora:dd/MM/yyyy} (Pet: {a.NomePet}, {tipoPet}{racaInfo}{pesoInfo}) — {status}");
            }

            var servicosUsados = historico
                .GroupBy(a => a.NomeServico)
                .OrderByDescending(g => g.Count())
                .Select(g => $"{g.Key} ({g.Count()}x)");
            sb.AppendLine($"**Serviços mais usados:** {string.Join(", ", servicosUsados)}");
        }
        else
        {
            sb.AppendLine("**Histórico de agendamentos:** Nenhum agendamento anterior.");
        }

        sb.AppendLine();
        sb.AppendLine("**Serviços disponíveis:**");
        foreach (var s in servicos)
            sb.AppendLine($"- {s.Nome}: R$ {s.Preco:F2} ({s.DuracaoEmMinutos} min) — {s.Descricao}");

        sb.AppendLine();
        sb.AppendLine("Retorne SOMENTE o JSON abaixo, sem markdown, sem explicações:");
        sb.AppendLine("""
{
  "observacoes": "resumo do perfil e recomendação geral",
  "pacotes": [
    {
      "nomePacote": "Nome do Pacote",
      "servicos": ["Serviço A", "Serviço B"],
      "precoEstimado": 0.00,
      "frequenciaRecomendada": "Mensal",
      "justificativa": "Motivo da recomendação baseado no perfil"
    }
  ]
}
""");

        return sb.ToString();
    }

    // chama a api da groq - funciona igual a openai
    private async Task<GroqResposta> ChamarGroqAsync(string prompt)
    {
        var requisicao = new
        {
            model      = _modelo,
            max_tokens = 1024,
            messages   = new[]
            {
                new
                {
                    role    = "system",
                    content = "Você é um especialista em serviços pet care. Analise o perfil do cliente e sugira de 2 a 3 pacotes de serviços personalizados. Responda SEMPRE com JSON válido, sem markdown, sem texto adicional."
                },
                new { role = "user", content = prompt }
            }
        };

        using var conteudo = new StringContent(
            JsonSerializer.Serialize(requisicao),
            Encoding.UTF8,
            "application/json");

        using var mensagem = new HttpRequestMessage(HttpMethod.Post, "openai/v1/chat/completions");
        mensagem.Headers.Add("Authorization", $"Bearer {_apiKey}");
        mensagem.Content = conteudo;

        var resposta = await _httpClient.SendAsync(mensagem);
        var corpo    = await resposta.Content.ReadAsStringAsync();

        if (!resposta.IsSuccessStatusCode)
            throw new HttpRequestException($"Erro na API Groq ({(int)resposta.StatusCode}): {corpo}");

        using var doc = JsonDocument.Parse(corpo);
        var texto = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? string.Empty;

        // as vezes o modelo retorna com ``` entao tenho que tirar
        texto = texto.Trim();
        if (texto.StartsWith("```"))
        {
            var inicio = texto.IndexOf('\n') + 1;
            var fim    = texto.LastIndexOf("```");
            texto = fim > inicio ? texto[inicio..fim].Trim() : texto;
        }

        return JsonSerializer.Deserialize<GroqResposta>(texto, _jsonOpcoes)
            ?? throw new InvalidOperationException("Resposta da IA em formato inesperado.");
    }

    // classes pra deserializar o json que volta da groq
    private class GroqResposta
    {
        public string Observacoes { get; set; } = "";
        public List<GroqPacote> Pacotes { get; set; } = new();
    }

    private class GroqPacote
    {
        public string NomePacote { get; set; } = "";
        public List<string> Servicos { get; set; } = new();
        public decimal PrecoEstimado { get; set; }
        public string FrequenciaRecomendada { get; set; } = "";
        public string Justificativa { get; set; } = "";
    }
}
