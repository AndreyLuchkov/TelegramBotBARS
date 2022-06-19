using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class StatementCommand : ExcelDataCommand
    {
        public override ExecuteResult Execute(string options)
        {
            var statement = 
                _dataProvider.GetStatements()
                .Where(s => s.Id == new Guid(options))
                .First();

            string teacher = $"{statement.Teacher.Split(' ').First()}  {String.Join("", statement.Teacher.Split(' ').Skip(1).Select(s => $"{s[0]}."))}";

            StringBuilder message = new($"<b>{statement.Discipline}</b>\n({teacher}, {statement.IAType})\n");
           
            message
                .AppendLine("------------------------------------------")
                .AppendLine(ControlEventsToString(statement));

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message.ToString(),
                Result = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("<<< Назад", $"/km?sem={statement.Semester.Substring(0, 12)}&iaT={String.Join("", statement.IAType.Take(3))}"))
            };
        }
        private string ControlEventsToString(Statement statement)
        {
            StringBuilder controlEventsStr = new StringBuilder();

            var oldScoreEvents =
                statement.ControlEvents
                .Where(ce => ce.ScoreStatus == ScoreStatus.Retake)
                .GroupBy(ce => ce.Name)
                .Select(group
                    => group
                        .OrderByDescending(ce => ce.RateDate)
                        .First());

            foreach (var ce in statement.ControlEvents
                                .OrderBy(ce => ce.Number)
                                .Where(ce => ce.ScoreStatus == ScoreStatus.Ok))
            {
                controlEventsStr
                    .AppendLine($"{ce.Number}. {ce.Name}")
                    .AppendLine($"Неделя проведения: {ce.WeekNumber}");

                ControlEvent? oldScoreEvent = oldScoreEvents
                    .Where(e => e.Name == ce.Name)
                    .FirstOrDefault();
                if (oldScoreEvent != null)
                {
                    controlEventsStr.AppendLine($"<b>Оценка: {ce.Score}</b> <i>(Пересдана: {oldScoreEvent.Score})</i>");
                }
                else
                {
                    controlEventsStr.AppendLine($"<b>Оценка: {ce.Score}</b>");
                }

                controlEventsStr.AppendLine("------------------------------------------");
            }

            return controlEventsStr.ToString();
        }
    }
}
