using System.Text;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotBARS.Entities;
using TelegramBotBARS.Services;

namespace TelegramBotBARS.Commands
{
    public class MissCommand : IServiceRequiredCommand
    {
        private ExcelDataProvider _dataProvider = null!;

        public IEnumerable<Type> RequiredServicesTypes { get; }

        public MissCommand()
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
            int page = int.Parse(
                optionsParams
                .Where(param => param.Contains("page"))
                .FirstOrDefault("1")
                .Split('=')
                .Last());

            var missedLessonRecords =
                _dataProvider.GetStatements()
                .Where(s => s.Semester == semester)
                .SelectMany(s => s.MissedLessonRecords)
                .OrderByDescending(record => record.LessonDate);

            int pageCount = (int)Math.Ceiling(missedLessonRecords.Count() / 5.0);

            string message =
                $"Семестр: <b>{semester}</b>\n" 
                + "------------------------------------------\n"
                + RecordsToString(
                    missedLessonRecords
                    .Skip((page - 1) * 5)
                    .Take(5));

            var buttonRows = new List<InlineKeyboardButton[]>();
            AddPageSwitchButtons(buttonRows, page, semester, pageCount);
            AddSemesterChangeButton(buttonRows);

            return new ExecuteResult
            {
                ResultType = ResultType.InlineKeyboardWithCallback,
                Message = message,
                Result = new InlineKeyboardMarkup(buttonRows)
            };
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
