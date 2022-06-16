using OfficeOpenXml;
using TelegramBotBARS.Entities;
using System.Collections.Concurrent;

namespace TelegramBotBARS.Parsers
{
    public class ExcelParser : IDisposable
    {
        private const string _excelFilePath = "excel.xlsx";
        private readonly ExcelPackage _package;

        public ExcelParser()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _package = new ExcelPackage(new FileInfo(_excelFilePath));
        }

        public IList<Statement> ParseStatements(Student student)
        {
            var statementSheet =
                _package.Workbook.Worksheets
                .Where(sheet => sheet.Name == "Успеваемость")
                .First();

            ConcurrentBag<Statement> statements = new();
            Parallel.For(statementSheet.Dimension.Start.Row + 1, statementSheet.Dimension.End.Row + 1, i =>
            {
                Statement statement = new();

                statement.Student = student;
                statement.StudentId = student.Id;

                statement.Semester = statementSheet.Cells[$"A{i}"].Value.ToString()!;
                statement.Discipline = statementSheet.Cells[$"B{i}"].Value.ToString()!;
                statement.Teacher = statementSheet.Cells[$"C{i}"].Value.ToString()!;
                statement.Id = new Guid(statementSheet.Cells[$"E{i}"].Value.ToString()!);

                float semesterScore;
                if (float.TryParse(statementSheet.Cells[$"K{i}"].Value.ToString()!, out semesterScore))
                {
                    statement.SemesterScore = semesterScore;
                } 
                else
                {
                    statement.SemesterScore = null;
                }

                int IAScore;
                if (int.TryParse(statementSheet.Cells[$"L{i}"].Value.ToString()!, out IAScore))
                {
                    statement.IAScore = IAScore;
                }
                else
                {
                    statement.IAScore = null;
                }

                int totalScore;
                if (int.TryParse(statementSheet.Cells[$"M{i}"].Value.ToString()!, out totalScore))
                {
                    statement.TotalScore = totalScore;
                }
                else
                {
                    statement.TotalScore = null;
                }
                
                statement.IAType = statementSheet.Cells[$"N{i}"].Value.ToString()!;

                statements.Add(statement);
            });

            return statements.ToList();
        }
        public void ParseControlEvents(IList<Statement> statements)
        {
            var controlEventSheet =
                _package.Workbook.Worksheets
                .Where(sheet => sheet.Name == "КМ по всем ведомостям и оценки")
                .First();

            ConcurrentDictionary<Guid, ConcurrentBag<ControlEvent>> controlEvents = new();

            foreach (var statement in statements)
            {
                controlEvents.TryAdd(statement.Id, new());
            }

            Parallel.For(controlEventSheet.Dimension.Start.Row + 1, controlEventSheet.Dimension.End.Row + 1, i =>
            {
                ControlEvent controlEvent = new();

                controlEvent.StatementId = new Guid(controlEventSheet.Cells[$"A{i}"].Value.ToString()!);
                controlEvent.WeekNumber = int.Parse(controlEventSheet.Cells[$"C{i}"].Value.ToString()!);
                controlEvent.Number = int.Parse(controlEventSheet.Cells[$"E{i}"].Value.ToString()!);
                controlEvent.Name = controlEventSheet.Cells[$"F{i}"].Value.ToString()!;
                controlEvent.Weight = int.Parse(controlEventSheet.Cells[$"G{i}"].Value.ToString()!);
                
                int score;
                if (int.TryParse(controlEventSheet.Cells[$"H{i}"].Value.ToString()!, out score))
                {
                    controlEvent.Score = score;
                }
                else
                {
                    controlEvent.Score = null;
                }

                controlEvent.ScoreStatus = controlEventSheet.Cells[$"J{i}"].Value.ToString()! switch
                {
                    "учитывается в итоговом балле" => ScoreStatus.Ok,
                    "пересдана из-за низкого результата" => ScoreStatus.Retake,
                    _ => ScoreStatus.Bad
                };

                controlEvents[controlEvent.StatementId].Add(controlEvent);
            });

            foreach (var statement in statements)
            {
                statement.ControlEvents.AddRange(controlEvents[statement.Id].ToList());
            }
        }
        public void Dispose()
        {
            _package.Dispose();
        }
    }
}
