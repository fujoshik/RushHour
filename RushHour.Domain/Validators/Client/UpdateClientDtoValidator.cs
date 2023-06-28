﻿using FluentValidation;
using RushHour.Domain.DTOs.ClientDtos;

namespace RushHour.Domain.Validators.Client
{
    public class UpdateClientDtoValidator : AbstractValidator<UpdateClientDto>
    {
        public UpdateClientDtoValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^[0 - 9] +$").WithMessage("{PropertyName} is not valid");

            RuleFor(x => x.Address)
                .NotEmpty()
                .Length(3, 50);
        }
    }
}
