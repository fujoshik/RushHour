using FluentValidation;
using RushHour.Domain.DTOs.ClientDtos;

namespace RushHour.Domain.Validators.Client
{
    public class CreateClientDtoValidator : AbstractValidator<CreateClientDto>
    {
        public CreateClientDtoValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$").WithMessage("{PropertyName} is not valid");

            RuleFor(x => x.Address)
                .NotEmpty()
                .Length(3, 50);           
        }
    }
}
