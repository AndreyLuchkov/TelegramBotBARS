namespace TelegramBotBARS.Entities
{
    public partial class MissedLessonRecord
    {
        public string LessonType { get; set; } = null!;
        public DateOnly LessonDate { get; set; }
        public string LessonTime { get; set; } = null!;
        public bool Reason { get; set; }
        public string Discipline { get; set; } = null!;
        public Guid StudentId { get; set; }
        public Guid StatementId { get; set; }

        public virtual Statement Statement { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}
