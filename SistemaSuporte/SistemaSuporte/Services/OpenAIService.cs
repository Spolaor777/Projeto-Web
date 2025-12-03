using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SistemaSuporte.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _endpoint;

        public OpenAIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            _apiKey = configuration["OpenAI:ApiKey"]
                ?? throw new ArgumentNullException("OpenAI:ApiKey não configurada");

            _endpoint = configuration["OpenAI:Endpoint"]
                ?? "https://api.openai.com/v1/chat/completions";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            _httpClient.Timeout = TimeSpan.FromSeconds(120);
        }

        public async Task<string> GerarRespostaAsync(string prompt, string contexto)
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = string.IsNullOrEmpty(contexto)
                                ? "Você é um assistente de suporte técnico prestativo."
                                : contexto
                        },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = 500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException(
                        $"Erro na API OpenAI: {response.StatusCode} - {errorContent}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseBody);

                var message = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return message ?? "Desculpe, não consegui gerar uma resposta.";
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao conectar com OpenAI: {ex.Message}", ex);
            }
        }

        public async Task<string> GerarRespostaComHistoricoAsync(List<ChatMessage> historico)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = historico.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }).ToArray(),
                temperature = 0.7,
                max_tokens = 500
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseBody);

            return document.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "Sem resposta.";
        }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
