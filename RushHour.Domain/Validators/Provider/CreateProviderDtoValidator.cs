using FluentValidation;
using RushHour.Domain.DTOs.ProviderDtos;

namespace RushHour.Domain.Validators.Provider
{
    public class CreateProviderDtoValidator : AbstractValidator<CreateProviderDto>
    {
        public CreateProviderDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 50);

            RuleFor(x => x.Website).NotEmpty();

            RuleFor(x => x.BusinessDomain)
                .NotEmpty()
                .Matches(@"^[a-zA-Z]+[a-zA-Z0-9""'\s-]*$").WithMessage("Letters and numbers only");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("{PropertyName} is not valid");

            RuleFor(x => x.StartTime).NotEmpty();

            RuleFor(x => x.EndTime).NotEmpty();

            RuleFor(x => x.WorkingDays).NotEmpty();
        }
    }
}
