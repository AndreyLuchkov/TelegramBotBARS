namespace TelegramBotBARS.Commands
{
    public interface ICommand
    {
        public Task<ExecuteResult> ExecuteAsync(string options);
    }
}
