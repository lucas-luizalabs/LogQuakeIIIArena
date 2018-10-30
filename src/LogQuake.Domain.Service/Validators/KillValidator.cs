using FluentValidation;
using LogQuake.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogQuake.Service.Validators
{
    public class KillValidator : AbstractValidator<Kill>
    {
        public KillValidator()
        {
            RuleFor(c => c)
                .NotNull()
                .OnAnyFailure(x =>
                {
                    throw new ArgumentNullException("Objeto não encontrado.");
                });

            RuleFor(c => c.PlayerKiller)
                .NotEmpty().WithMessage("Obrigatório informar o PlayerKiller.")
                .NotNull().WithMessage("Obrigatório informar o PlayerKiller.");

            RuleFor(c => c.IdGame)
                .NotEmpty().WithMessage("Obrigatório informar o IdGame.")
                .NotNull().WithMessage("Obrigatório informar o IdGame.");

            RuleFor(c => c.PlayerKilled)
                .NotEmpty().WithMessage("Obrigatório informar o PlayerKilled.")
                .NotNull().WithMessage("Obrigatório informar o PlayerKilled.");
        }
    }
}
