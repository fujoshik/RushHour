using System.ComponentModel.DataAnnotations;

namespace RushHour.Domain.DTOs.ActivityDtos
{
    public class CreateActivityDto
    {
        public string Name { get; set; }

        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Only a positive number allowed")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only a positive number allowed")]
        public int Duration { get; set; }

        public Guid ProviderId { get; set; }

        public List<Guid> EmployeeIds { get; set; }
    }
}
