using System.Dynamic;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class KmCommand : IServiceRequiredCommand
    {
        private ExcelDataProvider _dataProvider = null!;
        
        public IEnumerable<Type> RequiredServicesTypes { get; }

        public KmCommand()
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
            var optionsParams = options.Split('&');
            string semester = optionsParams
                .Where(param => param.Contains("sem"))
                .FirstOrDefault(GetDefaultSemester())
                .Split('=')
                .Last();
            string IAType = optionsParams
                .Where(param => param.Contains("iatype"))
                .FirstOrDefault("зач")
                .Split('=')
                .Last();

            var statements = _dataProvider.GetStatements()
                .Where(s => s.Semester == semester)
                .Where(s => s.IAType.Contains(IAType));

            var buttonRows = new List<InlineKeyboardButton[]>(statements.Count());
            foreach (var statement in statements)
            {
                buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData($"{statement.Discipline}", $";statement?{statement.Id}") });
            }

            SetSortButtons(buttonRows, IAType, semester);
            SetSemesterChangeButton(buttonRows);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = $"Семестр: <b>{semester}</b>\nВыберите предмет, по которому хотите посмотреть оценки.",
                Result = new InlineKeyboardMarkup(buttonRows)
            };
        }
        private void SetSortButtons(List<InlineKeyboardButton[]> buttonRows, string IAType, string semester)
        {
            switch (IAType)
            {
                case "экз":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&iatype=защ"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&iatype=зач"),
                    });
                    break;
                case "защ":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Экзамен", $"/km?sem={semester}&iatype=экз"),
                        InlineKeyboardButton.WithCallbackData("Зачёт >>>", $"/km?sem={semester}&iatype=зач"),
                    });
                    break;
                case "зач":
                    buttonRows.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("<<< Защита", $"/km?sem={semester}&iatype=защ"),
                        InlineKeyboardButton.WithCallbackData("Экзамен >>>", $"/km?sem={semester}&iatype=экз"),
                    });
                    break;

            }
        }
        private void SetSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester") });
        }
        private string GetDefaultSemester()
        {
            DateTime currDate = DateTime.Now;

            if (currDate.Month > 8 || currDate.Month < 2)
            {
                return $"{currDate.Year}/{currDate.Year + 1}, Осенний семестр";
            }
            else
            {
                return $"{currDate.Year - 1}/{currDate.Year}, Весенний семестр";
            }
        }
    }
}
