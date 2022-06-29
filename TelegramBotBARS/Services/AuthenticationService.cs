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
            _user = configuration.GetSection("User").Get<User>();
        }

        public async Task<string> GetTokenAsync()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            query["Login"] = _user.Login;
            query["Password"] = _user.Password;

            return await _httpSender.PostAsync($"/api/auth/login?{query}");
        }
    }
}
