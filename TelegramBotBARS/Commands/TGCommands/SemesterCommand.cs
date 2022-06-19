using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class SemesterCommand : ExcelDataCommand
    {
        public override ExecuteResult Execute(string options)
        {
            var buttonRows = new List<InlineKeyboardButton[]>();

            var optionsParams = options.Split('&');
            string from = optionsParams
                .Where(param => param.Contains("from"))
                .FirstOrDefault("/km")
                .Split('=')
                .Last();

            var semesters = _dataProvider.GetStatements()
                .Select(s => s.Semester)
                .Distinct()
                .GroupBy(sem 
                    => sem.Split(',').First())
                .OrderByDescending(group => group.Key)
                .SelectMany(group 
                    => group.OrderBy(value => value));
                

            foreach (var semester in semesters)
            {
                buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData(semester, $"{from}?sem={semester.Substring(0, 12)}") });
            }

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = "Выберите семестр.",
                Result = new InlineKeyboardMarkup(buttonRows)
            };
        }
    }
}
