using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Login.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
            {
                RuleFor(d => d.Username)
                    .NotNull().WithMessage("Username is required")

                    .NotEmpty().WithMessage("Username is required");

                RuleFor(d => d.Password)
                    .NotNull().WithMessage("Password is required")

                    .NotEmpty().WithMessage("Password is required");
            }
        

    }
}
