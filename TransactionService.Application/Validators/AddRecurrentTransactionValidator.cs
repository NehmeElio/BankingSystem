using FluentValidation;
using TransactionService.Application.DTO;

namespace TransactionService.Application.Validators;

public class AddRecurrentTransactionValidator : AbstractValidator<RecurrentTransactionDto>
{
    public AddRecurrentTransactionValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.IntervalValue).NotEmpty().WithMessage("Interval value is required.")
            .GreaterThan(0).WithMessage("Interval value must be greater than 0.");
    }
}