using FluentValidation;
using RushHour.Domain.DTOs.EmployeeDtos;

namespace RushHour.Domain.Validators.Employee
{
    public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(2, 50)
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("{PropertyName} must be alphanumeric");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("{PropertyName} is not valid");

            RuleFor(x => x.RatePerHour)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
