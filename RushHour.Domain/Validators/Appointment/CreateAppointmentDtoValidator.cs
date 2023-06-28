using FluentValidation;
using RushHour.Domain.DTOs.AppointmentDtos;

namespace RushHour.Domain.Validators.Appointment
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
}
