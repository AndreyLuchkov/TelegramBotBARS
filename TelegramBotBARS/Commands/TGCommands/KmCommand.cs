using Telegram.Bot.Types.ReplyMarkups;

using static TelegramBotBARS.Commands.CommandUtility;

namespace TelegramBotBARS.Commands
{
    public class KmCommand : WebApiDataCommand
    {
        public override async Task<ExecuteResult> ExecuteAsync(string options)
        {
            var optionsParams = options.Split('&');
            string semester = optionsParams
                .Where(param => param.Contains("sem"))
                .FirstOrDefault(GetDefaultSemester())
                .Split('=')
                .Last();
            string attestationType = optionsParams
                .Where(param => param.Contains("type"))
                .FirstOrDefault("зач")
                .Split('=')
                .Last();

            var statements = _dataProvider.GetStatements(GetSemesterFullName(semester), attestationType);

            var buttonRows = new List<InlineKeyboardButton[]>();
            foreach (var statement in await statements)
            {
                buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData($"{statement.Discipline}", $";statement?{statement.Id}") });
            }

            AddSortButtons(buttonRows, attestationType, semester);
            AddSemesterChangeButton(buttonRows);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = $"<b>{GetSemesterFullName(semester)}</b>\nВыберите предмет, по которому хотите посмотреть оценки.\n",
                Result = new InlineKeyboardMarkup(buttonRows)
            };
        }
        private void AddSortButtons(List<InlineKeyboardButton[]> buttonRows, string attestationType, string semester)
        {
            switch (attestationType)
            {
                case "экз":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&type=защ"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&type=зач"),
                    });
                    break;
                case "защ":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Экзамен", $"/km?sem={semester}&type=экз"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&type=зач"),
                    });
                    break;
                case "зач":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&type=защ"),
                        InlineKeyboardButton.WithCallbackData("Экзамен >>>", $"/km?sem={semester}&type=экз"),
                    });
                    break;

            }
        }
        private void AddSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester") });
        }
    }
}
