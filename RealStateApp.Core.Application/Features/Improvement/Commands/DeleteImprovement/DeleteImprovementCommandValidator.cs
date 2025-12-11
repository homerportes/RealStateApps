using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Commands.DeleteImprovement
{
    public class DeleteImprovementCommandValidator : AbstractValidator<DeleteImprovementCommand>
    {
        public DeleteImprovementCommandValidator()
        {

            RuleFor(st => st.Id)
           .NotNull()
           .WithMessage("Id is required")
           .GreaterThan(0)
           .WithMessage("Id must be greater than 0");
        }
    }
}
