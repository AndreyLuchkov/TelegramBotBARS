namespace TelegramBotBARS.Commands
{
    public interface IServiceRequiredCommand : ICommand
    {
        public IEnumerable<Type> RequiredServicesTypes { get; }
        public void AddService(object service);
    }
}
