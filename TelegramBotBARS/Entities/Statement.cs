namespace TelegramBotBARS.Entities
{
    public partial class Statement
    {
        public Statement()
        {
            ControlEvents = new HashSet<ControlEvent>();
            MissedLessonRecords = new HashSet<MissedLessonRecord>();
        }

        public Guid Id { get; set; }
        public string Discipline { get; set; } = null!;
        public string AttestationType { get; set; } = null!;
        public string Teacher { get; set; } = null!;
        public decimal? SemesterScore { get; set; }
        public int? AttestationScore { get; set; }
        public int? ResultScore { get; set; }
        public Guid? StudentId { get; set; }
        public string Semester { get; set; } = null!;

        public virtual Student? Student { get; set; }
        public virtual ICollection<ControlEvent> ControlEvents { get; set; }
        public virtual ICollection<MissedLessonRecord> MissedLessonRecords { get; set; }
    }
}
