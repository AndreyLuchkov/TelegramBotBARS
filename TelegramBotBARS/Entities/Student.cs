namespace TelegramBotBARS.Entities
{
    public partial class Student
    {
        public Student()
        {
            MissedLessonRecords = new HashSet<MissedLessonRecord>();
            Statements = new HashSet<Statement>();
        }

        public Guid Id { get; set; }
        public string Login { get; set; } = null!;

        public virtual ICollection<MissedLessonRecord> MissedLessonRecords { get; set; }
        public virtual ICollection<Statement> Statements { get; set; }
    }
}
