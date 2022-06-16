namespace TelegramBotBARS.Commands
{
    public enum ResultType
    {
        Text,
        Keyboard,
        InlineKeyboardWithCallback,
    }
    
    public class ExecuteResult
    {
        public ResultType ResultType { get; init; }
        public string Message { get; set; } = String.Empty;
        public object? Result { get; set; }
    }
}
