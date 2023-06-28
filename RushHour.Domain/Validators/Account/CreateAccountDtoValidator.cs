using FluentValidation;
using RushHour.Domain.Calculations;
using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.Validators.Account
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress().WithMessage("{PropertyName} is not valid");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(3, 50)
                .Must(CheckName.IsValidFullName)
                .WithMessage("{PropertyName} must contain only letters, hyphens, and apostrophes");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
                .WithMessage("{PropertyName} must contain at least 8 characters, one uppercase, one lowercase, one digit and a special symbol");
        }
    }
}
