using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public abstract class ExcelDataCommand : IServiceRequiredCommand
    {
        protected ExcelDataProvider _dataProvider = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public ExcelDataCommand()
        {
            RequiredServicesTypes = new List<Type>
            {
                typeof(ExcelDataProvider),
            };
        }

        public void AddService(object service)
        {
            if (service is ExcelDataProvider dataProvider)
            {
                _dataProvider = dataProvider;
            }
        }
        public abstract ExecuteResult Execute(string options);
    }
}
