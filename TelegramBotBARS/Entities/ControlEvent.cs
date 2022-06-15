namespace TelegramBotBARS.Entities
{
    public enum ScoreStatus
    {
        Empty,
        Ok,
        Bad,
        Retake,
    }
    public class ControlEvent
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public int WeekNumber { get; set; }
        public int Weight { get; set; }
        public int? Score { get; set; }
        public ScoreStatus ScoreStatus { get; set; }

        public Guid StatementId { get; set; }
        public Statement Statement { get; set; }
    }
}
