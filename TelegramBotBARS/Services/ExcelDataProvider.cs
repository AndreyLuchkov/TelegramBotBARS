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

            excelParser.ParseControlEvents(statements);

            _cachedStatements = statements;

            return statements;
        }
    }
}
