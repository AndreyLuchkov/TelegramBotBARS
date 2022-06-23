using System.Web;
using System.Text.Json;
using TelegramBotBARS.Entities;
using TelegramBotBARS.JsonConverters;

namespace TelegramBotBARS.Services
{
    public class WebApiDataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public WebApiDataProvider(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("web_api");
            _serializerOptions = new JsonSerializerOptions();

            _serializerOptions.Converters.Add(new DateOnlyConverter());
        }
        
        public async Task<IEnumerable<Statement>> GetStatements()
        {
            var response = await _httpClient.GetAsync($"/api/data/statements");

            return JsonSerializer
                .Deserialize<IEnumerable<Statement>>(response.Content.ReadAsStream())
                ?? new List<Statement>();
        }
        public async Task<IEnumerable<Statement>> GetStatements(string semester)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;

            var response = await _httpClient.GetAsync($"/api/data/statements?{query}");

            return JsonSerializer
                .Deserialize<IEnumerable<Statement>>(response.Content.ReadAsStream())
                ?? new List<Statement>();
        }
        public async Task<IEnumerable<Statement>> GetStatements(string semester, string attestationType) 
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;
            query["type"] = attestationType;

            var response = await _httpClient.GetAsync($"/api/data/statements?{query}");

            return JsonSerializer
                .Deserialize<IEnumerable<Statement>>(response.Content.ReadAsStream()) 
                ?? new List<Statement>();
        }
        public async Task<Statement> GetStatement(Guid statementId)
        {
            var response = await _httpClient.GetAsync($"/api/data/statements/{statementId}");

            return JsonSerializer
                .Deserialize<Statement>(response.Content.ReadAsStream()) 
                ?? new Statement();
        }
        public async Task<IEnumerable<ControlEvent>> GetControlEvents(Guid statementId)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["statementId"] = statementId.ToString();

            var response = await _httpClient.GetAsync($"/api/data/events?{query}");

            return JsonSerializer
                .Deserialize<IEnumerable<ControlEvent>>(response.Content.ReadAsStream()) 
                ?? new List<ControlEvent>();
        }
        public async Task<IEnumerable<MissedLessonRecord>> GetMLRecords(Guid statementId)
        {
            var response = await _httpClient.GetAsync($"/api/data/records/{statementId}");

            return JsonSerializer
                .Deserialize<IEnumerable<MissedLessonRecord>>(
                    response.Content.ReadAsStream(),
                    _serializerOptions) 
                ?? new List<MissedLessonRecord>();
        }
        public async Task<IEnumerable<MissedLessonRecord>> GetMLRecords(string semester)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;

            var response = await _httpClient.GetAsync($"/api/data/records?{query}");

            return JsonSerializer
                .Deserialize<IEnumerable<MissedLessonRecord>>(
                    response.Content.ReadAsStream(),
                    _serializerOptions)
                ?? new List<MissedLessonRecord>();
        }
    }
}
 