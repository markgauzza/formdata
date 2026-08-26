using bentley.DataAccess.Repositories.Interfaces;
using FluentValidation;

namespace bentley.Api.Validation
{
    public class FormDataValidator : AbstractValidator<IFormValidatable>
    {
        public FormDataValidator()
        {
            RuleFor(x => x.Priority)
                .InclusiveBetween(1, 10)
                .When(x => x.Priority.HasValue)
                .WithMessage("Priority must be between 1 and 10 if provided.");

            RuleFor(x => x.Subject)
                .NotEmpty()
                .WithMessage("Subject is required.")
                .Length(1, 200)
                .WithMessage("Subject must be between 1 and 200 characters.");

        }
    }
}
