using FluentValidation;
using RushHour.Domain.DTOs.ActivityDtos;

namespace RushHour.Domain.Validators.Activity
{
    public class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
    {
        public CreateActivityDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 50);

            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Duration)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
