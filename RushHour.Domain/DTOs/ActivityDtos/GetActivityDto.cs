using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Domain.DTOs.ActivityDtos
{
    public class GetActivityDto : BaseDto
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Duration { get; set; }

        public Guid ProviderId { get; set; }

        public List<Guid> EmployeeIds { get; set; }
    }
}
