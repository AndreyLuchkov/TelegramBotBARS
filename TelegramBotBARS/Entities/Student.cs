namespace TelegramBotBARS.Entities
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Login { get; set; }

        public List<Statement> Statements { get; set; } = new();
    }
}
