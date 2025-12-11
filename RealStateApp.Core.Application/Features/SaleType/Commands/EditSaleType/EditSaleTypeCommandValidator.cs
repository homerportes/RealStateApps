
using FluentValidation;


namespace RealStateApp.Core.Application.Features.SaleType.Commands.EditSaleType
{
    public class EditSaleTypeCommandValidator : AbstractValidator<EditSaleTypeCommand>
    {
        public EditSaleTypeCommandValidator()
        {
            RuleFor(st => st.Id)
              .NotNull()
              .WithMessage("Id is required")
              .GreaterThan(0)
              .WithMessage("Id must be greater than 0");


            RuleFor(st => st.Name)
                .NotNull ()
                .WithMessage("Name is required")

                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(150).
                WithMessage("Sale Type name must not exceed 150 characters");

            RuleFor(st => st.Description)
                .NotNull()
               .WithMessage("Description is required")

               .NotEmpty()
               .WithMessage("Description is required")
               .MaximumLength(250).WithMessage("Sale Type Description must not exceed 250 characters");

        }
    }
}
