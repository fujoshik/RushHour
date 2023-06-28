using FluentValidation;
using RushHour.Domain.Calculations;
using RushHour.Domain.DTOs.AccountDtos;

namespace RushHour.Domain.Validators.Account
{
    public class UpdateAccountWithoutRoleValidator : AbstractValidator<UpdateAccountWithoutRole>
    {
        public UpdateAccountWithoutRoleValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.FullName)
                .NotEmpty()
                .Length(3, 50)
                .Must(CheckName.IsValidFullName)
                .WithMessage("{PropertyName} must contain only letters, hyphens, and apostrophes");
        }
    }
}
