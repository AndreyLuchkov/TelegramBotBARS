namespace TelegramBotBARS.Entities
{
    public class MissedLessonRecord
    {
        public Guid StatementId { get; set; }
        public Statement Statement { get; set; }

        public string LessonType { get; set; }
        public DateTime LessonDate { get; set; }
        public string LessonTime { get; set; }
        public string Reason { get; set; }
    }
}
