using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;

namespace TelegramBotBARS.Commands
{
    public class StatementCommand : WebApiDataCommand
    {
        public override async Task<ExecuteResult> ExecuteAsync(string options)
        {
            var statement = await _dataProvider.GetStatement(new Guid(options));

            string teacher = $"{statement.Teacher.Split(' ').First()}  {String.Join("", statement.Teacher.Split(' ').Skip(1).Select(s => $"{s[0]}."))}";

            StringBuilder message = new($"<b>{statement.Discipline}</b>\n({teacher}, {statement.AttestationType})\n");
           
            message
                .AppendLine("------------------------------------------")
                .AppendLine(await ControlEventsToString(statement));

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message.ToString(),
                Result = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("<<< Назад", $"/km?sem={statement.Semester.Substring(0, 12)}&type={statement.AttestationType.Substring(0, 3)}"))
            };
        }
        private async Task<string> ControlEventsToString(Statement statement)
        {
            StringBuilder controlEventsStr = new StringBuilder();

            var controlEvents = await _dataProvider.GetControlEvents(statement.Id);

            var oldScoreEvents =
                controlEvents
                .Where(ce => ce.ScoreStatus == "пересдана из-за низкого результата")
                .GroupBy(ce => ce.Name)
                .Select(group
                    => group
                        .OrderByDescending(ce => ce.ScoreAddDate)
                        .First());

            foreach (var ce in controlEvents
                                .OrderBy(ce => ce.Number)
                                .Where(ce => ce.ScoreStatus == "учитывается в итоговом балле"))
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
