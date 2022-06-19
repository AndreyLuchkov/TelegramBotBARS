using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;

namespace TelegramBotBARS.Commands
{
    public class ResultsCommand : ExcelDataCommand
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

            var buttonRows = new List<InlineKeyboardButton[]>();
            AddSortButtons(buttonRows, IAType, semester);
            AddSemesterChangeButton(buttonRows);

            StringBuilder message = new($"Семестр: <b>{GetSemesterFullName(semester)}</b>\n");

            message
                .AppendLine("------------------------------------------")
                .AppendLine(StatementsToString(statements));

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message.ToString(),
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
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/results?sem={semester}&iaT=защ"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/results?sem={semester}&iaT=зач"),
                    });
                    break;
                case "защ":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Экзамен", $"/results?sem={semester}&iaT=экз"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/results?sem={semester}&iaT=зач"),
                    });
                    break;
                case "зач":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/results?sem={semester}&iaT=защ"),
                        InlineKeyboardButton.WithCallbackData("Экзамен >>>", $"/results?sem={semester}&iaT=экз"),
                    });
                    break;
            }
        }
        private void AddSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester?from=/results") });
        }
        private string GetSemesterFullName(string semester)
        {
            return semester.Last() == 'В'
                ? semester + "есенний семестр"
                : semester + "сенний семестр";
        }
        private string StatementsToString(IEnumerable<Statement> statements)
        {
            StringBuilder statementsStr = new();

            foreach (var s in statements)
            {
                statementsStr
                    .AppendLine($"<b>{s.Discipline}</b>")
                    .AppendLine($"<i>Промежуточная аттестация: {s.IAScore}</i>")
                    .AppendLine("------------------------------------------");
            }

            return statementsStr.ToString();
        }
    }
}
