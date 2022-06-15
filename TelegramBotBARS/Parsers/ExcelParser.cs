using System.Collections.Concurrent;
using TelegramBotBARS.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace TelegramBotBARS.Parsers
{
    public class ExcelParser : IDisposable
    {
        private const string _excelFilePath = "excel.xlsx";
        private readonly SpreadsheetDocument _document;

        public ExcelParser()
        {
            _document = SpreadsheetDocument.Open(_excelFilePath, false);
        }

        public IList<Statement> ParseStatements(Student student)
        {
            var workboolPart = _document.WorkbookPart!;
            var sheet = workboolPart.Workbook.Descendants<Sheet>()
                .Where(sheet => sheet.Name == "Успеваемость")
                .First();

            var sheetPart = (WorksheetPart)workboolPart.GetPartById(sheet.Id!);

            var sheetData = sheetPart.Worksheet.Elements<SheetData>().First();

            var stringTable =
                _document.WorkbookPart!
                .GetPartsOfType<SharedStringTablePart>()
                .FirstOrDefault();

            ConcurrentBag<Statement> statements = new();
            foreach (Row row in sheetData.Elements<Row>().Skip(1))
            {
                Parallel.ForEach(row.Elements<Cell>(), cell =>
                {
                    if (cell != null)
                    {
                        var value = cell.CellValue?.InnerText!;
                        string cellIndex = cell.CellReference?.Value!;
                        
                        if (cell.DataType != null)
                        {
                            switch (cell.DataType.Value)
                            {
                                case CellValues.SharedString:
                                    if (stringTable != null)
                                    {
                                        value = GetCellStringValue(stringTable, value);
                                        if (value == "NULL")
                                        {
                                            value = null;
                                        }
                                    }
                                    break;
                            }
                        }

                        Statement statement = new();
                        switch (cellIndex[0])
                        {
                            case 'A':
                                statement.Semester = value!;
                                break;
                            case 'B':
                                statement.Discipline = value!;
                                break;
                            case 'C':
                                statement.Teacher = value!;
                                break;
                            case 'E':
                                statement.Id = new Guid(value!);
                                break;
                            case 'J':
                                break;
                            case 'L':
                                if (value is null)
                                {
                                    statement.IAScore = null;
                                }
                                else
                                {
                                    int IAScore;
                                    int.TryParse(value, out IAScore);
                                    statement.IAScore = IAScore;
                                }
                                break;
                            case 'M':
                                if (value is null)
                                {
                                    statement.TotalScore = null;
                                } 
                                else
                                {
                                    int totalScore;
                                    int.TryParse(value, out totalScore);
                                    statement.TotalScore = totalScore;
                                }
                                break;
                            case 'N':
                                statement.IAType = value!;
                                break;

                        }
                        statements.Add(statement);
                    }
                });
            }
            return statements.ToList();
        }
        private string GetCellStringValue(SharedStringTablePart stringTable, string value) 
            => stringTable.SharedStringTable
                .ElementAt(int.Parse(value)).InnerText;
        public IList<ControlEvent> ParseControlEvents(IList<Statement> statements)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            _document.Dispose();
        }
    }
}
