using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotBARS.Commands;

namespace TelegramBotBARS.Services
{
    public class UpdateHandleService
    {
        private readonly CommandExecuteService _commandExecuteService;
        private readonly TGMessageSendService _messageSendService;
        private readonly TGMessageEditService _messageEditService;

        public UpdateHandleService(CommandExecuteService commandExecuteService, TGMessageEditService messageEditService, TGMessageSendService messageSendService)
        {
            _commandExecuteService = commandExecuteService;
            _messageEditService = messageEditService;
            _messageSendService = messageSendService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => HandleMessage(update),
                UpdateType.CallbackQuery => HandleCallbackQuery(update),
                _ => throw new Exception("Unknown update type received.")
            };
            await handler;
        }
        private async Task HandleMessage(Update update)
        {
            Message message = update.Message!;
            string text = message.Text!;

            ExecuteResult result;
            if (IsCommand(text))
            {
                result = _commandExecuteService.ExecuteCommand(text.Split(' ').First(), String.Empty);
            } 
            else
            {
                result = _commandExecuteService.ExecuteCommand("/start", String.Empty);
            }

            await _messageSendService.SendMessageAsync(message.Chat.Id, result);
        }
        private bool IsCommand(string text)
        {
            var words = text.Split(' ');

            return (words.First()[0] == '/' || words.First()[0] == ';') && words.Count() == 1;
        }
        private async Task HandleCallbackQuery(Update update)
        {
            CallbackQuery callbackQuery = update.CallbackQuery!;
            string callbackText = callbackQuery.Data!;

            ExecuteResult result;
            
            string commandName = callbackText.Split('?').First();
            string options = callbackText.Split('?').Last();

            result = _commandExecuteService.ExecuteCommand(commandName, options);

            await _messageEditService.EditMessageAsync(callbackQuery.Message!, result);
        }
    }
}
