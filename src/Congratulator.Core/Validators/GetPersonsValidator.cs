using Congratulator.SharedKernel.Contracts.Models.Requests;
using FluentValidation;

namespace Congratulator.Core.Validators;

public class GetPersonsValidator : AbstractValidator<GetPersonsRequest>
{
    public GetPersonsValidator()
    {
        RuleFor(x => x.Cursor)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
    }
}