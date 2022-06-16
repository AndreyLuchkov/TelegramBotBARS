using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class StatementCommand : IServiceRequiredCommand
    {
        private ExcelDataProvider _dataProvider = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public StatementCommand()
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
            var statement = _dataProvider.GetStatements()
                .Where(s => s.Id == new Guid(options))
                .First();

            string teacher = $"{statement.Teacher.Split(' ').First()}  {String.Join("", statement.Teacher.Split(' ').Skip(1).Select(s => $"{s[0]}."))}";

            string message = $"<b>{statement.Discipline}</b>\n({teacher}, {statement.IAType})\n"
                + "------------------------------------------\n"
                + GetControlEventTable(statement);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message,
                Result = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("<<< Назад", $"/km?sem={statement.Semester}&iatype={String.Join("", statement.IAType.Take(3))}"))
            };
        }
        private string GetControlEventTable(Statement statement)
        {
            StringBuilder table = new StringBuilder();

            foreach (var ce in statement.ControlEvents.OrderBy(ce => ce.Number))
            {
                table.AppendLine($"{ce.Number}. {ce.Name}")
                     .AppendLine($"Неделя проведения: {ce.WeekNumber}")
                     .AppendLine($"<b>Оценка: {ce.Score}</b>")
                     .AppendLine("------------------------------------------");
            }

            return table.ToString();
        }
    }
}
