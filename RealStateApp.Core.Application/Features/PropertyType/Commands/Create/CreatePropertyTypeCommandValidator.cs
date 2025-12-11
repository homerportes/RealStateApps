using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Commands.Create
{
    public class CreatePropertyTypeCommandValidator : AbstractValidator<CreatePropertyTypeCommand>
    {
        public CreatePropertyTypeCommandValidator()
        {
            RuleFor(st => st.Name)
                .NotNull()
              .WithMessage("Name is required")

            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(150).
            WithMessage("Property type name must not exceed 150 characters");

            RuleFor(st => st.Description)
                   .NotNull()
               .WithMessage("Description is required")
               .NotEmpty()
               .WithMessage("Description is required")
               .MaximumLength(250).WithMessage("Property type Description must not exceed 250 characters");
        }
    }
}
