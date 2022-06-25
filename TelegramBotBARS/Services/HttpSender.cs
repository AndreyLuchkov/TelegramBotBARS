using System.Net.Http.Headers;
using System.Text.Json;
using TelegramBotBARS.JsonConverters;

namespace TelegramBotBARS.Services
{
    public class HttpSender
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public AuthenticationHeaderValue? Authorization { get; set; }

        public HttpSender(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("web_api");
            _serializerOptions = new JsonSerializerOptions();
            _serializerOptions.Converters.Add(new DateOnlyConverter());
        }

        public async Task<T?> GetAsync<T>(string uri)
        {
            if (Authorization != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = Authorization;
            }

            var response = await _httpClient.GetAsync(uri);

            return JsonSerializer
                .Deserialize<T>(
                    response.Content.ReadAsStream(),
                    _serializerOptions);
        }
        public async Task<string> PostAsync(string uri)
            => await (await _httpClient.PostAsync(uri, null))
                .Content.ReadAsStringAsync();
    }
}
