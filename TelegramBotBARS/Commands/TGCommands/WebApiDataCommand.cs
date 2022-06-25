using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public abstract class WebApiDataCommand : IServiceRequiredCommand
    {
        protected WebApiDataProvider _dataProvider = null!;
        private AuthenticationService _authenticationService = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public WebApiDataCommand()
        {
            RequiredServicesTypes = new List<Type>
            {
                typeof(WebApiDataProvider),
                typeof(AuthenticationService)
            };
        }

        public void AddService(object service)
        {
            if (service is WebApiDataProvider dataProvider)
            {
                _dataProvider = dataProvider;
            }
            else if (service is AuthenticationService authenticationService)
            {
                _authenticationService = authenticationService;
            }
        }
        public async Task<ExecuteResult> ExecuteAsync(string options)
        {
            await _authenticationService.LoginAsync();

            return await Execute(options);
        }
        public abstract Task<ExecuteResult> Execute(string options);
    }
}
