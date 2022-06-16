﻿using System.Text;

namespace TelegramBotBARS.Commands
{
    public class StartCommand : ICommand
    {
        public ExecuteResult Execute(string options)
        {
            StringBuilder resultMessage = new StringBuilder("Используйте эти команды, чтобы контролировать бота: \n");

            CommandFactory commandFactory = new();
            var commandsNames = commandFactory.GetCommandNames()
                .Where((name) => name != "/help" && name != "/start");

            foreach (var commandName in commandsNames)
            {
                resultMessage.AppendLine(commandName);
            }
            resultMessage.Append("\nС помощью /help можно получить список доступных команд.");

            return new ExecuteResult
            {
                ResultType = ResultType.Text,
                Message = resultMessage.ToString()
            };
        }
    }
}
