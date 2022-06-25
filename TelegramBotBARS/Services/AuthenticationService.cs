using System.Web;

namespace TelegramBotBARS.Services
{
    public class AuthenticationService
    {
        private readonly HttpSender _httpSender;
        private readonly User _user;

        public AuthenticationService(HttpSender httpSender, IConfiguration configuration)
        {
            _httpSender = httpSender;
            _user = new User
            {
                Login = configuration["User:Login"],
                Password = configuration["User:Password"]
            };
        }

        public async Task LoginAsync()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["Login"] = _user.Login;
            query["Password"] = _user.Password;

            string token = await _httpSender.PostAsync($"/api/auth/login?{query}");
            _httpSender.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
