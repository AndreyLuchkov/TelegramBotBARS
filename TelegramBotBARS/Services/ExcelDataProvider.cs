using TelegramBotBARS.Entities;
using TelegramBotBARS.Parsers;

namespace TelegramBotBARS.Services
{
    public class ExcelDataProvider
    {
        private IList<Statement>? _cachedStatements;

        public IList<Statement> GetStatements()
        {
            if (_cachedStatements != null)
            {
                return _cachedStatements;
            }

            using ExcelParser excelParser = new();

            var statements = excelParser.ParseStatements(new Student
            {
                Id = new Guid("9e25c0af-6c14-eb11-80d1-005056be401c"),
                Login = "LuchkovAD"
            });

            var controlEvents = excelParser.ParseControlEvents();

            foreach (var s in statements)
            {
                s.ControlEvents.AddRange(
                    controlEvents
                    .Where(ce => ce.StatementId == s.Id)
                    .ToList());
            }

            var missedLessonRecords = excelParser.ParseMissedLessonRecords();

            foreach (var s in statements)
            {
                var records = missedLessonRecords
                    .Where(ce => ce.StatementId == s.Id);

                foreach (var record in records)
                {
                    record.Statement = s;
                }

                s.MissedLessonRecords.AddRange(records.ToList());
            }

            _cachedStatements = statements;

            return statements;
        }
    }
}
