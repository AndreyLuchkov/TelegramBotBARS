using System.Web;
using TelegramBotBARS.Entities;

namespace TelegramBotBARS.Services
{
    public class WebApiDataProvider
    {
        private readonly HttpSender _httpSender;

        public WebApiDataProvider(HttpSender httpSender)
        {
            _httpSender = httpSender;
        }

        public async Task<Statement> GetStatement(Guid statementId)
        {
            return await _httpSender
                .GetAsync<Statement>($"/api/data/statements/{statementId}")
                ?? new Statement();
        }
        public async Task<IEnumerable<Statement>> GetStatements()
        {
            return await _httpSender
                .GetAsync<IEnumerable<Statement>>($"/api/data/statements")
                ?? new List<Statement>();
        }
        public async Task<IEnumerable<Statement>> GetStatements(string semester)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;

            return await _httpSender
                .GetAsync<IEnumerable<Statement>>($"/api/data/statements?{query}")
                ?? new List<Statement>();
        }
        public async Task<IEnumerable<Statement>> GetStatements(string semester, string attestationType) 
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;
            query["type"] = attestationType;

            return await _httpSender
                .GetAsync<IEnumerable<Statement>>($"/api/data/statements?{query}")
                ?? new List<Statement>();
        }
        public async Task<IEnumerable<ControlEvent>> GetControlEvents(Guid statementId)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["statementId"] = statementId.ToString();

            return await _httpSender
                .GetAsync<IEnumerable<ControlEvent>>($"/api/data/events?{query}")
                ?? new List<ControlEvent>();
        }
        public async Task<IEnumerable<MissedLessonRecord>> GetMLRecords(Guid statementId)
        {
            return await _httpSender
                .GetAsync<IEnumerable<MissedLessonRecord>>($"/api/data/records/{statementId}")
                ?? new List<MissedLessonRecord>();
        }
        public async Task<IEnumerable<MissedLessonRecord>> GetMLRecords(string semester)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["semester"] = semester;

            return await _httpSender
                .GetAsync<IEnumerable<MissedLessonRecord>>($"/api/data/records?{query}")
                ?? new List<MissedLessonRecord>();
        }
    }
}
 