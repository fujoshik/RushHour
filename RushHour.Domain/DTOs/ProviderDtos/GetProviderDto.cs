namespace RushHour.Domain.DTOs.ProviderDtos
{
    public class GetProviderDto : BaseDto
    {
        public string Name { get; set; }

        public string Website { get; set; }

        public string BusinessDomain { get; set; }

        public string Phone { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string WorkingDays { get; set; }
    }
}
