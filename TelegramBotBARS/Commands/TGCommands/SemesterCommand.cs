using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class SemesterCommand : IServiceRequiredCommand
    {
        private ExcelDataProvider _dataProvider = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public SemesterCommand()
        {
            RequiredServicesTypes = new List<Type>
            {
                typeof(ExcelDataProvider),
            };
        }

        public void AddService(object service)
        {
            if (service is ExcelDataProvider dataProvider)
            {
                _dataProvider = dataProvider;
            }
        }
        public ExecuteResult Execute(string options)
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
                .OrderByDescending(sem => sem);
                

            foreach (var semester in semesters)
            {
                buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData(semester, $"{from}?sem={semester}") });
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
