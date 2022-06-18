namespace TelegramBotBARS.Entities
{
    public class Statement
    {
        public Guid Id { get; set; }
        public string Discipline { get; set; }
        public string Semester { get; set; }
        public string Teacher { get; set; }
        public float? SemesterScore { get; set; }
        public int? IAScore { get; set; }
        public int? TotalScore { get; set; }
        public string IAType { get; set; }
        
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public List<ControlEvent> ControlEvents { get; set; } = new();
        public List<MissedLessonRecord> MissedLessonRecords { get; set; } = new();
    }
}
