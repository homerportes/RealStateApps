using FluentValidation;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties
{
    public class GetAgentPropertiesQueryValidator : AbstractValidator<GetAgentPropertiesQuery>
    {
        public GetAgentPropertiesQueryValidator()
        {
            RuleFor(x => x.AgentId)
                .NotNull()
                .WithMessage("Agent ID is required")

                .NotEmpty()
                .WithMessage("Agent ID is required")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Agent ID must be a valid GUID");

              
        }
    }
}
