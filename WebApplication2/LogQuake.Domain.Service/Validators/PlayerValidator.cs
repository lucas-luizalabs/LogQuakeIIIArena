using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using LogQuake.Domain.Entities;

namespace LogQuake.Service.Validators
{
    public class PlayerValidator : AbstractValidator<Player>
    {
        public PlayerValidator()
        {
            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException("Can't found the object.");
                });

            RuleFor(c => c.PlayerName)
                .NotEmpty().WithMessage("Is necessary to inform the CPF.")
                .NotNull().WithMessage("Is necessary to inform the CPF.");

            RuleFor(c => c.Sobrenome)
                .NotEmpty().WithMessage("Is necessary to inform the birth date.")
                .NotNull().WithMessage("Is necessary to inform the birth date.");

            //RuleFor(c => c.Name)
            //    .NotEmpty().WithMessage("Is necessary to inform the name.")
            //    .NotNull().WithMessage("Is necessary to inform the birth date.");
        }

    }
}
