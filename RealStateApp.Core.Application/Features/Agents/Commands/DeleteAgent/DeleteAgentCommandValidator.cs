using FluentValidation;

namespace RealStateApp.Core.Application.Features.Agents.Commands.DeleteAgent
{
    public class DeleteAgentCommandValidator : AbstractValidator<DeleteAgentCommand>
    {
        public DeleteAgentCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotNull()
                .WithMessage("Agent ID is required")

                .NotEmpty()
                .WithMessage("Agent ID is required")
                .Must(id => Guid.TryParse(id, out _))
                .WithMessage("Agent ID must be a valid GUID");
        }
    }
}
