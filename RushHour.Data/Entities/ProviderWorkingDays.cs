namespace RushHour.Data.Entities
{
    public class ProviderWorkingDays
    {
        public Guid ProviderId { get; set; }
        public DayOfWeek DayOfTheWeek { get; set; }
    }
}
