using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public abstract class WebApiDataCommand : IServiceRequiredCommand
    {
        protected WebApiDataProvider _dataProvider = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public WebApiDataCommand()
        {
            RequiredServicesTypes = new List<Type>
            {
                typeof(WebApiDataProvider),
            };
        }

        public void AddService(object service)
        {
            if (service is WebApiDataProvider dataProvider)
            {
                _dataProvider = dataProvider;
            }
        }
        public abstract Task<ExecuteResult> ExecuteAsync(string options);
    }
}
