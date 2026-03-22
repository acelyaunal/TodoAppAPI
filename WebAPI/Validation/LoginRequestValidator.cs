using FluentValidation;
using TodoAppAPI.WebAPI.Contracts.Requests;

namespace TodoAppAPI.WebAPI.Validation;

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
