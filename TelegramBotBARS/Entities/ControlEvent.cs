namespace TelegramBotBARS.Entities
{
    public partial class ControlEvent
    {
        public string Name { get; set; } = null!;
        public int Number { get; set; }
        public int WeekNumber { get; set; }
        public int Weight { get; set; }
        public Guid StatementId { get; set; }
        public int Score { get; set; }
        public string? ScoreStatus { get; set; }
        public DateTime? ScoreAddDate { get; set; }

        public virtual Statement Statement { get; set; } = null!;
    }
}
