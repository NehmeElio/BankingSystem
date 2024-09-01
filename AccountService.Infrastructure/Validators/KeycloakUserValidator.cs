using FluentValidation;
using Keycloak.Net.Models.Users;

namespace AccountService.Infrastructure.Validators;

public class KeycloakUserValidator : AbstractValidator<User>
{
    public KeycloakUserValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required.");
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

    }
}