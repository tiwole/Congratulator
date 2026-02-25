using Congratulator.SharedKernel.Contracts.Models.Requests;
using FluentValidation;

namespace Congratulator.Core.Validators;

public class CreatePersonValidator :AbstractValidator<CreatePersonRequest>
{
    public CreatePersonValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.LastName)
            .MaximumLength(64);
        
        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .GreaterThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120)));
    }
}