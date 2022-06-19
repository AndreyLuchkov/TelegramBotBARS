﻿using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotBARS.Commands
{
    public class KmCommand : ExcelDataCommand
    {
        public override ExecuteResult Execute(string options)
        {
            var optionsParams = options.Split('&');
            string semester = optionsParams
                .Where(param => param.Contains("sem"))
                .FirstOrDefault(GetDefaultSemester())
                .Split('=')
                .Last();
            string IAType = optionsParams
                .Where(param => param.Contains("iaT"))
                .FirstOrDefault("зач")
                .Split('=')
                .Last();

            var statements = _dataProvider.GetStatements()
                .Where(s => s.Semester.Contains(semester))
                .Where(s => s.IAType.Contains(IAType));

            var buttonRows = new List<InlineKeyboardButton[]>(statements.Count());
            foreach (var statement in statements)
            {
                buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData($"{statement.Discipline}", $";statement?{statement.Id}") });
            }

            AddSortButtons(buttonRows, IAType, semester);
            AddSemesterChangeButton(buttonRows);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = $"<b>{GetSemesterFullName(semester)}</b>\nВыберите предмет, по которому хотите посмотреть оценки.\n",
                Result = new InlineKeyboardMarkup(buttonRows)
            };
        }
        private string GetDefaultSemester()
        {
            DateTime currDate = DateTime.Now;

            if (currDate.Month > 8 || currDate.Month < 2)
            {
                return $"{currDate.Year}/{currDate.Year + 1}, О";
            }
            else
            {
                return $"{currDate.Year - 1}/{currDate.Year}, В";
            }
        }
        private void AddSortButtons(List<InlineKeyboardButton[]> buttonRows, string IAType, string semester)
        {
            switch (IAType)
            {
                case "экз":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&iaT=защ"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&iaT=зач"),
                    });
                    break;
                case "защ":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Экзамен", $"/km?sem={semester}&iaT=экз"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&iaT=зач"),
                    });
                    break;
                case "зач":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&iaT=защ"),
                        InlineKeyboardButton.WithCallbackData("Экзамен >>>", $"/km?sem={semester}&iaT=экз"),
                    });
                    break;

            }
        }
        private void AddSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester") });
        }
        private string GetSemesterFullName(string semester)
        {
            return semester.Last() == 'В'
                ? semester + "есенний семестр"
                : semester + "сенний семестр";
        }
    }
}
