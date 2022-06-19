using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;

namespace TelegramBotBARS.Commands
{
    public class MissCommand : ExcelDataCommand
    {
        public override ExecuteResult Execute(string options)
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

            var missedLessonRecords =
                _dataProvider.GetStatements()
                .Where(s => s.Semester.Contains(semester))
                .SelectMany(s => s.MissedLessonRecords)
                .OrderByDescending(record => record.LessonDate);

            int pageCount = (int)Math.Ceiling(missedLessonRecords.Count() / 5.0);

            StringBuilder message = new($"<b>{GetSemesterFullName(semester)}</b>\n");

            message
                .AppendLine("------------------------------------------")
                .AppendLine(RecordsToString(missedLessonRecords
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
        private string RecordsToString(IEnumerable<MissedLessonRecord> records)
        {
            StringBuilder recordsStr = new();

            foreach (var rec in records)
            {
                recordsStr
                    .AppendLine($"{rec.LessonType}, {rec.Statement.Discipline}")
                    .AppendLine($"{rec.LessonDate.ToString("dd.MM.yy")}, {rec.LessonTime}");
                
                if (rec.Reason == "По уважительной причине")
                {
                    recordsStr.AppendLine($"<b>{rec.Reason}</b>");
                }

                recordsStr.AppendLine("------------------------------------------");
            }

            return recordsStr.ToString();
        }
        private string GetSemesterFullName(string semester)
        {
            return semester.Last() == 'В'
                ? semester + "есенний семестр"
                : semester + "сенний семестр";
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
