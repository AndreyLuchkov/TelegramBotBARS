using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;

using static TelegramBotBARS.Commands.CommandUtility;

namespace TelegramBotBARS.Commands
{
    public class ResultsCommand : WebApiDataCommand
    {
        public override async Task<ExecuteResult> Execute(string options)
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

            var statements = _dataProvider.GetStatements(semester, attestationType);

            var buttonRows = new List<InlineKeyboardButton[]>();
            AddSortButtons(buttonRows, attestationType, semester);
            AddSemesterChangeButton(buttonRows);

            StringBuilder message = new($"Семестр: <b>{GetSemesterFullName(semester)}</b>\n");

            message
                .AppendLine("------------------------------------------")
                .AppendLine(StatementsToString(await statements));

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message.ToString(),
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
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/results?sem={semester}&type=защ"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/results?sem={semester}&type=зач"),
                    });
                    break;
                case "защ":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Экзамен", $"/results?sem={semester}&type=экз"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/results?sem={semester}&type=зач"),
                    });
                    break;
                case "зач":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/results?sem={semester}&type=защ"),
                        InlineKeyboardButton.WithCallbackData("Экзамен >>>", $"/results?sem={semester}&type=экз"),
                    });
                    break;
            }
        }
        private void AddSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester?from=/results") });
        }
        private string StatementsToString(IEnumerable<Statement> statements)
        {
            StringBuilder statementsStr = new();

            foreach (var s in statements)
            {
                statementsStr
                    .AppendLine($"<b>{s.Discipline}</b>")
                    .AppendLine($"<i>Промежуточная аттестация: {s.AttestationScore}</i>")
                    .AppendLine("------------------------------------------");
            }

            return statementsStr.ToString();
        }
    }
}
