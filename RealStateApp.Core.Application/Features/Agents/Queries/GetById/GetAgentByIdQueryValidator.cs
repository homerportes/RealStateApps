using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetById
{
    public class GetAgentByIdQueryValidator: AbstractValidator<GetAgentByIdQuery>
    {
        public GetAgentByIdQueryValidator()
        {
            RuleFor(r => r.Id)
                .NotNull()
                .WithMessage("Id is required")

                .NotEmpty()
                .WithMessage("Id is required")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Id must be a valid GUID");
        }

    }
}
