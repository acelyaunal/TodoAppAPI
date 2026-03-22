using FluentValidation;
using TodoAppAPI.WebAPI.Contracts.Requests;

namespace TodoAppAPI.WebAPI.Validation;

public sealed class GetTodosRequestValidator : AbstractValidator<GetTodosRequest>
{
    public GetTodosRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
