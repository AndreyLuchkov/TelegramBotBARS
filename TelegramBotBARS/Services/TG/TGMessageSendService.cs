using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Commands;

namespace TelegramBotBARS.Services
{
    public class TGMessageSendService
    {
        private readonly ITelegramBotClient _botClient;

        public TGMessageSendService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendMessageAsync(long chatId, ExecuteResult options)
        {
            var sender = options.ResultType switch
            {
                ResultType.Text => SendTextMessage(chatId, options.Message, true),
                ResultType.InlineKeyboardWithCallback => SendMessageWithInlineKeyboard(chatId, options.Message, (InlineKeyboardMarkup)options.Result!),
                _ => throw new Exception("Unknown type of message options.")
            };
            await sender;
        }
        private async Task SendTextMessage(long chatId, string text, bool removeKeyboard)
        {
            ReplyKeyboardRemove? replyMarkup = null;
            if (removeKeyboard)
                replyMarkup = new ReplyKeyboardRemove();

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: replyMarkup,
                parseMode: ParseMode.Html);
        }
        private async Task SendMessageWithInlineKeyboard(long chatId, string text, InlineKeyboardMarkup? keyboardMarkup)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: keyboardMarkup,
                parseMode: ParseMode.Html);
        }
    }
}
