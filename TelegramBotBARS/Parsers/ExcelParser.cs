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
        public IList<ControlEvent> ParseControlEvents()
        {
            var controlEventSheet =
                _package.Workbook.Worksheets
                .Where(sheet => sheet.Name == "КМ по всем ведомостям и оценки")
                .First();

            ConcurrentBag<ControlEvent> controlEvents = new();
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

                controlEvents.Add(controlEvent);
            });

            return controlEvents.ToList();
        }
        public IList<MissedLessonRecord> ParseMissedLessonRecords()
        {
            var sheet =
                 _package.Workbook.Worksheets
                .Where(sheet => sheet.Name == "Посещаемость")
                .First();

            ConcurrentBag<MissedLessonRecord> records = new();
            Parallel.For(sheet.Dimension.Start.Row + 1, sheet.Dimension.End.Row + 1, i =>
            {
                MissedLessonRecord record = new();

                string guid = sheet.Cells[$"B{i}"].Value.ToString()!;
                if (guid != "NULL")
                {
                    record.StatementId = new Guid(guid);
                }
                
                record.LessonType = sheet.Cells[$"E{i}"].Value.ToString()!;
                record.LessonDate = DateTime.Parse(sheet.Cells[$"F{i}"].Value.ToString()!);
                record.LessonTime = sheet.Cells[$"G{i}"].Value.ToString()!;
                record.Reason = sheet.Cells[$"H{i}"].Value.ToString()!;

                records.Add(record);
            });

            return records.ToList();
        }
        public void Dispose()
        {
            _package.Dispose();
        }
    }
}
