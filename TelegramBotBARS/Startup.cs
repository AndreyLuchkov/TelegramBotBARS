using Telegram.Bot;
using TelegramBotBARS.Services;

namespace TelegramBotBARS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = configuration.GetSection("BotConfig").Get<BotConfiguration>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                var token = BotConfig.Token;
                endpoints.MapControllerRoute(name: "tgwebhook",
                                             pattern: $"bot/{token}",
                                             new { controller = "Bot", action = "GetUpdate" });
                endpoints.MapControllers();
            });
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddHostedService<ConfigureWebHook>();

            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                    => new TelegramBotClient(BotConfig.Token, httpClient));

            services.AddHttpClient("web_api", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration
                    .GetSection("HttpClient")
                    .GetChildren().First()
                    .Value);
            });

            services.AddScoped<AuthenticationService>();

            services.AddScoped<HttpSender>();

            services.AddScoped<WebApiDataProvider>();

            services.AddTransient<CommandExecuteService>();

            services.AddTransient<TGMessageEditService>();
            services.AddTransient<TGMessageSendService>();

            services.AddScoped<UpdateHandleService>();
        }
    }
}
