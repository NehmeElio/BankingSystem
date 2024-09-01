using AccountService.Domain.Models;

using FluentValidation;

namespace AccountService.Application.FluentValidator;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(customer => customer.CustomerId)
            .GreaterThan(0)
            .WithMessage("Customer ID must be greater than 0.");
        
        RuleFor(customer => customer.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(2, 50).WithMessage("First name must be between 2 and 50 characters.");

        RuleFor(customer => customer.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters.");
        
        
        RuleFor(customer => customer.NumberOfAccounts)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Number of accounts cannot be negative.");
        
       
    }
}