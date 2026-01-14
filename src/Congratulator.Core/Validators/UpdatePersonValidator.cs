using Congratulator.SharedKernel.Contracts.Models.Requests;
using FluentValidation;

namespace Congratulator.Core.Validators;

public class UpdatePersonValidator :AbstractValidator<CreatePersonRequest>
{
    public UpdatePersonValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(32);

        RuleFor(x => x.LastName)
            .MaximumLength(64);
        
        RuleFor(x => x.BirthDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120)));
    }
}