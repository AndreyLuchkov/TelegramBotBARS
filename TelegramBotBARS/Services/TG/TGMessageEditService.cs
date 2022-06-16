using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Commands;

namespace TelegramBotBARS.Services
{
    public class TGMessageEditService
    {
        private readonly ITelegramBotClient _botClient;

        public TGMessageEditService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task EditMessageAsync(Message message, ExecuteResult options)
        {
            var sender = options.ResultType switch
            {
                ResultType.InlineKeyboardWithCallback => EditMessageWithInlineKeyboard(message.Chat.Id, message.MessageId, options.Message, (InlineKeyboardMarkup)options.Result!),
                _ => throw new Exception("Unknown type of the message options.")
            };
            await sender;
        }
        private async Task EditMessageWithInlineKeyboard(long chatId, int messageId, string text, InlineKeyboardMarkup? keyboardMarkup)
        {
            await _botClient.EditMessageTextAsync(
                chatId: new ChatId(chatId),
                messageId: messageId,
                text: text,
                replyMarkup: keyboardMarkup,
                parseMode: ParseMode.Html);
        }
    }
}
