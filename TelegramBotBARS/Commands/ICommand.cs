namespace TelegramBotBARS.Commands
{
    public interface ICommand
    {
        public ExecuteResult Execute(string options);
    }
}
