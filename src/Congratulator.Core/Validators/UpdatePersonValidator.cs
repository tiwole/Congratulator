using Congratulator.SharedKernel.Interfaces;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using FluentValidation;

namespace Congratulator.Core.Validators;

public class UpdatePersonValidator : AbstractValidator<CreatePersonRequest>
{
    public UpdatePersonValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(32);

        RuleFor(x => x.LastName)
            .MaximumLength(64);

        RuleFor(x => x.BirthDate)
            .LessThanOrEqualTo(dateTimeProvider.Today)
            .GreaterThan(DateOnly.FromDateTime(dateTimeProvider.UtcNow.AddYears(-120)));
    }
}
