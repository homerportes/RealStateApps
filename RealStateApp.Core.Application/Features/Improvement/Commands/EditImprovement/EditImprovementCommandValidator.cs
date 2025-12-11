using FluentValidation;


namespace RealStateApp.Core.Application.Features.Improvement.Commands.EditImprovement
{
    public class EditImprovementCommandValidator : AbstractValidator<EditImprovementCommand>
    {
        public EditImprovementCommandValidator()
        {
            RuleFor(st => st.Id)
             .NotNull()
             .WithMessage("Id is required")
             .GreaterThan(0)
             .WithMessage("Id must be greater than 0");


            RuleFor(st => st.Name)
                .NotNull()
               .WithMessage("Description is required")
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(150).
                WithMessage("Improvement name must not exceed 150 characters");

            RuleFor(st => st.Description)
                      .NotNull()
               .WithMessage("Description is required")
               .NotEmpty()
               .WithMessage("Description is required")
               .MaximumLength(250).WithMessage("Improvement Description must not exceed 250 characters");
        }
    }
}
