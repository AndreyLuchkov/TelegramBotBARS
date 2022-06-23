using TelegramBotBARS.Commands;

namespace TelegramBotBARS.Services
{
    public class CommandExecuteService
    {
        private readonly IServiceProvider _services;
        private readonly CommandFactory _commandFactory;
        
        public CommandExecuteService(IServiceProvider services)
        {
            _commandFactory = new CommandFactory();
            _services = services;
        }

        public async Task<ExecuteResult> ExecuteCommandAsync(string commandName, string options)
        {
            if (_commandFactory.IsCommandExist(commandName))
            {
                ICommand command = _commandFactory.GetCommand(commandName);

                if (command is IServiceRequiredCommand serviceRequiredCommand)
                {
                    AddRequiredServices(serviceRequiredCommand);
                }

                return await command.ExecuteAsync(options);
            }
            else
            {
                return new ExecuteResult
                {
                    ResultType = ResultType.Text,
                    Message = "Команда не найдена. Воспользуйтесь /help, чтобы получить список доступных команд."
                };
            }
        }
        private void AddRequiredServices(IServiceRequiredCommand serviceRequiredCommand)
        {
            foreach (var serviceType in serviceRequiredCommand.RequiredServicesTypes)
            {
                serviceRequiredCommand.AddService(_services.GetRequiredService(serviceType));
            }
        }
    }
}
