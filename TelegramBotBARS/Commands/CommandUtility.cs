namespace TelegramBotBARS.Commands
{
    public static class CommandUtility
    {
        public static string GetDefaultSemester()
        {
            DateTime currDate = DateTime.Now;

            if (currDate.Month > 8 || currDate.Month < 2)
            {
                return $"{currDate.Year}/{currDate.Year + 1}, О";
            }
            else
            {
                return $"{currDate.Year - 1}/{currDate.Year}, В";
            }
        }
        public static string GetSemesterFullName(string semester)
        {
            return semester.Last() == 'В'
                ? semester + "есенний семестр"
                : semester + "сенний семестр";
        }
    }
}
