using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;

using static TelegramBotBARS.Commands.CommandUtility;

namespace TelegramBotBARS.Commands
{
    public class MissCommand : WebApiDataCommand
    {
        public override async Task<ExecuteResult> ExecuteAsync(string options)
        {
            var optionsParams = options.Split('&');
            string semester = optionsParams
                .Where(param => param.Contains("sem"))
                .FirstOrDefault(GetDefaultSemester())
                .Split('=')
                .Last();
            int page = int.Parse(
                optionsParams
                .Where(param => param.Contains("page"))
                .FirstOrDefault("1")
                .Split('=')
                .Last());

            var mlRecords = await _dataProvider.GetMLRecords(semester);

            int pageCount = (int)Math.Ceiling(mlRecords.Count() / 5.0);

            StringBuilder message = new($"<b>{GetSemesterFullName(semester)}</b>\n");

            message
                .AppendLine("------------------------------------------")
                .AppendLine(RecordsToString(mlRecords
                                                .OrderByDescending(record => record.LessonDate)
                                                .Skip((page - 1) * 5)
                                                .Take(5)));

            var buttonRows = new List<InlineKeyboardButton[]>();
            AddPageSwitchButtons(buttonRows, page, semester, pageCount);
            AddSemesterChangeButton(buttonRows);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message.ToString(),
                Result = new InlineKeyboardMarkup(buttonRows)
            };
        }
        private string RecordsToString(IEnumerable<MissedLessonRecord> records)
        {
            StringBuilder recordsStr = new();

            foreach (var rec in records)
            {
                recordsStr
                    .AppendLine($"{rec.LessonType}, {rec.Discipline}")
                    .AppendLine($"{rec.LessonDate.ToString("dd.MM.yy")}, {rec.LessonTime}");
                
                if (rec.Reason)
                {
                    recordsStr.AppendLine($"<b>По уважительной причине</b>");
                } 

                recordsStr.AppendLine("------------------------------------------");
            }

            return recordsStr.ToString();
        }
        private void AddPageSwitchButtons(List<InlineKeyboardButton[]> buttonRows, int page, string semester, int pageCount)
        {
            if (page == 1)
            {
                buttonRows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData("2 >>>", $"/miss?page=2&sem={semester}")
                });
            } 
            else if (page == pageCount - 1)
            {
                buttonRows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"<<< {page - 1}", $"/miss?page={page - 1}&sem={semester}")
                });
            }
            else
            {
                buttonRows.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData($"<<< {page - 1}", $"/miss?page={page - 1}&sem={semester}"),
                    InlineKeyboardButton.WithCallbackData($"{page + 1} >>>", $"/miss?page={page + 1}&sem={semester}")
                });
            }
        }
        private void AddSemesterChangeButton(List<InlineKeyboardButton[]> buttonRows)
        {
            buttonRows.Add(new[] { InlineKeyboardButton.WithCallbackData("Выбрать семестр", ";semester?from=/miss") });
        }
    }
}
