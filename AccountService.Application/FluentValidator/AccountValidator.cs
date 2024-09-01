using AccountService.Domain.Models;
using FluentValidation;

namespace AccountService.Application.FluentValidator;

public class AccountValidator : AbstractValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(account => account.CustomerId)
            .NotEmpty().WithMessage("CustomerId cannot be null");
        
        RuleFor(account => account.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance cannot be negative");
        
        RuleFor(account => account.CreatedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Date cannot reference the future");


    }
}

