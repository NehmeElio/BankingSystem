
using AccountService.Persistence.Models;
using FluentValidation;

namespace AccountService.Application.FluentValidator;

public class UserValidator:AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username is required.");
        
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        
    }
}