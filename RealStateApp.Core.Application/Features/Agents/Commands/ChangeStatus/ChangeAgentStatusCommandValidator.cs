using FluentValidation;

namespace RealStateApp.Core.Application.Features.Agents.Commands.ChangeStatus
{
    public class ChangeAgentStatusCommandValidator : AbstractValidator<ChangeAgentStatusCommand>

    {
        public ChangeAgentStatusCommandValidator()
        {
            RuleFor(r => r.Id)
                .NotNull ()
                .WithMessage("Id is required")


              .NotEmpty()
              .WithMessage("Id is required")
              .Must(id => Guid.TryParse(id, out _))
              .WithMessage("Id must be a valid GUID");

            RuleFor(r => r.Status)
              .NotNull()
              .WithMessage("Status is required");
             
        }
    }
}
